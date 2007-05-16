using System;
using System.Collections.Generic;
using System.Text;
using Shoop.IO;
using Shoop.Data;
using Shoop.Communication;
using Shoop.Data.Query;
using System.Configuration;
using System.Text.RegularExpressions;
using Shoop.Util;

namespace Shoop.Command
{
    /// <summary>
    /// StateHandler class that controls Login and player creation.
    /// </summary>
    public class LoginStateHandler : AbstractStateMachine
    {
        private bool _failed;
        private bool _echoOn;
        public LoginStateHandler(IClient client)
            : base(client)
        {
            _failed = false;
            _echoOn = true;
        }

        protected override void DetermineNextState()
        {
            if (!Contains("name"))
                Require("Nanny.Name", "Enter Name: ", new ValidateDelegate(this.ValidateName));
            else if (GetValue<bool>("isNew") == true) {
                if (!Contains("confirmName"))
                    Require("Nanny.ConfirmNewName", "Is that right, " + GetValue<Player>("player").Title + " (Y/N)? ", new ValidateDelegate(this.ConfirmName));
                else if (!Contains("password"))
                {
                    MultipartMessage message = new MultipartMessage(MessageType.Multiple, "Nanny.Password");
                    message.Parts.Add(new StringMessage(MessageType.Prompt, "Nanny.Password", "Password: "));
                    message.Parts.Add(new EchoOffMessage());

                    Require(message, new ValidateDelegate(this.ValidateNewPassword));
                }
                else if (!Contains("confirmPassword"))
                {
                    MultipartMessage message = new MultipartMessage(MessageType.Multiple, "Nanny.ConfirmPassword");
                    message.Parts.Add(new StringMessage(MessageType.Prompt, "Nanny.ConfirmPassword", "Confirm password: "));
                    message.Parts.Add(new EchoOffMessage());

                    Require(message, new ValidateDelegate(this.ConfirmPassword));
                }
                else
                {
                    Finished = true;
                }
            } else {
                if (!Contains("password"))
                {
                    MultipartMessage message = new MultipartMessage(MessageType.Multiple, "Nanny.Password");
                    message.Parts.Add(new StringMessage(MessageType.Prompt, "Nanny.Password", "Password: "));
                    message.Parts.Add(new EchoOffMessage());
                    _echoOn = false;
                    Require(message, new ValidateDelegate(this.ValidateOldPassword));
                }
                else
                {
                    Finished = true;
                }
            }
        }

        protected override void InitialState()
        {
            Client.Write(new StringMessage(MessageType.Information, "Welcome", "Welcome to the mud\n\r"));
        }

        protected override void FinalState()
        {
            if (_failed)
            {
                Client.Close();
            }
            else
            {
                Player isPlaying = (Player)GlobalLists.GetInstance().Find(new ObjectQuery(null, "Players", new ObjectQuery(GetValue<Player>("player").URI)));
                if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
                {
                    Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.AlreadyPlaying", "That player is already playing.\r\n"));
                    Client.Player = null;
                    Client.State = ConnectedState.Connecting;
                    Client.Close();
                }
                else
                {

                    //log_string( $ch->{Name}, "\@", $desc->{HOST}, " has connected." );
                    GlobalLists globalLists = GlobalLists.GetInstance();
                    globalLists.Players.Add(GetValue<Player>("player"));
                    GetValue<Player>("player").Room = (Room)globalLists.Find(ConfigurationManager.AppSettings["default.room"]);
                    GetValue<Player>("player").Room.Animates.Add(GetValue<Player>("player"));

                    Client.Write(new StringMessage(MessageType.Information, "Welcome", "\n\rWelcome to CROM 0.1.  Still in development.\n\r"));
                    //descriptor.writeToBuffer( "Color TesT: " + CLR_TEST + "\n\r");
                    Client.State = ConnectedState.Playing;
                    //Client->WriteToChannel(GLOBAL, $ch->Short . " has entered the game.\n\r",  $desc);	

                    Client.Player = GetValue<Player>("player");
                    GetValue<Player>("player").Client = Client;
                }
            }
            Client.StateHandler = null;
            Client = null;            
        }

