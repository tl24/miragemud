using System;
using System.Net.Sockets;
namespace Mirage.Core.IO
{
    public interface IClientListener
    {
        /// <summary>
        /// Accepts a new connection and creates a client for it
        /// </summary>
        /// <returns>the client connection</returns>
        SocketConnection Accept();

        /// <summary>
        /// Returns true if there are any pending connection requests
        /// </summary>
        /// <returns></returns>
        bool Pending();

        /// <summary>
        /// Gets the socket for the listener
        /// </summary>
        Socket Socket { get; }

        /// <summary>
        /// Starts the listener listening
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the listener
        /// </summary>
        void Stop();
    }
}
