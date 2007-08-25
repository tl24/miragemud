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
        private ContextMenuStrip _menuStrip;

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
            set
            {
                _data = value;
                _loaded = value != null;
            }
        }

        public virtual string GetCommand
        {
            get { return _getCommand; }
        }

        public virtual string GetResponse
        {
            get { return _getResponse; }
        }

        public virtual void Load()
        {
            if (_loaded)
                this.TreeHandler.NodeGet(this);
        }

        public virtual BaseTag Parent
        {
            get
            {
                if (_node != null && _node.Parent != null && _node.Parent.Tag is BaseTag)
                {
                    return (BaseTag)_node.Parent.Tag;
                }
                else
                {
                    return null;
                }
            }
        }
        public abstract ProcessStatus HandleResponse(Mirage.Communication.Message message);

        public virtual void OnNodeClick(TreeNodeMouseClickEventArgs e)
        {
        }

        /// <summary>
        /// Returns the context menustrip for this node
        /// </summary>
        protected ContextMenuStrip MenuStrip
        {
            get
            {
                return _menuStrip;
            }
        }

        protected void CreateMenu() {
            this._menuStrip = new System.Windows.Forms.ContextMenuStrip();
            //this._menuStrip.Size = new System.Drawing.Size(153, 70);
            this.Node.ContextMenuStrip = _menuStrip;
        }

        protected ToolStripItem AddMenuItem(string Name, System.EventHandler handler)
        {
            if (MenuStrip == null)
                CreateMenu();


            
            ToolStripItem menuItem = MenuStrip.Items.Add(Name, null, handler);
            /*
            menuItem.Name = "MenuItem" + MenuStrip.Items.Count;
            menuItem.Size = new System.Drawing.Size(132, 22);
            menuItem.Text = Name;
            if (handler != null)
                menuItem.Click += handler;

            MenuStrip.Items.Add(menuItem);
             */
            return menuItem;
        }

        protected virtual void ItemChanged(object sender, global::MirageGUI.Code.ItemChangedEventArgs e)
        {
        }
        #endregion

    }
}
