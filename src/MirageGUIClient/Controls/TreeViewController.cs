using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MirageGUI.Controls
{
    public class TreeViewController
    {
        private TreeView _treeView;
        private ITreeModel _model;
        private INodeRenderer _nodeRenderer;

        public TreeViewController(TreeView treeView, ITreeModel model) : this(treeView, model, new DefaultNodeRenderer())
        {
        }

        public TreeViewController(TreeView treeView, ITreeModel model, INodeRenderer nodeRenderer)
        {
            _treeView = treeView;
            _treeView.Tag = this;
            //_treeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(TreeView_NodeMouseClick);
            _treeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            _model = model;
            _model.StructureChanged += new EventHandler<TreeEventArgs>(Model_StructureChanged);
            _model.DataChanged += new EventHandler<TreeDataEventArgs>(Model_DataChanged);
            _nodeRenderer = nodeRenderer;
            _nodeRenderer.Controller = this;
            Initialize();
        }

        void Model_DataChanged(object sender, TreeDataEventArgs e)
        {
            if (_treeView.InvokeRequired)
            {
                _treeView.Invoke(new EventHandler<TreeDataEventArgs>(Model_DataChanged), sender, e);
                return;
            }

            TreePath path = e.Path;
            TreeNode node = FindNodeFromPath(path);
            ControllerNode cNode = null;
            if (node == null)
            {
                if (path.Count == 1)
                {
                    cNode = (ControllerNode)_treeView.Nodes[0].Tag;
                }
            }
            else
            {
                cNode = (ControllerNode) node.Tag;
            }
            if (cNode != null)
            {
                cNode.Invalidate(e.IsRecursive, e.Data);
            }
        }

        void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            ControllerNode ctrlNode = node.Tag as ControllerNode;
            if (ctrlNode != null)
            {
                // see if this works ok, might conflict with the event allready in progress
                ctrlNode.Expand();
            }
        }


        void Model_StructureChanged(object sender, TreeEventArgs e)
        {
            if (_treeView.InvokeRequired)
            {
                _treeView.Invoke(new EventHandler<TreeEventArgs>(Model_StructureChanged), sender, e);
                return;
            }

            TreePath path = e.Path;
            TreeNode node = FindNodeFromPath(path);
            TreeNodeCollection nodes;
            if (node == null)
                nodes = _treeView.Nodes;
            else
                nodes = node.Nodes;

            nodes.Clear();
            EnumerateNodes(nodes, path);
        }

        private void Initialize()
        {
            EnumerateNodes(_treeView.Nodes, TreePath.EmptyPath);
        }

        private TreeNode FindNodeFromPath(TreePath path)
        {
            if (path.IsEmpty)
                return null;
            else
                return FindNodeFromPath(_treeView.Nodes, path, 0);
        }

        private TreeNode FindNodeFromPath(TreeNodeCollection nodes, TreePath path, int level)
        {
            foreach (TreeNode node in nodes)
            {
                ControllerNode tag = node.Tag as ControllerNode;
                if (tag != null)
                {
                    if (tag.Path.Equals(path) || tag.Data == path.FullPath[level] || tag.Data.ToString() == path.FullPath[level].ToString())
                    {
                        if (path.Count - 1 == level)
                            return node;
                        else
                        {
                            TreeNode result = FindNodeFromPath(node.Nodes, path, level + 1);
                            if (result == null)
                                return node;
                            else
                                return result;
                        }
                    }
                }
            }
            return null;
        }

        private void EnumerateNodes(TreeNodeCollection nodes, TreePath path)
        {
            nodes.Clear();
            foreach (object item in _model.GetChildren(path))
            {
                TreePath itemPath = path.Append(item);
                int count = _model.GetChildCount(itemPath);
                TreeNode node = new TreeNode();
                nodes.Add(node);
                node.Tag = new ControllerNode(this, item, node, itemPath, count);
            }
        }

        public object GetNodeData(TreeNode node)
        {
            if (node.Tag is ControllerNode)
                return ((ControllerNode)node.Tag).Data;
            else
                return null;
        }

        public TreePath GetPathFromNode(TreeNode node)
        {
            if (node.Tag is ControllerNode)
                return ((ControllerNode)node.Tag).Path;
            else
                return null;
        }

        private class ControllerNode
        {
            private TreeViewController _owner;
            private object _data;
            private TreeNode _node;
            private bool _hasBeenExpanded;
            private TreePath _path;
            private int _childCount;
            private static object markerNode = new object();

            public ControllerNode(TreeViewController owner, object data, TreeNode node, TreePath path, int childCount)
            {
                _owner = owner;
                this._data = data;
                this._node = node;
                this._path = path;
                this._childCount = childCount;
                if (_childCount == -1 || _childCount > 0)
                {
                    TreeNode _childNode = new TreeNode();
                    _childNode.Tag = markerNode;
                    node.Nodes.Add(_childNode);
                }
                owner._nodeRenderer.Render(node, data);
            }

            public void Invalidate(bool InvalidateChildren)
            {
                Invalidate(InvalidateChildren, Data);
            }

            public void Invalidate(bool InvalidateChildren, object data)
            {
                // just redraw everything for right now
                this._data = data;
                _owner._nodeRenderer.Render(_node, _data);
                if (InvalidateChildren)
                {
                    foreach (TreeNode childNode in _node.Nodes)
                    {
                        if (childNode.Tag is ControllerNode)
                            ((ControllerNode)childNode.Tag).Invalidate(true);
                    }
                }
            }

            public object Data
            {
                get { return this._data; }
            }

            public System.Windows.Forms.TreeNode Node
            {
                get { return this._node; }
            }

            public bool IsExpanded
            {
                get { return this._node.IsExpanded; }
            }

            public bool HasBeenExpanded
            {
                get { return _hasBeenExpanded; }
                set { _hasBeenExpanded = value; }
            }

            public void Expand()
            {
                if (!_hasBeenExpanded)
                {
                    Node.Nodes.Clear();
                    _owner.EnumerateNodes(Node.Nodes, _path);                    
                }
                //Node.Expand();
                _hasBeenExpanded = true;
            }

            public TreePath Path
            {
                get { return _path; }
            }
        }
    }
}
