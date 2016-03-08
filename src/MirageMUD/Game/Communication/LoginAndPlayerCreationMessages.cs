using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Messaging;

namespace Mirage.Game.Communication
{
    public static class LoginAndPlayerCreationMessages
    {
        public static readonly MessageDefinition Example = new MessageDefinition("namespace", "message text");
        public static readonly MessageDefinition PlayerAlreadyPlaying = new MessageDefinition("negotiation.authentication.PlayerAlreadyPlaying", "That player is already playing.");
		public static readonly MessageDefinition NewplayerPassword = new MessageDefinition("negotiation.authentication.NewplayerPassword", "New character.\r\nEnter a password for ${player}: ");	
		public static readonly MessageDefinition ExistingplayerPassword = new MessageDefinition("negotiation.authentication.ExistingplayerPassword", "Password: ");
		public static readonly MessageDefinition EnterName = new MessageDefinition("negotiation.authentication.EnterName", "Enter Name: ");
		public static readonly MessageDefinition EnterAnotherName = new MessageDefinition("negotiation.authentication.EnterAnotherName", "Ok, what is it, then? ");
		public static readonly MessageDefinition ConfirmNewName = new MessageDefinition("negotiation.authentication.ConfirmNewName", "Is that right, ${player} (Y/N)? "); 
		public static readonly MessageDefinition ErrorConfirmName = new MessageDefinition("negotiation.authentication.ErrorConfirmName", "Please type Yes or No.\r\n");
		public static readonly MessageDefinition ConfirmPassword = new MessageDefinition("negotiation.authentication.ConfirmPassword", "Confirm password: ");
		public static readonly MessageDefinition ErrorIllegalName = new MessageDefinition("negotiation.authentication.ErrorIllegalName", "Illegal name, try another.\r\n");
		public static readonly MessageDefinition ErrorAlreadyPlaying = new MessageDefinition("negotiation.authentication.ErrorAlreadyPlaying", "That player is already playing.  Try another name.\r\n");
		public static readonly MessageDefinition ErrorWrongPassword = new MessageDefinition("negotiation.authentication.ErrorWrongPassword", "\r\nWrong Password\r\n");
		public static readonly MessageDefinition ErrorPasswordLength = new MessageDefinition("negotiation.authentication.ErrorPasswordLength", "Password must be at least five characters long.\n\r");
		public static readonly MessageDefinition ErrorPasswordsDontMatch  = new MessageDefinition("negotiation.authentication.ErrorPasswordsDontMatch", "Passwords don't match.\r\n");
    }
}
