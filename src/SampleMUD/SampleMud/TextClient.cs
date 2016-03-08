using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.IO.Net;
using Mirage.Core.Command;
using Mirage.Core.Messaging;

namespace SampleMud
{
    public class TextClient : TextClientBase<ClientState>
    {
        public TextClient(TextConnection connection)
            : base(connection)
        {
        }

        protected override void OnInputReceived(string input)
        {
            if (ClientState.Player == null)
            {
                HandleLogin(input);
            }
            else
            {
                CommandInvoker.Instance.Interpret(ClientState.Player, input);
            }
        }

        private void HandleLogin(string input)
        {
            // very simple login, ask for name only
            // this should be the name
            if (string.IsNullOrEmpty(input))
            {
                // no name...disconnect
                Write(new StringMessage("Login.NoName", "No name specified, disconnecting.\r\n"));
                Close();
                return;
            }

            var otherPlayer = World.Players.Find(p => p.Name.Equals(input, StringComparison.CurrentCultureIgnoreCase));
            if (otherPlayer != null)
            {
                Write(new StringMessage("Login.AlreadyPlaying", "That person is already logged in.\r\n"));
                Write(new StringMessage("Login", "Enter your name: "));
                return;
            }

            // logged in
            Player player = new Player(input);
            player.Client = this;
            ClientState.Player = player;
            World.Players.Add(player);
            Write(new StringMessage("Login.success", "Welcome, " + input + "!\r\n"));
            return;
        }
    }
}
