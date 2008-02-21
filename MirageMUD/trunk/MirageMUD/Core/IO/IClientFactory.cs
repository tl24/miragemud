using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Util;
using System.Net.Sockets;

namespace Mirage.Core.IO
{
    /// <summary>
    /// A client factory is responsible for create IMudClient instances from
    /// a tcpclient socket.  The client handles the implementation details
    /// of converting from on-the-wire protocol to commands and messages based
    /// on the type of client connected.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Creates an IMudClient instance from the TcpClient client.  Each client factory
        /// is responsible for configuring the newly created client
        /// </summary>
        /// <param name="client">the tcp client</param>
        /// <returns>mud client instance</returns>
        ITelnetClient CreateClient(TcpClient client);
    }
}
