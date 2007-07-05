using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Mirage.IO;
using System.Threading;
using JsonExSerializer;
using Mirage.Communication;

namespace MirageGUIClient
{
    public partial class frmConsole : Form
    {
        public TcpClient client;
        public BinaryReader reader;
        public BinaryWriter writer;

        public frmConsole()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient("localhost", 4501);
                NetworkStream stm = client.GetStream();
                reader = new BinaryReader(stm);
                writer = new BinaryWriter(stm);
                Thread t = new Thread(new ThreadStart(this.Run));
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (InputText.Text != "")
            {
                if (!InputText.UseSystemPasswordChar)
                {
                    OutputText.AppendText(InputText.Text);
                }
                OutputText.AppendText("\r\n");                
                writer.Write((int)AdvancedClientTransmitType.StringMessage);
                writer.Write(InputText.Text);
                InputText.Text = "";
            }
        }

        public void WriteResponse(string data)
        {
            OutputText.AppendText(data);
        }

        public void SetEcho(bool enabled)
        {
            this.InputText.UseSystemPasswordChar = enabled;
        }

        delegate void WriteTextDelegate(string data);
        delegate void SetEchoDelegate(bool enabled);

        private void Run()
        {
            string name;
            string data;
            Mirage.Communication.Message msg;
            Serializer serializer = Serializer.GetSerializer(typeof(object));
            while (true)
            {
                int type = reader.ReadInt32();
                switch ((AdvancedClientTransmitType)type)
                {
                    case AdvancedClientTransmitType.StringMessage:
                        name = reader.ReadString();
                        data = reader.ReadString();
                        break;
                    case AdvancedClientTransmitType.JsonEncodedMessage:
                        name = reader.ReadString();
                        data = reader.ReadString();
                        msg =  (Mirage.Communication.Message) serializer.Deserialize(data);
                        if (msg is EchoOnMessage)
                        {
                            Invoke(new SetEchoDelegate(this.SetEcho), false);
                            data = null;
                        }
                        else if (msg is EchoOffMessage)
                        {
                            Invoke(new SetEchoDelegate(this.SetEcho), true);
                            data = null;
                        }
                        else
                        {
                            data = msg.ToString();
                        }
                        break;
                    default:
                        throw new Exception("Unrecognized response: " + type);
                }                
                if (data != null)
                    this.Invoke(new WriteTextDelegate(this.WriteResponse), data);
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = OutputText.Font;
            fontDialog1.Color = OutputText.ForeColor;
            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                fontDialog1_Apply(fontDialog1, new EventArgs());
            }
        }

        void fontDialog1_Apply(object sender, EventArgs e)
        {
            OutputText.Font = fontDialog1.Font;
            OutputText.ForeColor = fontDialog1.Color;
            InputText.Font = fontDialog1.Font;
            InputText.ForeColor = fontDialog1.Color;
        }
    }
}