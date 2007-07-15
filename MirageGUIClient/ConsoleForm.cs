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
    public partial class ConsoleForm : Form, IResponseHandler
    {
        private IOHandler _handler;

        public ConsoleForm(IOHandler handler)
        {
            InitializeComponent();
            this._handler = handler;
            this._handler.ConnectStateChanged += new IOHandler.ConnectStateChangedHandler(handler_ConnectStateChanged);
            SendButton.Enabled = _handler.IsConnected;
        }

        void handler_ConnectStateChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IOHandler.ConnectStateChangedHandler(handler_ConnectStateChanged), sender, e);
                return;
            }
            if (_handler.IsConnected)
            {
                SendButton.Enabled = true;
            }
            else
            {
                SendButton.Enabled = false;
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
                _handler.SendString(InputText.Text);
                InputText.Text = "";
            }
        }

        public void HandleResponse(MudResponse response)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ResponseHandler(HandleResponse), response);
                return;
            }

            if (response.Type == AdvancedClientTransmitType.JsonEncodedMessage)
            {
                Mirage.Communication.Message msg = (Mirage.Communication.Message)response.Data;
                if (msg is EchoOnMessage)
                {
                    this.InputText.UseSystemPasswordChar = false;
                }
                else if (msg is EchoOffMessage)
                {
                    this.InputText.UseSystemPasswordChar = true;
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
            MirageGUIClient.Default.Save();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = OutputText.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                OutputText.BackColor = colorDialog1.Color;
                InputText.BackColor = colorDialog1.Color;
                MirageGUIClient.Default.Save();
            }
        }
    }
}