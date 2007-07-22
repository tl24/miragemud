using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using JsonExSerializer;
using Mirage.IO;

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

        public delegate void ConnectStateChangedHandler(object sender, EventArgs e);
        
        public event ResponseHandler ResponseReceived;
        public event EventHandler ConnectStateChanged;
        public event EventHandler LoginSuccess;

        public void Connect(string remoteHost, int port)
        {
            client = new TcpClient("localhost", 4501);
            NetworkStream stm = client.GetStream();
            reader = new BinaryReader(stm);
            writer = new BinaryWriter(stm);
            Thread t = new Thread(new ThreadStart(this.Run));
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
            OnConnectStateChanged();
        }

        private void Run()
        {
            string name;
            object data;
            Serializer serializer = Serializer.GetSerializer(typeof(object));
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
                            //TODO: Wrap in exception handler so it doesn't kill the thread
                            data = (Mirage.Communication.Message)serializer.Deserialize((string)data);
                            break;
                        default:
                            throw new Exception("Unrecognized response: " + type);
                    }
                    //TODO: Wrap in exception handler so it doesn't kill the thread
                    OnResponseReceived((Mirage.Communication.Message) data);
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
            finally
            {
                Close();
            }
        }

        protected void OnResponseReceived(Mirage.Communication.Message response)
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
