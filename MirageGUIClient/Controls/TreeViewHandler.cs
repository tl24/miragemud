using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Code;
using Mirage.Communication;
using System.Reflection;
using Mirage.Data;
using MirageGUI.ItemEditor;

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

        public TreeViewHandler(TreeView tree, IOHandler IOHandler, MessageDispatcher dispatcher)
        {
            this.tree = tree;
            tree.Tag = this;
            this.ioHandler = IOHandler;
            tree.Nodes.Add("Areas");
            _responseTypes = new Dictionary<string, string>();
            this.tree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(NodeMouseDoubleClick);
            dispatcher.AddHandler(FormPriority.MasterFormPriority, this);
        }

        public void Fill()
        {
            ioHandler.SendString("GetWorld");
        }

        public void RegisterCommand(string command, string response)
        {
            _responseTypes[command] = response;
        }

        public void NodeGet(BaseTag tagData)
        {
            string cmd = tagData.GetCommand;
            string path = tagData.Node.FullPath;
            ioHandler.SendString(string.Format("{0} {1}", cmd, path));
        }

        public ProcessStatus HandleResponse(Mirage.Communication.Message response)
        {
            if (this.tree.InvokeRequired)
                return (ProcessStatus) this.tree.Invoke(new ResponseHandler(HandleResponse), response);

            ProcessStatus result = ProcessStatus.NotProcessed;
            if (response.IsMatch(Namespaces.Area, "World"))
            {
                DataMessage dm = response as DataMessage;
                ProcessAttributes(dm.Data.GetType(), null);
                result = ProcessStatus.SuccessAbort;
            }
            else if (_responseTypes.ContainsKey(response.QualifiedName.ToString()))
            {
                DataMessage dm = response as DataMessage;
                string itemUri = dm.ItemUri;
                string treePath = ItemUriToTreePath(itemUri);
                // find the node
                TreeNode tNode = tree.Nodes[treePath];
                if (tNode.Tag is BaseTag)
                    return ((BaseTag)tNode.Tag).HandleResponse(response);
            }
            return ProcessStatus.NotProcessed;
        }

        /// <summary>
        /// Converts an item uri returned from the server into a path in the tree
        /// </summary>
        /// <param name="ItemUri">the item uri</param>
        /// <returns>path to item in the tree</returns>
        public string ItemUriToTreePath(string ItemUri)
        {
            return ItemUri;
        }

        public void ProcessAttributes(Type t, TreeNode node)
        {
            if (node == null)
            {
                tree.Nodes.Clear();
            }
            foreach (PropertyInfo prop in t.GetProperties()) {
                if (prop.IsDefined(typeof(EditorTreePropertyAttribute), false))
                {
                    EditorTreePropertyAttribute treeAttr = (EditorTreePropertyAttribute) prop.GetCustomAttributes(typeof(EditorTreePropertyAttribute), false)[0];
                    string GetListCommand = treeAttr.GetListCommand;
                    string GetListResponse = treeAttr.ListReturnMessage;
                    string GetItemCommand = treeAttr.GetItemCommand;
                    string GetItemResponse = treeAttr.ItemReturnMessage;
                    Type itemType = treeAttr.ItemType;
                    TreeNode newNode;
                    if (node == null)
                    {
                        newNode = tree.Nodes.Add(prop.Name, prop.Name);
                    }
                    else
                    {
                        newNode = node.Nodes.Add(prop.Name, prop.Name);
                    }
                    RegisterCommand(GetListResponse, GetListCommand);
                    RegisterCommand(GetItemResponse, GetItemCommand);
                    ListTag tag = new ListTag(newNode, GetListCommand, GetListResponse, GetItemCommand, GetItemResponse, itemType);
                    newNode.Tag = tag;
                }
            }
        }

        public void StartEdit(string itemPath, object data, EditMode mode)
        {
            // fire off edit command
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
