using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Communication;
using System.Net.Sockets;
using Mirage.Data;
using Mirage.Command;
using Mirage.Util;

namespace Mirage.IO
{
    /// <summary>
    /// The IClient interface specifies the interface for reading input and
    /// writing output to the remote client connected to the mud.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Initialize the client with its TCP client socket connection.
        /// </summary>
        /// <param name="client">The TcpClient that this client will be connected to</param>
        void Open(TcpClient client);

        /// <summary>
        /// Close the connection and its underlying connection
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
        void Write(Message message);

        /// <summary>
        /// Write the prompt for the client to the output buffer
        /// </summary>
        void WritePrompt();

        /// <summary>
        /// Process all messages in the output buffer and write them to the client
        /// </summary>
        void FlushOutput();

        /// <summary>
        /// Returns true if the client has output waiting to be sent
        /// </summary>
        /// <returns></returns>
        bool HasOutput();

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
        Player Player { get; set; }

        /// <summary>
        /// The state handler for the client that will receive a series of
        /// input from the client.  If this is non-null it will take precedence over the
        /// command interpreter.
        /// </summary>
        /// <see cref="Mirage.Command.LoginStateHandler"/>
        ILoginInputHandler LoginHandler { get; set; }

        /// <summary>
        /// Gets or sets a reference to the ClientFactory that created this client
        /// </summary>
        IClientFactory ClientFactory { get; set; }

        /// <summary>
        /// Checks to see if the client socket is still open
        /// </summary>
        /// <returns></returns>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the underlying TcpClient socket
        /// </summary>
        TcpClient TcpClient { get; }

    }
}
