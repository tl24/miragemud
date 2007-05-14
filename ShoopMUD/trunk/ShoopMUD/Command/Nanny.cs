using System;
using System.Collections.Generic;
using System.Text;
using Shoop.IO;
using Shoop.Data;
using System.Text.RegularExpressions;
using System.Configuration;
using Shoop.Communication;
using Shoop.Data.Query;
using System.Security.Principal;

namespace Shoop.Command
{
    /// <summary>
    ///     The Nanny handles connections until they have either loaded a previous player
    /// or have created a new one
    /// </summary>
    public class Nanny
    {
        private IClient _client;
        private Player player;
        private delegate void InputHandler(string input);
        private InputHandler[] handlers;
        private states currentState;
        private bool isNew = false;

        private enum states
        {            
            GetName,
            GetOldPassword,
            ConfirmNewName,
            GetNewPassword,
            ConfirmPassword,
            Connected
        }

        /// <summary>
        ///     Creates an instance of the nanny for this client.
        /// </summary>
        /// <param name="client">the client</param>
        private Nanny(IClient client)
        {
            this._client = client;
            handlers = new InputHandler[Enum.GetNames(typeof(states)).Length];
            this.currentState = states.GetName;
            handlers[(int)states.GetName] = new InputHandler(getName);
            handlers[(int)states.GetOldPassword] = new InputHandler(getOldPassword);
            handlers[(int)states.ConfirmNewName] = new InputHandler(confirmNewName);
            handlers[(int)states.GetNewPassword] = new InputHandler(getNewPassword);
            handlers[(int)states.ConfirmPassword] = new InputHandler(confirmPassword);
            handlers[(int)states.Connected] = new InputHandler(finish);
        }

        /// <summary>
        ///     Creates a nanny instance for the given descriptor.  The nanny
        /// will handle input and output until the descriptor is fully connected.
        /// </summary>
        /// <param name="descriptor">the descriptor</param>
        /// <returns>nanny instance</returns>
        public static Nanny getInstance(IClient client)
        {
            return new Nanny(client);
        }

        /// <summary>
        ///     Processes the current input of the descriptor
        /// </summary>
        /// <param name="input">current input</param>
        public void handleInput(string input)
        {
            input = input.TrimStart();
            int i = (int)currentState;
            InputHandler handler = handlers[i];
            handler(input);
        }

