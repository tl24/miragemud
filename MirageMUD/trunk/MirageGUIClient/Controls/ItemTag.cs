using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MirageGUI.ItemEditor;
using MirageGUI.Code;
using Mirage.Communication;

namespace MirageGUI.Controls
{
    public class ItemTag : BaseTag
    {
        private Type _itemType;

        public ItemTag(TreeNode node, Type itemType, string GetCommand, string GetResponse)
            : base(node)
        {
            this._itemType = itemType;
            this._getCommand = GetCommand;
            this._getResponse = GetResponse;
        }

        public Type ItemType
        {
            get { return this._itemType; }
            set { this._itemType = value; }
        }

        public override void OnNodeClick(TreeNodeMouseClickEventArgs e)
        {
            if (IsLoaded)
                TreeHandler.StartEdit(Node.FullPath, Data, EditMode.EditMode);
            else
                TreeHandler.NodeGet(this);
        }

        /// <summary>
        /// Process a list from the mud.  Populate the tree.
        /// </summary>
        public override ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            if (response.MessageType == MessageType.Data)
            {
                this._data = ((DataMessage)response).Data;
                TreeHandler.StartEdit(Node.FullPath, Data, EditMode.EditMode);
                _loaded = true;
            }
            return ProcessStatus.SuccessAbort;
        }
    }
}
