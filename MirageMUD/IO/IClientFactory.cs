using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Util;
using System.Net.Sockets;

namespace Mirage.IO
{
    /// <summary>
    /// A client factory is responsible for create IClient instances from
    /// a tcpclient socket.  The client handles the implementation details
    /// of converting from on-the-wire protocol to commands and messages based
    /// on the type of client connected.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Creates an IClient instance from the TcpClient client.  Each client factory
        /// is responsible for configuring the newly created client
        /// </summary>
        /// <param name="client">the tcp client</param>
        /// <returns>mud client instance</returns>
        IClient CreateClient(TcpClient client);
    }
}
