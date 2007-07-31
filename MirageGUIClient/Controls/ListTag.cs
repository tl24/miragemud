using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mirage.Communication;
using MirageGUI.Code;
using System.Collections;
using Mirage.Data;

namespace MirageGUI.Controls
{
    public class ListTag : BaseTag
    {
        private string _getItemCommand;
        private string _getItemResponse;
        private Type _itemType;

        public ListTag(TreeNode node, string GetCommand, string GetResponse, string GetItemCommand, string GetItemResponse, Type itemType)
            : base(node)
        {
            this._getCommand = GetCommand;
            this._getResponse = GetResponse;
            this._getItemCommand = GetItemCommand;
            this._getItemResponse = GetItemResponse;
            this._itemType = itemType;

            if (node.Nodes.Count == 0)
            {
                node.Nodes.Add("Loading...");
                node.TreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            }
        }

        void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == this.Node && !IsLoaded)
            {
                this.TreeHandler.NodeGet(this);
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
                foreach (string item in (IEnumerable)((DataMessage)response).Data)
                {
                    TreeNode childNode = Node.Nodes.Add(item, item);
                    ItemTag tag = CreateChildNodeTag(childNode);
                    childNode.Tag = tag;
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
