using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Util;
using System.Net.Sockets;

namespace Mirage.IO
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
        /// Starts this factory listening for connections
        /// </summary>
        void Start();

        /// <summary>
        /// Accepts a socket from the listener and creates a client object from it and
        /// returns it
        /// </summary>
        /// <returns></returns>
        IClient Accept();

        /// <summary>
        /// Determines if there are connections waiting to be read
        /// </summary>
        /// <returns></returns>
        bool Pending();

        /// <summary>
        /// Stop this factory, close any listeners and any connected clients.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets the underlying socket for the listener
        /// </summary>
        Socket Socket { get; }
    }
}
