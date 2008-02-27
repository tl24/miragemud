using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using Mirage.Core.IO;
using Mirage.Core.Communication;
using Mirage.Core.Command;
using System.Security.Principal;

namespace Mirage.Core.Data
{
    /// <summary>
    /// A minimal interface for identifying a player
    /// </summary>
    public interface IPlayer : IUri, IActor
    {
        IMudClient Client { get; set; }

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
