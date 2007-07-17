using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Code;
using Mirage.Communication;
using Mirage.Communication.BuilderMessages;
namespace MirageGUI.Forms
{
    public partial class ConnectForm : Form, IResponseHandler
    {
        private IOHandler handler;

        public ConnectForm(IOHandler handler)
        {
            this.handler = handler;
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.handler.Connect(RemoteHost.Text, int.Parse(RemotePort.Text));
                MirageGUI.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void Login_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            LoginMessage lmsg = new LoginMessage();
            lmsg.Login = Login.Text;
            lmsg.Password = Password.Text;
            handler.SendObject(lmsg.Name, lmsg);
        }

        #region IResponseHandler Members

        public ProcessStatus HandleResponse(MudResponse response)
        {
            if (response.Data is Mirage.Communication.Message)
            {
                if (response.Data is ErrorMessage)
                {
                    ErrorMessage emsg = (ErrorMessage)response.Data;
                    MessageBox.Show(emsg.MessageString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoginMode();
                    return ProcessStatus.SuccessAbort;
                }
                else
                {
                    Mirage.Communication.Message msg = (Mirage.Communication.Message) response.Data;
                    if (msg.MessageType == MessageType.Prompt && msg.Name == "Nanny.Challenge")
                    {
                        DoLogin();
                        return ProcessStatus.SuccessAbort;
                    }
                    else if (msg.MessageType == MessageType.Confirmation && msg.Name == "Login")
                    {
                        // success
                        handler.OnLogin();
                        // we're done, close the form
                        this.Close();
                        return ProcessStatus.SuccessAbort;
                    }
                }
            }
            return ProcessStatus.NotProcessed;
        }

        #endregion

        private void LoginMode()
        {
            if (RemoteHost.Enabled)
            {
                ConnectButton.Click -= new EventHandler(ConnectButton_Click);
                ConnectButton.Click += new EventHandler(Login_Click);
                ConnectButton.Text = "Login";
                RemoteHost.Enabled = false;
                RemotePort.Enabled = false;
            }
            Login.Focus();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (handler.IsConnected)
            {
                // if we call cancel and we're connected it means we're not logged in
                // so we should disconnect
                handler.Close();
            }
            this.Close();
        }
    }
}