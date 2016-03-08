using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Messaging;

namespace SampleMud
{
    public class SampleCommands
    {
        [Command(Aliases = new [] { "'", "say" })]
        public void Say([Actor] Player actor, [CustomParse] string message)
        {
            actor.Write(null, new StringMessage("say.self", "You said \"" + message + "\"\r\n"));
        }

        [Command]
        public void Quit([Actor] Player actor)
        {
            World.Players.Remove(actor);
            actor.Write(null, new StringMessage("quit", "Bye, bye!\r\n"));
            actor.Client.ClientState.Player = null;
            actor.Client.Close();
        }
    }
}
