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

namespace MirageGUI.Forms
{
    public partial class BuilderPane : Form, IResponseHandler
    {
        private IOHandler _handler;
        private ConsoleForm console;
        private MessageDispatcher _dispatcher;

        public BuilderPane()
        {
            InitializeComponent();
            _handler = new IOHandler();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectForm form = new ConnectForm(_handler);
            form.ShowDialog(this);
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

            _handler.ConnectStateChanged += new IOHandler.ConnectStateChangedHandler(_handler_ConnectStateChanged);
            _dispatcher = new MessageDispatcher(this);

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
            //if (_handler.IsConnected)
            //{
            //    _handler.SendString("GetAreas");
            //}
        }

        public ConsoleForm Console
        {
            get { return console; }
        }


        #region IResponseHandler Members

        public void HandleResponse(MudResponse response)
        {
            if (response.Name == "Area.List")
            {
                AreaTree.Nodes.Clear();
                TreeNode root = AreaTree.Nodes.Add("Areas");
                AreaListMessage areaList = (AreaListMessage)response.Data;
                foreach (string area in areaList.Areas)
                {
                    TreeNode areaNode = root.Nodes.Add(area,area);
                    areaNode.Tag = typeof(Area);
                }
            }
            else if (response.Name == "Area.Get")
            {
                AreaMessage message = (AreaMessage)response.Data;
                Area area = message.Area;
                TreeNode node = AreaTree.Nodes[0].Nodes.Find(area.Uri, false)[0];
                node.Tag = area;
                AddTab(area.Title, area);
            }
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
            EditorTabs.TabPages.Add(page);
            form.Dock = DockStyle.Fill;
            form.Visible = true;
        }
    }
}