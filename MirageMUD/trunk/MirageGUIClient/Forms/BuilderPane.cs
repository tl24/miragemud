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

namespace MirageGUI.Forms
{
    public partial class BuilderPane : Form, IResponseHandler
    {
        private IOHandler _handler;
        private ConsoleForm console;
        private MessageDispatcher _dispatcher;
        private IDictionary<string, ResponseHandler> handlerDelegates;

        public BuilderPane()
        {
            InitializeComponent();
            _handler = new IOHandler();
            InitHandlerDelegates();
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

        /// <summary>
        /// Initializes a map of ResponseHandler delegates for responding to
        /// messages sent from the mud
        /// </summary>
        private void InitHandlerDelegates()
        {
            handlerDelegates = new Dictionary<string, ResponseHandler>();
            handlerDelegates.Add(new Uri(Namespaces.Area, "Area.List").ToString(), new ResponseHandler(ProcessAreaList));
            handlerDelegates.Add(new Uri(Namespaces.Area, "Area.Get").ToString(), new ResponseHandler(ProcessAreaGet));
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

            AreaTree.Nodes.Add("Areas");
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
            _handler.SendString("GetAreas");
        }

        public ConsoleForm Console
        {
            get { return console; }
        }


        #region IResponseHandler Members

        public ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            ProcessStatus result = ProcessStatus.NotProcessed;
            ResponseHandler handler;
            if (handlerDelegates.TryGetValue(response.QualifiedName, out handler))
            {
                result = handler(response);
            }
            return result;
        }

        /// <summary>
        /// Process a list of areas from the mud.  Populate the area tree.
        /// </summary>
        private ProcessStatus ProcessAreaList(Mirage.Communication.Message response)
        {
            AreaTree.Nodes.Clear();
            TreeNode root = AreaTree.Nodes.Add("Areas");
            foreach (string area in (IEnumerable)((DataMessage)response).Data)
            {
                TreeNode areaNode = root.Nodes.Add(area, area);
                areaNode.Tag = typeof(Area);
            }
            return ProcessStatus.SuccessAbort;
        }

        private ProcessStatus ProcessAreaGet(Mirage.Communication.Message response)
        {
            Area area = (Area)((DataMessage)response).Data;
            TreeNode node = AreaTree.Nodes[0].Nodes.Find(area.Uri, false)[0];
            node.Tag = area;
            AddTab(area.Title, area);
            return ProcessStatus.SuccessAbort;
        }
        #endregion

        private void AreaTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            object tag = e.Node.Tag;
            if (tag is Type)
            {
                Type t = (Type)tag;
                if (typeof(Area).IsAssignableFrom(t))
                {
                    _handler.SendString("GetArea " + e.Node.Name);
                }
            }
            else
            {
                if (tag is Area)
                {
                    AddTab(((Area)tag).Title, tag);
                }
            }
        }

        private void AddTab(string name, object data)
        {
            EditorForm form = new EditorForm(data);
            form.TopMost = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.ShowInTaskbar = false;
            form.TopLevel = false;
            TabPage page = new TabPage(name);
            page.Controls.Add(form);
            page.ContextMenuStrip.Items.Add("Close");
            EditorTabs.TabPages.Add(page);
            form.Dock = DockStyle.Fill;
            form.Visible = true;
        }

        private void BuilderPane_FormClosing(object sender, FormClosingEventArgs e)
        {
            _handler.Close();
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
    }
}