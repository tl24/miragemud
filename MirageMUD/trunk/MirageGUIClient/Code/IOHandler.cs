using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using JsonExSerializer;
using Mirage.Game.Communication;
using Mirage.IO.Net;
using Mirage.Core.Messaging;

namespace MirageGUI.Code
{
    public class IOHandler
    {
        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;
        private object syncObject = new object();
        private string _host;
        private int _port;
        private bool _isClosing = false;
        private string closeError = null;

        public delegate void ConnectStateChangedHandler(object sender, EventArgs e);
        
        public event ResponseHandler ResponseReceived;
        public event EventHandler ConnectStateChanged;
        public event EventHandler LoginSuccess;

        public void Connect(string remoteHost, int port)
        {
            client = new TcpClient(remoteHost, port);
            NetworkStream stm = client.GetStream();
            reader = new BinaryReader(stm);
            writer = new BinaryWriter(stm);
            Thread t = new Thread(new ThreadStart(this.Run));
            t.IsBackground = true;
            t.Start();
            _host = remoteHost;
            _port = port;
            OnConnectStateChanged();
        }

        /// <summary>
        /// Returns true if there is an active connection to the server
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (client != null && client.Connected)
                    return true;
                else
                    return false;
            }
        }

        public void Close()
        {
            Close(null);
        }

        /// <summary>
        /// Closes the connection and sends out the closeReasonMessage
        /// if applicable.
        /// </summary>
        /// <param name="closeReasonMessage">The reason the connection was closed</param>
        private void Close(string closeReasonMessage)
        {
            if (_isClosing)
                return;
            _isClosing = true;
            bool wasConnected = reader != null || writer != null;
            if (IsConnected)
                client.Close();
            if (reader != null)
                reader.Close();
            if (writer != null)
                writer.Close();
            client = null;
            reader = null;
            writer = null;
            _host = null;
            _port = 0;
            if (wasConnected)
                OnConnectStateChanged();
            if (closeReasonMessage != null)
                OnResponseReceived(new StringMessage(MessageType.SystemError, new MessageName("common.error.GuiError", "ConnectionError"), closeReasonMessage));
            _isClosing = false;
        }

        private void Run()
        {
            string name;
            object data;
            Serializer serializer = Serializer.GetSerializer(typeof(object));
            string closeError = null;

            try
            {
                while (true)
                {
                    int type = reader.ReadInt32();
                    switch ((AdvancedClientTransmitType)type)
                    {
                        case AdvancedClientTransmitType.StringMessage:
                            name = "";
                            data = reader.ReadString();
                            break;
                        case AdvancedClientTransmitType.JsonEncodedMessage:
                            name = reader.ReadString();
                            data = reader.ReadString();
                            try
                            {
                                data = (Mirage.Core.Messaging.Message)serializer.Deserialize((string)data);
                            }
                            catch (Exception e)
                            {
                                data = new ExceptionMessage("JsonParseError", e, data);
                            }
                            break;
                        default:
                            throw new Exception("Unrecognized response: " + type);
                    }
                    try
                    {
                        OnResponseReceived((Mirage.Core.Messaging.Message)data);
                    }
                    catch (Exception e)
                    {
                        // event handler errored out, try to send out the error
                        Mirage.Core.Messaging.Message error = new ExceptionMessage("ResponseHandlerException", e, data);
                        OnResponseReceived(error);
                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                // do nothing, the mud closed the connection
                closeError = null;
            }
            catch (Exception ex)
            {

                closeError = ex.Message;
            }
            finally
            {
                Close(closeError);
            }
        }

        protected void OnResponseReceived(Mirage.Core.Messaging.Message response)
        {
            if (ResponseReceived != null)
                ResponseReceived(response);
        }

        protected void OnConnectStateChanged()
        {
            if (ConnectStateChanged != null)
                ConnectStateChanged(this, new EventArgs());
        }

        public void OnLogin()
        {
            if (LoginSuccess != null)
                LoginSuccess(this, new EventArgs());
        }
        /// <summary>
        /// Sends string data to the connection
        /// </summary>
        /// <param name="data">the data to send</param>
        public void SendString(string data)
        {
            writer.Write((int)AdvancedClientTransmitType.StringMessage);
            writer.Write(data);
        }

        public void SendObject(string name, object o)
        {
            writer.Write((int)AdvancedClientTransmitType.JsonEncodedMessage);
            writer.Write(name);
            Serializer serializer = Serializer.GetSerializer(typeof(object));
            serializer.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
            writer.Write(serializer.Serialize(o));
        }

        public string Host
        {
            get { return this._host; }
        }

        public int Port
        {
            get { return this._port; }
        }
    }
}
