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
using System.Threading;
using JsonExSerializer;
using Mirage.Game.Communication;
using MirageGUI.Code;
using log4net.Appender;
using log4net.Repository;
using Mirage.Core.Messaging;

namespace MirageGUI.Forms
{
    public partial class ConsoleForm : Form, IResponseHandler, IAppender
    {
        private IOHandler _handler;
        private string[] _commandHistory = new string[20];
        private int _currentHistoryIndex = -1;
        private int _historyOffset = 0;
        public ConsoleForm(IOHandler handler)
        {
            InitializeComponent();
            this._handler = handler;
            this._handler.ConnectStateChanged += new EventHandler(handler_ConnectStateChanged);
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
                    _commandHistory[++_currentHistoryIndex % _commandHistory.Length] = InputText.Text;
                }
                OutputText.AppendText("\r\n");
                _handler.SendString(InputText.Text);
                InputText.Text = "";
            }
        }

        public ProcessStatus HandleResponse(Mirage.Core.Messaging.Message msg)
        {
            if (this.InvokeRequired)
            {
                return (ProcessStatus) this.Invoke(new ResponseHandler(HandleResponse), msg);
            }

            if (msg.IsMatch(Namespaces.System, "EchoOn"))
            {
                this.InputText.UseSystemPasswordChar = false;
            }
            else if (msg.IsMatch(Namespaces.System, "EchoOff"))
            {
                this.InputText.UseSystemPasswordChar = true;
            }
            else if (msg.IsMatch(MessageType.SystemError))
            {
                string text;
                if (msg is ExceptionMessage)
                {
                    ExceptionMessage excMsg = (ExceptionMessage)msg;
                    text = excMsg.Text;
                    text += "\r\n" + excMsg.Inner.StackTrace;
                }
                else
                {
                    text = msg.ToString();
                }
                MessageBox.Show(text, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (msg.IsMatch(MessageType.Confirmation) && msg.ToString() == string.Empty)
            {
                MessageBox.Show(msg.Name.Name, "Confirmation", MessageBoxButtons.OK);
            }
            else
            {
                OutputText.AppendText(msg.ToString());
            }
            return ProcessStatus.SuccessAbort;
        }

        internal void ShowFont(object sender, EventArgs e)
        {
            fontDialog1.Font = OutputText.Font;
            fontDialog1.Color = OutputText.ForeColor;
            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                ApplyFont(fontDialog1, new EventArgs());
            }
        }

        void ApplyFont(object sender, EventArgs e)
        {
            OutputText.Font = fontDialog1.Font;
            OutputText.ForeColor = fontDialog1.Color;
            InputText.Font = fontDialog1.Font;
            InputText.ForeColor = fontDialog1.Color;
            AppSettings.Default.Save();
        }

        internal void ShowBackgroundColor(object sender, EventArgs e)
        {
            colorDialog1.Color = OutputText.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                OutputText.BackColor = colorDialog1.Color;
                InputText.BackColor = colorDialog1.Color;
                AppSettings.Default.Save();
            }
        }

        #region IAppender Members

        private delegate void AppendTextHandler(string text);
        private void AppendText(string text)
        {
            OutputText.AppendText(text);
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            string text = loggingEvent.RenderedMessage;
            text += "\r\n";

            if (loggingEvent.ExceptionObject != null) 
                text += loggingEvent.ExceptionObject.StackTrace;

            if (this.InvokeRequired)
                this.Invoke(new AppendTextHandler(AppendText), text);
        }

        #endregion

        private void ConsoleForm_Load(object sender, EventArgs e)
        {

            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetLoggerRepository()).Root.AddAppender(this);
        }

        private bool IsHistoryKey = false;
        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (_historyOffset < _commandHistory.Length && _currentHistoryIndex >= 0 && _historyOffset <= _currentHistoryIndex)
                {
                    IsHistoryKey = true;
                    InputText.Text = _commandHistory[(_currentHistoryIndex - _historyOffset) % _commandHistory.Length];
                    IsHistoryKey = false;
                    _historyOffset++;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_historyOffset > 0)
                {
                    _historyOffset--;
                    IsHistoryKey = true;
                    InputText.Text = _commandHistory[(_currentHistoryIndex - _historyOffset) % _commandHistory.Length];
                    IsHistoryKey = false;
                }
            }
        }

        private void InputText_TextChanged(object sender, EventArgs e)
        {
            if (IsHistoryKey)
                return;
            if (InputText.Text != "")
                _historyOffset = 0;
        }
    }
}