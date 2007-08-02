using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Code;
using Mirage.Communication;
using System.Reflection;
using Mirage.Data;
using MirageGUI.ItemEditor;
using System.IO;
using MirageGUI.Forms;
using System.ComponentModel;

namespace MirageGUI.Controls
{
    /// <summary>
    /// Class to handle loading data and events on the tree view
    /// </summary>
    public class TreeViewHandler : IResponseHandler
    {
        private TreeView tree;
        private IOHandler ioHandler;
        private IDictionary<string, string> _responseTypes;
        private MessageDispatcher dispatcher;
        private BuilderPane builder;
        public TreeViewHandler(TreeView tree, BuilderPane builder, IOHandler IOHandler, MessageDispatcher dispatcher)
        {
            this.builder = builder;
            this.tree = tree;
            tree.PathSeparator = "/";
            tree.Tag = this;
            this.ioHandler = IOHandler;
            tree.Nodes.Add("Areas");
            _responseTypes = new Dictionary<string, string>();
            this.tree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(NodeMouseDoubleClick);
            dispatcher.AddHandler(FormPriority.MasterFormPriority, this);
        }

        public TreeView Tree
        {
            get { return tree; }
        }

        public void Fill()
        {
            ioHandler.SendString("GetWorld");
        }

        public System.ComponentModel.IContainer Container
        {
            get
            {
                return this.builder.Container;
            }
        }
        public void RegisterCommand(string command, string response)
        {
            _responseTypes[command] = response;
        }

        public void NodeGet(BaseTag tagData)
        {
            string cmd = tagData.GetCommand;
            string uri = TreePathToItemUri(tagData.Node.FullPath);
            ioHandler.SendString(string.Format("{0} {1}", cmd, uri));
        }

        public ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            if (this.tree.InvokeRequired)
                return (ProcessStatus) this.tree.Invoke(new ResponseHandler(HandleResponse), response);

            ProcessStatus result = ProcessStatus.NotProcessed;
            if (response.IsMatch(Namespaces.Area, "World"))
            {
                result = CreateWorld((DataMessage)response);
            }
            else if (_responseTypes.ContainsKey(response.QualifiedName.ToString()))
            {
                DataMessage dm = response as DataMessage;
                string itemUri = dm.ItemUri;
                string treePath = ItemUriToTreePath(itemUri);
                // find the node
                TreeNode tNode = FindNode(treePath);
                if (tNode.Tag is BaseTag)
                    return ((BaseTag)tNode.Tag).HandleResponse(response);
            }
            return ProcessStatus.NotProcessed;
        }

        private ProcessStatus CreateWorld(DataMessage dataMessage)
        {
            tree.Nodes.Clear();
            TreeNode WorldNode = tree.Nodes.Add("World", "World");
            NodeFactory factory = NodeFactory.CreateNodeFactory(dataMessage.Data.GetType());
            factory.ConstructNodes(WorldNode, this);
            return ProcessStatus.SuccessAbort;
        }

        /// <summary>
        /// Finds a node with the given path from the root of the tree
        /// </summary>
        /// <param name="TreePath">path to the node</param>
        /// <returns>the node if found or null</returns>
        public TreeNode FindNode(string TreePath)
        {
            return FindNode(TreePath, this.tree.Nodes);
        }

        /// <summary>
        /// Finds a node in the tree node collection with the given path
        /// </summary>
        /// <param name="TreePath">path to the node</param>
        /// <param name="TreeCol"></param>
        /// <returns></returns>
        public TreeNode FindNode(string TreePath, TreeNodeCollection TreeCol)
        {
            string[] PTms = TreePath.Split(new string[] { this.tree.PathSeparator } , StringSplitOptions.None);

            for (int k = 0; k < TreeCol.Count; k++)
            {
                if (TreeCol[k].Name == PTms[0])
                {
                    if (TreeCol[k].Nodes.Count == 0 || PTms.Length == 1)
                        return TreeCol[k];

                    return FindNode(TreePath.Remove(0, PTms[0].Length + 1), TreeCol[k].Nodes);
                }
            }
            return null;
        }
        /// <summary>
        /// Converts an item uri returned from the server into a path in the tree
        /// </summary>
        /// <param name="ItemUri">the item uri</param>
        /// <returns>path to item in the tree</returns>
        public string ItemUriToTreePath(string ItemUri)
        {
            return "World/" + ItemUri;
        }

        /// <summary>
        /// Converts a tree node path into an item uri on the server
        /// </summary>
        /// <param name="TreePath">path to item in the tree</param>
        /// <returns>the item uri</returns>
        public string TreePathToItemUri(string TreePath)
        {
            if (TreePath.StartsWith("World"))
                TreePath = TreePath.Substring(TreePath.IndexOf(this.tree.PathSeparator) + 1);
            return TreePath;
        }

        public EditorForm StartEdit(string itemPath, object data, EditMode mode)
        {
            EditorForm form = builder.AddTab(itemPath, data, mode);
            return form;
        }

        /// <summary>
        /// Event handler for double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {            
            object tag = e.Node.Tag;
            if (tag is BaseTag)
            {
                BaseTag baseTag = tag as BaseTag;
                baseTag.OnNodeClick(e);
            }
        }

    }
}
