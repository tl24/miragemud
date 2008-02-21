using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Mirage.Core.IO
{
    public interface ITelnetClient : IMudClient, IDisposable
    {

        /// <summary>
        /// Read from the socket and populate the input queue
        /// </summary>
        void ReadInput();

        /// <summary>
        /// Process all messages in the output buffer and write them to the client
        /// </summary>
        void FlushOutput();

        /// <summary>
        /// Gets the underlying TcpClient socket
        /// </summary>
        TcpClient TcpClient { get; }

        /// <summary>
        /// Gets or sets a reference to the ClientFactory that created this client
        /// </summary>
        IClientFactory ClientFactory { get; set; }
    }
}
