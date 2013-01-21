using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Messaging;

namespace Mirage.Core.IO.Net
{
    /// <summary>
    /// Handles communication to the Connection, such as translating messages
    /// into the the correct format that the connection understands.  A client type
    /// is usually paired to a connection type.
    /// </summary>
    public interface IClient<TState> where TState : new()
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
        /// State for the client such as the connected player instance and any other information necessary
        /// </summary>
        TState ClientState { get; }

        /// <summary>
        /// Checks to see if the client socket is still open
        /// </summary>
        /// <returns></returns>
        bool IsOpen { get; }

        string Address { get; }
    }
}
