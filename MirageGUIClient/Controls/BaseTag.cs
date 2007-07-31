using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Code;

namespace MirageGUI.Controls
{
    public abstract class BaseTag : IResponseHandler
    {
        protected bool _loaded;
        protected object _data;
        protected string _getCommand;
        protected string _getResponse;
        protected TreeNode _node;

        public BaseTag(TreeNode node)
        {
            this._node = node;
        }

        public TreeViewHandler TreeHandler
        {
            get { return (TreeViewHandler) this.Node.TreeView.Tag; }
        }

        public TreeNode Node
        {
            get { return _node; }
        }

        #region ITreeViewTagData Members

        public virtual bool IsLoaded
        {
            get { return _loaded; }
        }

        public virtual object Data
        {
            get { return _data; }
        }

        public virtual string GetCommand
        {
            get { return _getCommand; }
        }

        public virtual string GetResponse
        {
            get { return _getResponse; }
        }

        public abstract ProcessStatus HandleResponse(Mirage.Communication.Message message);

        public virtual void OnNodeClick(TreeNodeMouseClickEventArgs e)
        {
        }
        #endregion

    }
}
