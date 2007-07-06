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

        delegate void ResponseHandler(MudResponse response);

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

        public void HandleResponse(MudResponse response)
        {
            if (response.Type == AdvancedClientTransmitType.JsonEncodedMessage)
            {
                Mirage.Communication.Message msg = (Mirage.Communication.Message)response.Data;
                if (msg is EchoOnMessage)
                {
                    this.InputText.UseSystemPasswordChar = false;
                }
                else if (msg is EchoOffMessage)
                {
                    this.InputText.UseSystemPasswordChar = false;
                }
                else
                {
                    OutputText.AppendText(msg.ToString());
                }
            }
            else
            {
                OutputText.AppendText((string)response.Data);
            }

        }

        private void Run()
        {
            string name;
            object data;
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
                        data =  (Mirage.Communication.Message) serializer.Deserialize((string) data);
                        break;
                    default:
                        throw new Exception("Unrecognized response: " + type);
                }
                this.Invoke(new ResponseHandler(this.HandleResponse), new MudResponse((AdvancedClientTransmitType) type, name, data));
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

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = OutputText.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                OutputText.BackColor = colorDialog1.Color;
                InputText.BackColor = colorDialog1.Color;
            }
        }
    }
}