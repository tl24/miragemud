using System;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using Shoop.Command;
using System.Diagnostics;
using Shoop.Communication;

namespace Shoop.IO
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    public class ClientManager : IClientFactory
    {
        private List<IClientFactory> _factories;
        private List<IClient> _readable;
        private List<IClient> _writable;
        private List<IClient> _errored;

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ClientManager()
        {
            _factories = new List<IClientFactory>();
        }

        /// <summary>
        /// Add a client factory to be managed by this instance of the client manager.
        /// </summary>
        /// <param name="factory">the factory to be managed</param>
        public void AddFactory(IClientFactory factory)
        {
            _factories.Add(factory);
        }

        public bool Poll(int timeout)
        {
            bool result = false;
            _readable = new List<IClient>();
            _writable = new List<IClient>();
            _errored = new List<IClient>();

            if (timeout != 0 && _factories.Count > 0)
            {
                timeout /= _factories.Count;
                if (timeout == 0)
                    timeout = 1;
            }

            foreach (IClientFactory factory in _factories)
            {
                if (factory.Poll(timeout))
                {
                    result = true;
                    _readable.AddRange(factory.ReadableClients);
                    _writable.AddRange(factory.WritableClients);
                    _errored.AddRange(factory.ErroredClients);
                }
            }
            return result;
        }

        public IList<IClient> ReadableClients
        {
            get { return _readable; }
        }

        public IList<IClient> WritableClients
        {
            get { return _writable; }
        }

        public IList<IClient> ErroredClients
        {
            get { return _errored; }
        }

        public void Shutdown() {
            foreach (IClientFactory factory in _factories)
            {
                factory.Shutdown();
            }
        }

        public void Remove(IClient client)
        {
            foreach (IClientFactory factory in _factories)
            {
                factory.Remove(client);
            }
        }
    }




}