        /// <summary>
        ///     Checks a name to see if it is valid
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>true if valid</returns>
        private bool CheckName(string name) {
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
        /// Validate the player's name.  Check to make sure the name is a valid name,
        /// if it is, then determine if it is an existing player or a new one.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateName(string input)
        {
            if (input.Length == 0) {
                _failed = true;
                Finished = true;
	            return;
	        }   
	
	        if (!CheckName( input )) {
	            Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.IllegalName", "Illegal name, try another.\n\r" ));
                return;
	        }

            Player isPlaying = (Player) GlobalLists.GetInstance().Find(new ObjectQuery(null, "Players", new ObjectQuery(input)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.AlreadyPlaying", "That player is already playing.  Try another name.\r\n"));
                return;
            }

            SetValue<string>("name", input);
            Player oldPlayer = Player.Load(input);
	        //TODO: Check Deny players
	        //TODO: Check Sitebans
	        //TODO: Check reconnect
	        //TODO: Check wizlock
            if (oldPlayer != null)
            {
                // Old player
                SetValue<bool>("isNew", false);
                SetValue<Player>("player", oldPlayer);
            }
            else
            {
                // New Player
                SetValue<bool>("isNew", true);
                /*
	            if ($GB_Newlock) {
		            descriptor.writeToBuffer( "The game is newlocked.\n\r" );
		            descriptor.close;
		            return;
	            }
                */
                // Check newbie ban
                Player player = new Player();
                player.URI = input;
                player.Title = input;
                SetValue<Player>("player", player);
            }
        }

        /// <summary>
        /// Confirm a new name for a player and that the player wants to create a new player
        /// and did not mistype the name.
        /// </summary>
        /// <param name="input"></param>
        private void ConfirmName(string input)
        {
            if (input.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
            {  // Yes
                SetValue<bool>("confirmName", true);
                Client.Write(new StringMessage(MessageType.Prompt, "Nanny.Password", "New character.\n\rEnter a password for " + GetValue<Player>("player").Title + ": "));
                Client.Write(new EchoOffMessage());
            }
            else if (input.StartsWith("n", StringComparison.CurrentCultureIgnoreCase))
            {  // No
                Client.Write(new StringMessage(MessageType.Prompt, "Nanny.NewName", "Ok, what is it, then? "));
                Clear();                
            }
            else
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.InvalidConfirmName", "Please type Yes or No? "));
            }
        }

        /// <summary>
        /// Validate the password of an existing player.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateOldPassword(string input)
        {
            Client.Write(new EchoOnMessage());
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.WrongPassword", "\n\rWrong password.\n\r"));
                Finished = _failed = true;
                return;
            }
            //return if check_playing( $desc, $ch->{Name} );
            //return if check_reconnect( $desc, $ch->{Name}, TRUE );
            // wiznet logins
            // motd stuff
            // $desc->{LINES_READ} = [] unless $desc->{LINES_READ};
            // push @{$desc->{LINES_READ}}, "\n\r";
            SetValue<string>("password", input);
        }

        /// <summary>
        /// Accept the initial password for a new player.  Verify that it meets any password
        /// standards.
        /// </summary>
        /// <param name="input"></param>
        private void ValidateNewPassword(string input)
        {
            Client.Write(new EchoOnMessage());
            if (input.Length < 5)
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.InvalidPassword", "Password must be at least five characters long.\n\r"));
                return;
            }
            GetValue<Player>("player").SetPassword(input);
            SetValue<string>("password", input);
        }

        /// <summary>
        /// Confirm the players password.  This is the 2nd time the player enters the password and is
        /// validated against their first entry.
        /// </summary>
        /// <param name="input"></param>
        private void ConfirmPassword(string input)
        {
            Client.Write(new EchoOnMessage());
            _echoOn = true;
            Client.Write(new StringMessage(MessageType.UIControl, "Newline", "\n\r"));
            if (!GetValue<Player>("player").ComparePassword(input))
            {
                Client.Write(new StringMessage(MessageType.PlayerError, "Nanny.PasswordsDontMatch", "Passwords don't match.\n\r"));
                GetValue<Player>("player").SetPassword("");
                Remove("password");
            }
            SetValue<string>("confirmPassword", input);
        }

    }
}
