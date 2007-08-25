using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mirage.Communication;
using MirageGUI.Code;
using System.Collections;
using Mirage.Data;
using MirageGUI.Forms;
using Mirage.Data.Query;
using Mirage.Command;

namespace MirageGUI.Controls
{
    public class ListTag : BaseTag
    {
        private string _getItemCommand;
        private string _getItemResponse;
        private Type _itemType;
        protected bool _lazyLoad = false;

        public ListTag(TreeNode node, string GetCommand, string GetResponse, string GetItemCommand, string GetItemResponse, Type itemType)
            : base(node)
        {
            this._getCommand = GetCommand;
            this._getResponse = GetResponse;
            this._getItemCommand = GetItemCommand;
            this._getItemResponse = GetItemResponse;
            this._itemType = itemType;
            _lazyLoad = true;
        }

        public ListTag(TreeNode node, Type itemType) :base(node) {
            this._itemType = itemType;            
            _lazyLoad = false;
        }

        private void Initialize() {
            if (Node.Nodes.Count == 0)
            {
                Node.Nodes.Add("Loading...");
                Node.TreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            }
            AddMenuItem("Add New", new EventHandler(AddNew));
        }

        void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == this.Node && !IsLoaded)
            {
                if (_lazyLoad) {
                    this.TreeHandler.NodeGet(this);
                }
                else {
                    this.Parent.Load();
                }            
            }
        }

        void AddNew(object sender, System.EventArgs e)
        {
            object o = Activator.CreateInstance(_itemType);
            string name = _itemType.Name;
            EditorForm form = this.TreeHandler.StartEdit("New " + name, o, MirageGUI.ItemEditor.EditMode.NewMode);
            form.ItemChanged += new ItemChangedHandler(ItemChanged);
        }

        protected override void ItemChanged(object sender, global::MirageGUI.Code.ItemChangedEventArgs e)
        {
            
            if (e.ChangeType == ChangeType.Add)
            {
                string name;
                if (e.Data is IUri)
                    name = ((IUri)e.Data).Uri;
                else
                    name = e.ToString();
                // add the node to our list
                TreeNode childNode = Node.Nodes.Add(name, name);
                ItemTag tag = CreateChildNodeTag(childNode);
                childNode.Tag = tag;
                tag.Data = e.Data;

                // create the child nodes
                NodeFactory factory = NodeFactory.CreateNodeFactory(this._itemType);
                factory.ConstructNodes(childNode, this.TreeHandler);

                // remove the handler we don't need it any more
                ((EditorForm)sender).ItemChanged -= new ItemChangedHandler(ItemChanged);
            }
        }
        /// <summary>
        /// Process a list from the mud.  Populate the tree.
        /// </summary>
        public override ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            if (response.MessageType == MessageType.Data)
            {
                Node.Nodes.Clear();
                NodeFactory factory = NodeFactory.CreateNodeFactory(this._itemType);
                foreach (string item in (IEnumerable)((DataMessage)response).Data)
                {
                    TreeNode childNode = Node.Nodes.Add(item, item);
                    ItemTag tag = CreateChildNodeTag(childNode);
                    childNode.Tag = tag;
                    factory.ConstructNodes(childNode, this.TreeHandler);
                }
                // remove the event we don't need it now
                Node.TreeView.BeforeExpand -= new TreeViewCancelEventHandler(TreeView_BeforeExpand);
                _loaded = true;
            }
            return ProcessStatus.SuccessAbort;
        }

        protected ItemTag CreateChildNodeTag(TreeNode node)
        {
            ItemTag tag = new ItemTag(node, _itemType, GetItemCommand, GetItemResponse);
            return tag;
        }

        public string GetItemCommand
        {
            get { return this._getItemCommand; }
            set { this._getItemCommand = value; }
        }

        public string GetItemResponse
        {
            get { return this._getItemResponse; }
            set { this._getItemResponse = value; }
        }

    }
}
