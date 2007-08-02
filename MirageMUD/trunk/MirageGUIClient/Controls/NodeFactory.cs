using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mirage.Data;
using System.Windows.Forms;

namespace MirageGUI.Controls
{
    /// <summary>
    /// A node factory handles creating sub nodes for collection items.  Generally
    /// each collection item must be created the same way.  The NodeFactory contains
    /// the necessary information for doing this.
    /// </summary>
    public class NodeFactory
    {
        private Type _dataType;
        private List<SubItem> SubItems;
        private static Dictionary<Type, NodeFactory> cache = new Dictionary<Type, NodeFactory>();

        /// <summary>
        /// Creates a NodeFactory for the given type
        /// </summary>
        /// <param name="forType"></param>
        protected NodeFactory(Type forType)
        {
            _dataType = forType;
            SubItems = new List<SubItem>();
            ProcessAttributes();
        }

        /// <summary>
        /// Factory method for creating node factories
        /// </summary>
        /// <param name="forType">the type to create for</param>
        /// <returns>the node factory</returns>
        public static NodeFactory CreateNodeFactory(Type forType)
        {
            if (!cache.ContainsKey(forType))
            {
                cache[forType] = new NodeFactory(forType);
            }
            return cache[forType];
        }

        /// <summary>
        /// Loop the properties for a type looking for those tagged with EditorTreeProperty and
        /// remember their properties for later constructing the nodes
        /// </summary>
        private void ProcessAttributes()
        {
            foreach (PropertyInfo prop in _dataType.GetProperties())
            {
                if (prop.IsDefined(typeof(EditorTreePropertyAttribute), false))
                {
                    EditorTreePropertyAttribute treeAttr = (EditorTreePropertyAttribute)prop.GetCustomAttributes(typeof(EditorTreePropertyAttribute), false)[0];
                    string GetListCommand = treeAttr.GetListCommand;
                    string GetListResponse = treeAttr.ListReturnMessage;
                    string GetItemCommand = treeAttr.GetItemCommand;
                    string GetItemResponse = treeAttr.ItemReturnMessage;
                    Type itemType = treeAttr.ItemType;
                    SubItems.Add(new SubItem(prop.Name, GetListCommand, GetListResponse, GetItemCommand, GetItemResponse, itemType));
                }
            }
        }

        /// <summary>
        /// Constructs the necessary nodes and tags for a node
        /// </summary>
        /// <param name="parentNode">the parent node to add nodes to, or null for the root of the tree</param>
        /// <param name="handler">the tree view handler instance</param>
        public void ConstructNodes(TreeNode parentNode, TreeViewHandler handler)
        {
            foreach (SubItem item in SubItems)
            {
                TreeNode newNode;
                if (parentNode == null)
                {
                    newNode = handler.Tree.Nodes.Add(item.Name, item.Name);
                }
                else
                {
                    newNode = parentNode.Nodes.Add(item.Name, item.Name);
                }
                handler.RegisterCommand(item.GetResponse, item.GetCommand);
                handler.RegisterCommand(item.GetItemResponse, item.GetItemCommand);
                ListTag tag = new ListTag(newNode, item.GetCommand, item.GetResponse, item.GetItemCommand, item.GetItemResponse, item.itemType);
                newNode.Tag = tag;
            }
        }

        /// <summary>
        /// Struct for holding the information for sub nodes
        /// </summary>
        private struct SubItem
        {
            public string GetCommand;
            public string GetResponse;
            public string GetItemCommand;
            public string GetItemResponse;
            public Type itemType;
            public string Name;

            public SubItem(string Name, string GetCommand, string GetResponse, string GetItemCommand, string GetItemResponse, Type itemType)
            {
                this.Name = Name;
                this.GetCommand = GetCommand;
                this.GetResponse = GetResponse;
                this.GetItemCommand = GetItemCommand;
                this.GetItemResponse = GetItemResponse;
                this.itemType = itemType;
            }
        }
    }    
}
