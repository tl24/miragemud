using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace MirageGUI.Forms
{
    public partial class SelectTypeDialog : Form
    {
        private Type _selectedType;

        public SelectTypeDialog()
        {
            InitializeComponent();
        }

        private void SelectTypeDialog_Shown(object sender, EventArgs e)
        {
            TypeTree.Nodes.Clear();
            TreeNode node = TypeTree.Nodes.Add("Types");
            TraverseAssembly(this.GetType().Assembly, node);
        }

        private void TraverseAssembly(Assembly asmbly, TreeNode parent)
        {
            TreeNode asmNode = parent.Nodes.Add(asmbly.GetName().Name);
            foreach (Module mod in asmbly.GetModules())
            {
                TraverseModule(mod, asmNode);
            }
        }

        private void TraverseModule(Module mod, TreeNode parent)
        {
            TreeNode modNode = parent.Nodes.Add(mod.Name);
            foreach (Type t in mod.GetTypes())
            {
                TraverseType(t, modNode);
            }
        }

        private void TraverseType(Type t, TreeNode parent)
        {
            TreeNode typeNode = parent.Nodes.Add(t.Name);
            typeNode.Tag = t;
            foreach (Type subType in t.GetNestedTypes())
            {
                TraverseType(subType, typeNode);
            }
        }

        public System.Type SelectedType
        {
            get { return this._selectedType; }
            set { this._selectedType = value; }
        }
    }
}