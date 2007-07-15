using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MirageGUIClient
{
    public partial class ConnectForm : Form
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
                MirageGUIClient.Default.Save();
                this.Close();
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
    }
}