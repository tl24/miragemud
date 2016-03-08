using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using MirageGUI.Code;
using log4net;
using MirageGUI.ItemEditor;
using Mirage.Game.Command;
using Mirage.Game.Communication.BuilderMessages;

namespace MirageGUI.Controls
{
    /// <summary>
    /// Base item for data displayed by the tree
    /// </summary>
    public abstract class BaseItem : ITreeModel
    {
        protected static ILog logger = LogManager.GetLogger(typeof(BaseItem));
        private BaseItem _parent;
        private int _level;
        protected object _data;
        protected bool _isDirty;

        public BaseItem(BaseItem parent)
        {
            _parent = parent;
            if (_parent != null)
                _level = _parent.Level + 1;
        }

        /// <summary>
        /// The data object of this node
        /// </summary>
        public virtual object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #region ITreeModel Members

        public abstract IEnumerable GetChildren(TreePath path);

        public abstract int GetChildCount(TreePath path);

        public abstract void UpdateChild(BaseItem child, object data, ChangeType changeType);

        public event EventHandler<TreeEventArgs> StructureChanged
        {
            // just assign the events to the top model
            add { FindModel().StructureChanged += value; }
            remove { FindModel().StructureChanged -= value; }
        }

        protected void OnStructureChanged()
        {
            OnStructureChanged(CreatePath());
        }

        protected void OnStructureChanged(TreePath path)
        {
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat("firing StructureChanged Event ({0}) {1}, path: {2}", this.GetType(), this, path);
            }
            FindModel().OnStructureChanged(path);
        }

        public event EventHandler<TreeDataEventArgs> DataChanged
        {
            add { FindModel().DataChanged += value; }
            remove { FindModel().DataChanged -= value; }
        }

        protected void OnDataChanged(bool isRecursive)
        {
            OnDataChanged(CreatePath(), isRecursive);
        }

        protected void OnDataChanged(TreePath path, bool isRecursive)
        {
            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat("firing OnDataChanged Event ({0}) {1}, path: {2}, recursive: {3}", this.GetType(), this, path, isRecursive);
            }
            FindModel().OnDataChanged(path, isRecursive, this);
        }

        public bool IsPath(TreePath path)
        {
            if (path.LastNode == this && path.Count == Level + 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a tree path from this item and its ancestors
        /// </summary>
        /// <returns>the tree path</returns>
        public TreePath CreatePath()
        {
            if (_parent == null)
                return new TreePath(this);
            else
                return new TreePath(_parent.CreatePath(), this);
        }

        public virtual AreaTreeModel FindModel()
        {
            if (this is RootItem)
                return ((RootItem)this).Model;
            else if (this._parent != null)
                return this._parent.FindModel();
            else
                return null;
        }
        /// <summary>
        /// Indicates the depth of this item in the tree
        /// </summary>
        public virtual int Level
        {
            get { return _level; }
        }

        public virtual ProcessStatus HandleResponse(Mirage.Core.Messaging.Message response)
        {
            return ProcessStatus.NotProcessed;
        }

        public BaseItem Parent
        {
            get { return _parent; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
        }

        /// <summary>
        /// Sets a flag indicating that data has changed but not been committed yet
        /// </summary>
        public virtual void SetDirty()
        {
            _isDirty = true;
            OnDataChanged(false);
            if (Parent != null)
                Parent.SetDirty();
        }

        public virtual void ClearDirty()
        {
            ClearDirty(true);
        }

        public virtual void ClearDirty(bool sendEvent)
        {
            if (_isDirty != false)
            {
                _isDirty = false;
                if (sendEvent)
                {
                    OnDataChanged(true);
                }
            }
        }
        #endregion
    }
}
