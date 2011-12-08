using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World.Query;
using Mirage.IO.Net;
using Mirage.Game.Communication;
using Mirage.Game.Command;
using System.Security.Principal;
using Mirage.Game.IO.Net;

namespace Mirage.Game.World
{
    /// <summary>
    /// A minimal interface for identifying a player
    /// </summary>
    public interface IPlayer : IUri, IActor
    {
        IConnectionAdapter Client { get; set; }

        void FirePlayerEvent(PlayerEventType eventType);

        event PlayerEventHandler PlayerEvent;

        /// <summary>
        ///     The Command interpreters in effect for this player
        /// </summary>
        IInterpret Interpreter { get; set; }

        ICommunicationPreferences CommunicationPreferences { get; }

    }

    public delegate void PlayerEventHandler(object sender, PlayerEventArgs eventArgs);

    public enum PlayerEventType
    {
        Disconnected,
        Quiting
    }

    public class PlayerEventArgs : EventArgs
    {
        public PlayerEventType EventType;

        public PlayerEventArgs(PlayerEventType eventType)
        {
            this.EventType = eventType;
        }

    }
}
