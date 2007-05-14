using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Communication;
using System.Net.Sockets;
using Shoop.Data;
using Shoop.Command;

namespace Shoop.IO
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
        /// Read a command from the client
        /// </summary>
        /// <returns>a command or null if no command ready</returns>
        string Read();

        /// <summary>
        /// Write a message to the client's output buffer.  The message will not be sent to the
        /// client until FlushOutput is called.
        /// </summary>
        /// <param name="message">The message to write</param>
        void Write(Message message);

        /// <summary>
        /// Process all messages in the output buffer and write them to the client
        /// </summary>
        void FlushOutput();

        /// <summary>
        /// The current connected state of the Client
        /// </summary>
        ConnectedState State { get; set; }

        /// <summary>
        /// The player object associated with this client
        /// </summary>
        Player Player { get; set; }

        /// <summary>
        /// The nanny that manages the initial connection state
        /// </summary>
        Nanny Nanny { get; set; }

        /// <summary>
        /// Gets or sets a reference to the ClientFactory that created this client
        /// </summary>
        IClientFactory ClientFactory { get; set; }
    }
}
