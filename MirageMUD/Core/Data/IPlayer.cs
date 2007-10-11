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
        IClient Client { get; set; }

        /// <summary>
        /// Writes a message to the player's output stream
        /// </summary>
        /// <param name="message">the message to write</param>
        void Write(IMessage message);

        void FirePlayerEvent(PlayerEventType eventType);

        event PlayerEventHandler PlayerEvent;

        /// <summary>
        ///     The Command interpreters in effect for this player
        /// </summary>
        IInterpret Interpreter { get; set; }

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