        /// <summary>
        ///     Get the players name (user id)
        /// </summary>
        /// <param name="input">input</param>
        private void getName(string input)
        {
            if (input.Length == 0) {
                //TODO: Make sure everything is cleaned up
        	    _client.Close();
	            return;
	        }   
	
	        if (!checkName( input )) {
	            _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.IllegalName", "Illegal name, try another.\n\r" ));
                _client.Write(new StringMessage(MessageType.Prompt, "Nanny.Name", "Name: "));
                return;
	        }

            Player isPlaying = (Player) GlobalLists.GetInstance().Find(new ObjectQuery(null, "Players", new ObjectQuery(input)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.AlreadyPlaying", "That player is already playing.\r\n"));
                _client.Write(new StringMessage(MessageType.Prompt, "Nanny.Name", "Name: "));
                return;
            }

            Player oldPlayer = Player.Load(input);
            player = oldPlayer;
	        //TODO: Check Deny players
	        //TODO: Check Sitebans
	        //TODO: Check reconnect
	        //TODO: Check wizlock
        	if (oldPlayer != null) {
	            // Old player
                isNew = false;
	            _client.Write(new StringMessage(MessageType.Prompt, "Nanny.Password", "Password: "));
                _client.Write(new EchoOffMessage());
	            currentState = states.GetOldPassword;
	            return;
	        } else {
	            // New Player
                isNew = true;
                /*
	            if ($GB_Newlock) {
		            descriptor.writeToBuffer( "The game is newlocked.\n\r" );
		            descriptor.close;
		            return;
	            }
                */
	            // Check newbie ban
                player = new Player();
                player.URI = input;
                player.Title = input;
	            _client.Write(new StringMessage(MessageType.Prompt, "Nanny.ConfirmNewName", "Is that right, " + player.Title + " (Y/N)? " ));
                currentState = states.ConfirmNewName;
	            return;
	        }	  
        }

        /// <summary>
        ///     Checks a name to see if it is valid
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>true if valid</returns>
        private bool checkName(string name) {
            Regex parser = new Regex(@"all|auto|immortal|self|someone|something|the|you|loner|none");
            if (parser.IsMatch(name)) {
                return false;
            }
    
            if (name.Length < 2 || name.Length > 12) {
                return false;
            }

            // check valid characters
            parser = new Regex(@"^[a-zA-Z][a-z0-9]+$");
            if (!parser.IsMatch(name)) {
                return false;
            }

            //TODO: check mob names
	        return true;
        }

        /// <summary>
        ///     Get an existing player's password
        /// </summary>
        /// <param name="input">password</param>
        private void getOldPassword(string input) {
            _client.Write(new EchoOnMessage());
            if (!player.ComparePassword(input))
            {
	            _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.WrongPassword", "\n\rWrong password.\n\r" ));
	            _client.Close();
    	        return;
	        }
            //return if check_playing( $desc, $ch->{Name} );
            //return if check_reconnect( $desc, $ch->{Name}, TRUE );
            // wiznet logins
            // motd stuff
            // $desc->{LINES_READ} = [] unless $desc->{LINES_READ};
            // push @{$desc->{LINES_READ}}, "\n\r";
            currentState = states.Connected;
            finish(input);
	        return;
        }

        /// <summary>
        ///     Confirm a new player's name
        /// </summary>
        /// <param name="input">y or n</param>
        private void confirmNewName(string input) {
	        if (input.StartsWith("y", StringComparison.CurrentCultureIgnoreCase)) {  // Yes
	            _client.Write(new StringMessage(MessageType.Prompt, "Nanny.Password", "New character.\n\rEnter a password for " + player.Title + ": "));
                _client.Write(new EchoOffMessage());
                currentState = states.GetNewPassword;
	        } else if (input.StartsWith("n", StringComparison.CurrentCultureIgnoreCase)) {  // No
	            _client.Write(new StringMessage(MessageType.Prompt, "Nanny.NewName", "Ok, what is it, then? "));
	            player = null;
	            currentState = states.GetName;
	        } else {
                _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.InvalidConfirmName", "Please type Yes or No? "));
	        }
        }

        /// <summary>
        ///     Get a new password for a new player
        /// </summary>
        /// <param name="input">new password</param>
        private void getNewPassword(string input) {
    	    _client.Write(new EchoOnMessage());
	        if (input.Length < 5) {
	            _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.InvalidPassword", "Password must be at least five characters long.\n\r"));
				_client.Write(new StringMessage(MessageType.Prompt,"Nanny.Password", "Password: "));
	            _client.Write(new EchoOffMessage());
	            return;
	        }
            player.SetPassword( input );
	        _client.Write(new StringMessage(MessageType.Prompt,"Nanny.ConfirmPassword", "Confirm password: "));
	        _client.Write(new EchoOffMessage());
	        currentState = states.ConfirmPassword;	
        }

        /// <summary>
        ///     Confirm a password entered
        /// </summary>
        /// <param name="input">password</param>
        private void confirmPassword(string input) {
	        _client.Write(new StringMessage(MessageType.UIControl, "Newline", "\n\r"));
            if (!player.ComparePassword(input)) {
                _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.PasswordsDontMatch", "Passwords don't match.\n\r"));
                _client.Write(new StringMessage(MessageType.Prompt, "Nanny.Password", "Retype password: "));
                _client.Write(new EchoOffMessage());
	            player.SetPassword("");
                currentState = states.GetNewPassword;
        	    return;
	        }
            _client.Write(new EchoOnMessage());
            currentState = states.Connected;
            finish(input);
        }

        /// <summary>
        ///     Final state
        /// </summary>
        /// <param name="input"></param>
        private void finish(string input)
        {
            Player isPlaying = (Player)GlobalLists.GetInstance().Find(new ObjectQuery(null, "Players", new ObjectQuery(player.URI)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                _client.Write(new StringMessage(MessageType.PlayerError, "Nanny.AlreadyPlaying", "That player is already playing.\r\n"));
                _client.Player = null;
                _client.State = ConnectedState.Connecting;
                _client.Close();
                return;
            }

            //log_string( $ch->{Name}, "\@", $desc->{HOST}, " has connected." );
            GlobalLists globalLists = GlobalLists.GetInstance();
            globalLists.Players.Add(player);
            player.Room = (Room) globalLists.Find(ConfigurationManager.AppSettings["default.room"]);
            player.Room.Animates.Add(player);

	        _client.Write (new StringMessage(MessageType.Information, "Welcome", "\n\rWelcome to CROM 0.1.  Still in development.\n\r" ));
	        //descriptor.writeToBuffer( "Color TesT: " + CLR_TEST + "\n\r");
	        _client.State = ConnectedState.Playing;
	        //Client->WriteToChannel(GLOBAL, $ch->Short . " has entered the game.\n\r",  $desc);	

            _client.Player = player;
            player.Client = _client;
            _client.Nanny = null;
            _client = null;
            player = null;

	        return;
        }
    }
}
