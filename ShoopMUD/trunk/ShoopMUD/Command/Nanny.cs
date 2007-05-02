using System;
using System.Collections.Generic;
using System.Text;
using Shoop.IO;
using Shoop.Data;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Shoop.Command
{
    /// <summary>
    ///     The Nanny handles connections until they have either loaded a previous player
    /// or have created a new one
    /// </summary>
    public class Nanny
    {
        private Descriptor descriptor;
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
        ///     Creates an instance of the nanny for this descriptor.
        /// </summary>
        /// <param name="descriptor">the descriptor</param>
        private Nanny(Descriptor descriptor)
        {
            this.descriptor = descriptor;
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
        public static Nanny getInstance(Descriptor descriptor)
        {
            return new Nanny(descriptor);
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
        	    descriptor.close();
	            return;
	        }   
	
	        if (!checkName( input )) {
	            descriptor.writeToBuffer( "Illegal name, try another.\n\rName: " );
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
	            descriptor.writeToBuffer( "Password: ", false );
                descriptor.echoOff();
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
	            descriptor.writeToBuffer("Is that right, " + player.Title + " (Y/N)? " );
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
            descriptor.echoOn();
            if (!player.ComparePassword(input))
            {
	            descriptor.writeToBuffer( "\n\rWrong password.\n\r" );
	            descriptor.close();
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
	            descriptor.writeToBuffer("New character.\n\rEnter a password for " + player.Title + ": ");
                descriptor.echoOff();
                currentState = states.GetNewPassword;
	        } else if (input.StartsWith("n", StringComparison.CurrentCultureIgnoreCase)) {  // No
	            descriptor.writeToBuffer("Ok, what is it, then? ");
	            player = null;
	            currentState = states.GetName;
	        } else {
	            descriptor.writeToBuffer("Please type Yes or No? ");
	        }
        }

        /// <summary>
        ///     Get a new password for a new player
        /// </summary>
        /// <param name="input">new password</param>
        private void getNewPassword(string input) {
    	    descriptor.echoOn();
	        if (input.Length < 5) {
	            descriptor.writeToBuffer(
				   "Password must be at least five characters long.\n\rPassword: ");
	            descriptor.echoOff();
	            return;
	        }
            player.SetPassword( input );
	        descriptor.writeToBuffer("Confirm password: ");
	        descriptor.echoOff();
	        currentState = states.ConfirmPassword;	
        }

        /// <summary>
        ///     Confirm a password entered
        /// </summary>
        /// <param name="input">password</param>
        private void confirmPassword(string input) {
	        descriptor.writeToBuffer("\n\r");
            if (!player.ComparePassword(input)) {
	            descriptor.writeToBuffer("Passwords don't match.\n\rRetype password: ");
	            descriptor.echoOff();
	            player.SetPassword("");
                currentState = states.GetNewPassword;
        	    return;
	        }
            descriptor.echoOn();
            currentState = states.Connected;
            finish(input);
        }

        /// <summary>
        ///     Final state
        /// </summary>
        /// <param name="input"></param>
        private void finish(string input)
        {	
            //log_string( $ch->{Name}, "\@", $desc->{HOST}, " has connected." );
            GlobalLists globalLists = GlobalLists.GetInstance();
            globalLists.Players.Add(player);
            player.Room = (Room) globalLists.Find(ConfigurationManager.AppSettings["default.room"]);
            player.Room.Animates.Add(player);

	        descriptor.writeToBuffer ( "\n\rWelcome to CROM 0.1.  Still in development.\n\r" );
	        //descriptor.writeToBuffer( "Color TesT: " + CLR_TEST + "\n\r");
	        descriptor.state = ConnectedState.Playing;
	        //Descriptor->WriteToChannel(GLOBAL, $ch->Short . " has entered the game.\n\r",  $desc);	
            descriptor.player = player;
            player.Descriptor = descriptor;
            descriptor.nanny = null;
            descriptor = null;
            player = null;
	        return;
        }
    }
}
