using System;
using Mirage.Game.Command;
using Mirage.Game.IO.Net;
using Mirage.Core.Command;
using Mirage.Core.IO.Net;

namespace Mirage.Game.World
{
    /// <summary>
    /// A minimal interface for identifying a player
    /// </summary>
    public interface IPlayer : IActor
    {
        IClient<ClientPlayerState> Client { get; set; }

        void WritePrompt();

        string Name { get; }

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