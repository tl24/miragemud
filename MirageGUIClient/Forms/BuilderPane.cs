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
            handlerDelegates.Add(new Uri(Namespaces.Area, "AreaList").ToString(), new ResponseHandler(ProcessAreaList));
            handlerDelegates.Add(new Uri(Namespaces.Area, "Area").ToString(), new ResponseHandler(ProcessAreaGet));
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
            root.ContextMenuStrip = AreasContextMenu;

            foreach (string area in (IEnumerable)((ChildItemsMessage)response).Items)
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
            AddTab(area.Title, area, EditMode.EditMode);
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
                    AddTab(((Area)tag).Title, tag, EditMode.EditMode);
                }
            }
        }

        private void AddTab(string name, object data, EditMode Mode)
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

        }

        void EditorForm_ItemChanged(object sender, global::MirageGUI.Code.ItemChangedEventArgs e)
        {
            EditorForm form = (EditorForm)sender;
            TabPage page = (TabPage) form.Parent;
            if (e.Data is IUri)
            {
                page.Name = ((IUri)e.Data).Uri;
            }
            if (e.Data is Area)
            {
                Area area = (Area)e.Data;
                TreeNode AreasNode = AreaTree.Nodes[0];
                if (e.ChangeType == ChangeType.ItemAdded)
                {
                    TreeNode areaNode = AreasNode.Nodes.Add(area.Uri, area.Uri);
                    areaNode.Tag = area;
                }
                else
                {
                    TreeNode[] nodes = AreasNode.Nodes.Find(area.Uri, false);
                    if (nodes.Length == 1)
                    {
                        nodes[0].Tag = area;
                    }
                    else
                    {
                        throw new ApplicationException("Duplicate area found: " + area.Uri);
                    }
                }
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