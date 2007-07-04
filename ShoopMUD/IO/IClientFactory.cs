using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.IO
{
    /// <summary>
    /// A client factory is responsible for create IClient instances and
    /// managing their lifetime.  Calls to the factory are expected to go
    /// in this order:
    /// Poll
    /// Any one of: ReadableClients, GetWriteable, GetErrored
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Polls the clients managed by this factory to see if any are ready to be read,
        /// written, or have errors.  If true is returned then at least one client matches those states
        /// and at least one of the calls to ReadableClients, WritableClients, or GetErrored should return a non-empty
        /// list.
        /// </summary>
        /// <param name="timeout">The timeout to wait for events</param>
        /// <returns>true if any clients are ready for reading, writing or in error state</returns>
        bool Poll(int timeout);

        /// <summary>
        /// Get the clients that are ready to be read from for the last call to Poll.
        /// </summary>
        /// <returns>a list of readable clients</returns>
        IList<IClient> ReadableClients { get; }

        /// <summary>
        /// Get the clients that are ready to be written to from the last call to Poll.
        /// </summary>
        /// <returns>list of writable clients</returns>
        IList<IClient> WritableClients { get; }

        /// <summary>
        /// Get the clients that are in an error state from the last call to Poll.
        /// </summary>
        /// <returns>list of errored clients</returns>
        IList<IClient> ErroredClients { get; }

        /// <summary>
        /// Shutdown this factory, close any listeners and any connected clients.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Remove this client from the factory's managed clients
        /// </summary>
        /// <param name="client"></param>
        void Remove(IClient client);
    }
}
