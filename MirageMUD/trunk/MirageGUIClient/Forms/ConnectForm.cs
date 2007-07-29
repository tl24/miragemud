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
            if (AppSettings.Default.RememberPassword)
                Password.Text = AppSettings.Default.Password;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.handler.Connect(RemoteHost.Text, int.Parse(RemotePort.Text));
                AppSettings.Default.Save();
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

        public ProcessStatus HandleResponse(Mirage.Communication.Message msg)
        {
            if (msg.IsMatch(Namespaces.Authentication))
            {
                if (msg.MessageType == MessageType.PlayerError || msg.MessageType == MessageType.SystemError)
                {
                    StringMessage emsg = (StringMessage)msg;
                    MessageBox.Show(emsg.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoginMode();
                    return ProcessStatus.SuccessAbort;
                }
                else
                {
                    if (msg.IsMatch(Namespaces.Authentication, "Nanny.Challenge"))
                    {
                        DoLogin();
                        return ProcessStatus.SuccessAbort;
                    }
                    else if (msg.IsMatch(Namespaces.Authentication, "Login"))
                    {
                        // success
                        handler.OnLogin();
                        if (RememberPassword.Checked)
                        {
                            AppSettings.Default.Password = Password.Text;
                        }
                        else
                        {
                            AppSettings.Default.Password = "";
                        }
                        AppSettings.Default.Save();
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