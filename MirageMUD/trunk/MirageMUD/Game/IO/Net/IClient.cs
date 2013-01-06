using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.IO.Net;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Handles communication to the Connection, such as translating messages
    /// into the the correct format that the connection understands.  A client type
    /// is usually paired to a connection type.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Close the client and its underlying connection
        /// </summary>
        void Close();

        /// <summary>
        /// Process input available and Execute a command
        /// </summary>
        void ProcessInput();

        /// <summary>
        /// Write a message to the client's output buffer.  The message will not be sent to the
        /// client until FlushOutput is called.
        /// </summary>
        /// <param name="message">The message to write</param>
        void Write(IMessage message);

        /// <summary>
        /// Returns true if the client had a message written to its queue during this execution loop
        /// </summary>
        /// <returns></returns>
        bool OutputWritten { get; set; }

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        bool CommandRead { get; set; }

        /// <summary>
        /// The current connected state of the Client
        /// </summary>
        ConnectedState State { get; set; }

        /// <summary>
        /// The player object associated with this client
        /// </summary>
        IPlayer Player { get; set; }

        /// <summary>
        /// The state handler for the client that will receive a series of
        /// input from the client.  If this is non-null it will take precedence over the
        /// command interpreter.
        /// </summary>
        /// <see cref="Mirage.Core.Command.LoginStateHandler"/>
        ILoginInputHandler LoginHandler { get; set; }

        /// <summary>
        /// Checks to see if the client socket is still open
        /// </summary>
        /// <returns></returns>
        bool IsOpen { get; }

        string Address { get; }
    }
}
