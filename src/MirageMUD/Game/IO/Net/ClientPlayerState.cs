using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.IO.Net;
using Mirage.Game.World;
using Mirage.Game.Command;

namespace Mirage.Game.IO.Net
{
    public class ClientPlayerState
    {
        /// <summary>
        /// The current connected state of the Client
        /// </summary>
        public ConnectedState State { get; set; }

        /// <summary>
        /// The player object associated with this client
        /// </summary>
        public IPlayer Player { get; set; }

        /// <summary>
        /// The state handler for the client that will receive a series of
        /// input from the client.  If this is non-null it will take precedence over the
        /// command interpreter.
        /// </summary>
        /// <see cref="Mirage.Core.Command.LoginStateHandler"/>
        public ILoginInputHandler LoginHandler { get; set; }
    }
}
