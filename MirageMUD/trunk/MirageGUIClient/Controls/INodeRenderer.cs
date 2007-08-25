using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MirageGUI.Controls
{
    /// <summary>
    /// Interface to control the rendering of nodes
    /// </summary>
    public interface INodeRenderer
    {
        TreeViewController Controller { get; set; }
            
        void Render(TreeNode Node, object Data);
    }

    public class DefaultNodeRenderer : INodeRenderer
    {
        private TreeViewController _controller;

        public virtual void Render(TreeNode Node, object Data)
        {
            Node.Text = Data == null ? "Null" : Data.ToString();
        }

        public TreeViewController Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }
    }
}
