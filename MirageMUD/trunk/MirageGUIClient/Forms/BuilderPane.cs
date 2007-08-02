using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mirage.Communication.BuilderMessages;
using Mirage.Data;
using MirageGUI.Code;
using Mirage.Communication;
using System.Collections;
using MirageGUI.ItemEditor;
using Mirage.Data.Query;
using MirageGUI.Controls;

namespace MirageGUI.Forms
{
    public partial class BuilderPane : Form, IResponseHandler
    {
        private IOHandler _handler;
        private ConsoleForm console;
        private MessageDispatcher _dispatcher;
        private IDictionary<string, ResponseHandler> handlerDelegates;
        private TreeViewHandler _treeHandler;

        public BuilderPane()
        {
            InitializeComponent();
            _handler = new IOHandler();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectForm form = new ConnectForm(_handler);
            _dispatcher.AddHandler(FormPriority.ConnectFormPriority, form);
            form.ShowDialog(this);
            _dispatcher.RemoveHandler(form);
        }

        public IOHandler IOHandler
        {
            get { return _handler; }
        }



        private void BuilderPane_Load(object sender, EventArgs e)
        {
            // construct the console
            console = new ConsoleForm(this._handler);
            console.TopMost = false;
            console.FormBorderStyle = FormBorderStyle.None;
            console.ShowInTaskbar = false;
            console.TopLevel = false;
            OuterSplit.Panel2.Controls.Add(console);
            console.Dock = DockStyle.Fill;
            console.Visible = true;

            _handler.ConnectStateChanged += new EventHandler(_handler_ConnectStateChanged);
            _handler.LoginSuccess += new EventHandler(LoginSuccess);

            // create the message dispatcher
            _dispatcher = new MessageDispatcher(IOHandler);
            _dispatcher.AddHandler(FormPriority.MasterFormPriority, this);
            _dispatcher.AddHandler(FormPriority.ConsoleFormPriority, this.Console);

            _treeHandler = new TreeViewHandler(AreaTree, this, IOHandler, _dispatcher);            
        }

        void _handler_ConnectStateChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IOHandler.ConnectStateChangedHandler(_handler_ConnectStateChanged), sender, e);
                return;
            }
            ConnectedLabel.Text = _handler.IsConnected ? "Connected " + _handler.Host + "@" + _handler.Port : "Not Connected";
            if (!_handler.IsConnected)
                connectMenuItem.Enabled = true;
            disconnectMenuItem.Enabled = _handler.IsConnected;
        }

        void LoginSuccess(object sender, EventArgs e)
        {
            connectMenuItem.Enabled = false;
            _treeHandler.Fill();
            
        }

        public ConsoleForm Console
        {
            get { return console; }
        }


        public ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            return ProcessStatus.NotProcessed;
        }


        public EditorForm AddTab(string name, object data, EditMode Mode)
        {
            EditorForm form = new EditorForm(data, Mode, IOHandler);
            form.TopMost = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.ShowInTaskbar = false;
            form.TopLevel = false;
            TabPage page = new TabPage(name);
            page.Controls.Add(form);
            EditorTabs.TabPages.Add(page);
            form.FormClosing += new FormClosingEventHandler(EditorForm_FormClosing);
            form.ItemChanged += new ItemChangedHandler(EditorForm_ItemChanged);
            form.Dock = DockStyle.Fill;
            form.Visible = true;
            return form;

        }

        void EditorForm_ItemChanged(object sender, global::MirageGUI.Code.ItemChangedEventArgs e)
        {
            EditorForm form = (EditorForm)sender;
            TabPage page = (TabPage)form.Parent;
            if (e.Data is IUri)
            {
                page.Name = ((IUri)e.Data).Uri;
            }
        }

        void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            EditorTabs.TabPages.Remove((TabPage) ((Form)sender).Parent);
        }

        
        private void BuilderPane_FormClosing(object sender, FormClosingEventArgs e)
        {
            _handler.Close();
            AppSettings.Default.Save();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.ShowFont(sender, e);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.ShowBackgroundColor(sender, e);
        }

        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to disconnect?", "Disconnect", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                IOHandler.SendString("quit");
            }
        }

        private void newAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTab("New Area", new Area(), EditMode.NewMode);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IOHandler.SendString("SaveArea all");
        }
    }
}