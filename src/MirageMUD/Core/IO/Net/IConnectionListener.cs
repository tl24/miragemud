using System.Net.Sockets;
using System.Threading.Tasks;

namespace Mirage.Core.IO.Net
{
    public interface IConnectionListener
    {
        /// <summary>
        /// Accepts a new connection and creates a client for it
        /// </summary>
        /// <returns>the client connection</returns>
        Task<SocketConnection> AcceptAsync();

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
