using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Messaging;

namespace Mirage.Game.Communication
{
    public static class CommonMessages
    {
        public static readonly MessageDefinition EchoOn = new MessageDefinition("system.EchoOn", "\u001b[0m" );
        public static readonly MessageDefinition EchoOff = new MessageDefinition("system.EchoOff", "\u001b[0;30;40m");
        public static readonly MessageDefinition Newline = new MessageDefinition("system.Newline", "\r\n");
        public static readonly MessageDefinition ConfirmationPrompt = new MessageDefinition("system.ConfirmationPrompt", "Are you sure? (y\\n) ");
        public static readonly MessageDefinition ConfirmationCancel = new MessageDefinition("system.ConfirmationCancel", "Command cancelled.");
        public static readonly MessageDefinition ErrorNotHere = new MessageDefinition("common.error.NotHere", "${target} is not here.");
        public static readonly MessageDefinition ErrorPlayerNotPlaying = new MessageDefinition("common.error.playernotplaying", "${target} is not playing right now.");
        public static readonly MessageDefinition ErrorInvalidCommand = new MessageDefinition("common.error.invalidcommand", "Huh?");
        public static readonly MessageDefinition ErrorSystem = new MessageDefinition("common.error.system", "A system error has occurred executing your command.");

        public static readonly MessageDefinition Welcome = new MessageDefinition("system.welcome", "\r\nWelcome to MirageMUD 0.1.  Still in development.");
    }
}
