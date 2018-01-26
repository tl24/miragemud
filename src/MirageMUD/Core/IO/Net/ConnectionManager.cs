using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Mirage.Core.Collections;
using Castle.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace Mirage.Core.IO.Net
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    public class ConnectionManager
    {
        private List<IConnectionListener> _listeners;
        private BlockingCollection<IConnection> _newClients;
        private bool _started = false;
        private ConcurrentQueue<SocketConnection> _internalNewClients;

        private IList<Socket> _sockets;
        private Dictionary<Socket, object> _clientMap = new Dictionary<Socket, object>();
        private Task _runTask;
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ConnectionManager(IList<IConnectionListener> listeners)
        {
            _listeners = new List<IConnectionListener>(listeners);
            _internalNewClients = new ConcurrentQueue<SocketConnection>();
            _sockets = new List<Socket>();
        }

        /// <summary>
        /// Polls the listening clients to see which ones are ready to read, ready to be written
        /// to and have errors.  These lists can then be access through the ReadableClients,
        /// WritableClients, and ErroredClients properties.
        /// </summary>
        protected virtual async Task Run(CancellationToken token)
        {
            StartListeners();

            DateTime startTime;
            TimeSpan elapsed;
            while (!token.IsCancellationRequested)
            {
                startTime = DateTime.Now;

                RemoveClosedConnections();
                await PollForIO();

                // add any new clients
                AddNewClients();

                // make sure there's some time between polls so we don't burn up all the cpu
                elapsed = DateTime.Now.Subtract(startTime);
                if (elapsed.TotalMilliseconds < 50)
                {
                    Thread.Sleep(50 - (int)elapsed.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Adds any new clients found during the Poll phase
        /// </summary>
        private void AddNewClients()
        {
            SocketConnection newClient;
            while (_internalNewClients.TryDequeue(out newClient))
            {
                _sockets.Add(newClient.TcpClient.Client);
                _clientMap.Add(newClient.TcpClient.Client, newClient);
                _newClients.Add(newClient);
            }
        }

        /// <summary>
        /// Polls the sockets to see if any have any pending activity and the queues the
        /// appropriate action
        /// </summary>
        private async Task PollForIO()
        {
            //TODO: Use Socket.DuplicateAndClose for copyover support
            List<Socket> checkRead = new List<Socket>(_sockets);
            List<Socket> checkWrite = new List<Socket>(_sockets);
            List<Socket> checkError = new List<Socket>(_sockets);

            Socket.Select(checkRead, checkWrite, checkError, 100000);
            
            List<Task> ops = new List<Task>(checkRead.Count + checkWrite.Count + checkError.Count);
            foreach (Socket s in checkRead)
            {
                object client = _clientMap[s];
                if (client is IConnectionListener)
                    ops.Add(ProcessOperation((IConnectionListener)client, AcceptClient));
                else
                    ops.Add(ProcessOperation((SocketConnection)client, ReadFromConnection));
            }
            ops.AddRange(checkWrite.Select(s => ProcessOperation((SocketConnection)_clientMap[s], WriteToConnection)));
            ops.AddRange(checkError.Select(s => ProcessOperation((SocketConnection)_clientMap[s], ProcessConnectionError)));
            await Task.WhenAll(ops.ToArray());
        }

        private void StartListeners()
        {
            _sockets.Clear();
            _clientMap.Clear();

            // start the factories
            foreach (IConnectionListener listener in _listeners)
            {
                listener.Start();
                _sockets.Add(listener.Socket);
                _clientMap.Add(listener.Socket, listener);
            }
        }

        /// <summary>
        /// Removes and cleans up any closed or errored connections
        /// </summary>
        private void RemoveClosedConnections()
        {
            Queue<Socket> removeItems = new Queue<Socket>();

            foreach (KeyValuePair<Socket, object> entry in _clientMap)
            {
                Socket skey = (Socket)entry.Key;
                SocketConnection client = entry.Value as SocketConnection;
                if (client != null)
                {
                    if (!client.IsOpen)
                    {
                        client.FlushOutputAsync();
                        // close the connection
                        client.Dispose();
                        // remove from the list
                        removeItems.Enqueue(skey);
                    }
                }
            }

            while (removeItems.Count > 0)
            {
                Socket sck = removeItems.Dequeue();
                _clientMap.Remove(sck);
                _sockets.Remove(sck);
            }
        }

        private async Task ProcessOperation<T>(T arg, Func<T, Task> operation)
        {
            try
            {
                await operation(arg);
            }
            catch (IOException)
            {
                var connection = arg as SocketConnection;
                if (connection != null)
                    // error, close the connection, main thread will clean it up
                    connection.Close();
            }
        }

        private async Task AcceptClient(IConnectionListener listener)
        {
            SocketConnection client = await listener.AcceptAsync();
            _internalNewClients.Enqueue(client);
        }

        private async Task ReadFromConnection(SocketConnection client)
        {
            await client.ReadInputAsync();
        }

        private async Task WriteToConnection(SocketConnection client)
        {
            await client.FlushOutputAsync();
        }

        private Task ProcessConnectionError(SocketConnection client)
        {
            client.Close();
            return Task.CompletedTask;
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();            
            _runTask = Task.Run(() => Run(_tokenSource.Token));            
            _started = true;
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            foreach (IConnectionListener listener in _listeners)
            {
                listener.Stop();
            }
            _runTask.Wait();
            _started = false;
        }

        public BlockingCollection<IConnection> NewClients
        {
            get { return this._newClients; }
            set
            {
                if (_started)
                {
                    throw new InvalidOperationException("NewClients can not be set while the factory is running");
                }
                this._newClients = value;
            }
        }
    }
}
