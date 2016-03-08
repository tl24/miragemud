using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace MirageGUI.Forms
{
    public partial class SelectTypeDialog : Form
    {
        private Type _selectedType;
        private TypeFilterFlags _typeFilterFlags;
        private Type _baseType;
        private ArrayList _assemblies = new ArrayList();
        private string _defaultNamespace;

        public SelectTypeDialog()
        {
            InitializeComponent();
        }

        private void SelectTypeDialog_Shown(object sender, EventArgs e)
        {
            TypeTree.Nodes.Clear();
            if (Assemblies.Count == 0)
                Assemblies.Add(Assembly.GetExecutingAssembly());

            foreach (object asm in Assemblies)
            {
                Assembly asmObj = null;
                if (asm is Assembly)
                {
                    asmObj = (Assembly)asm;
                }
                else if (asm is string)
                {
                    asmObj = Assembly.Load((string)asm);
                }
                else if (asm is AssemblyName)
                {
                    asmObj = Assembly.Load((AssemblyName)asm);
                }
                else
                {
                    throw new ArgumentException("Invalid assembly object: " + asm);
                }
                TraverseAssembly(asmObj);
            }
            if (TypeTree.SelectedNode == null && DefaultNamespace != null && DefaultNamespace != string.Empty)
            {
                string[] parts = DefaultNamespace.Split('.');
                TreeNodeCollection nodes = TypeTree.Nodes;
                bool setNode = true;
                TreeNode lastNode = null;
                foreach (string part in parts)
                {
                    TreeNode[] results = nodes.Find(part, false);
                    
                    if (results.Length == 0)
                    {
                        setNode = false;
                        break;
                    }
                    else
                    {
                        lastNode = results[0];
                        nodes = lastNode.Nodes;
                    }                    
                }
                if (setNode && lastNode != null)
                    TypeTree.SelectedNode = lastNode;

            }
        }

        private void TraverseAssembly(Assembly asmbly)
        {
            foreach (Type t in asmbly.GetTypes())
            {
                if (!IsTypeAvailable(t))
                    continue;

                string namspace = t.Namespace;
                string[] parts = namspace.Split('.');
                TreeNodeCollection nodes = TypeTree.Nodes;
                foreach (string part in parts)
                {
                    TreeNode[] results = nodes.Find(part, false);
                    TreeNode nextNode;
                    if (results.Length == 0)
                    {
                        nextNode = nodes.Add(part,part,"folder_closed");                        
                    }
                    else
                    {
                        nextNode = results[0];
                    }
                    nodes = nextNode.Nodes;
                }
                TreeNode typeNode = nodes.Add(t.FullName, t.Name, "class");
                typeNode.Tag = t;
                if (SelectedType == t)
                    TypeTree.SelectedNode = typeNode;
            }
        }

        private bool IsTypeAvailable(Type t)
        {
            TypeFilterFlags flags = this.TypeFilterFlags;

            if (BaseType != null && !BaseType.IsAssignableFrom(t))
                return false;

            if (t.IsNotPublic && ((flags & TypeFilterFlags.PublicOnly) == 0))
                return false;

            if (t.IsInterface && ((flags & TypeFilterFlags.Interfaces) == 0))
                return false;

            if (t.IsAbstract && ((flags & TypeFilterFlags.Abstract) == 0))
                return false;

            if ((flags & TypeFilterFlags.Instantiable) > 0)
            {
                if (t.IsInterface || t.IsAbstract)
                    return false;
                bool bFound = false;
                foreach (ConstructorInfo cInfo in t.GetConstructors())
                {
                    if (cInfo.IsPublic)
                    {
                        bFound = true;
                        break;
                    }
                }
                if (!bFound)
                    return false;
            }

            if ((flags & TypeFilterFlags.NoArgConstructor) > 0)
            {
                bool bFound = false;
                foreach (ConstructorInfo cInfo in t.GetConstructors())
                {
                    if (cInfo.IsPublic && cInfo.GetParameters().Length == 0)
                    {
                        bFound = true;
                        break;
                    }
                }
                if (!bFound)
                    return false;
            }
            return true;
        }

        public System.Type SelectedType
        {
            get { return this._selectedType; }
            set { this._selectedType = value; }
        }

        /// <summary>
        /// Flags for filtering types from the list
        /// </summary>
        public TypeFilterFlags TypeFilterFlags
        {
            get { return this._typeFilterFlags; }
            set { this._typeFilterFlags = value; }
        }

        /// <summary>
        /// Class or interface that the types must inherit from,
        /// </summary>
        public Type BaseType
        {
            get { return this._baseType; }
            set { this._baseType = value; }
        }

        public System.Collections.ArrayList Assemblies
        {
            get { return this._assemblies; }
            set { this._assemblies = value; }
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            if (TypeTree.SelectedNode == null)
            {
                MessageBox.Show("You must select a type");
            }
            else if (TypeTree.SelectedNode.Tag == null)
            {
                MessageBox.Show("You have not selected a valid type");
            }
            else
            {
                SelectedType = (Type)TypeTree.SelectedNode.Tag;
            }
        }

        public string DefaultNamespace
        {
            get { return this._defaultNamespace; }
            set { this._defaultNamespace = value; }
        }

    }

    [Flags]
    public enum TypeFilterFlags
    {
        PublicOnly = 1,
        Instantiable = 2,
        Interfaces = 4,
        Abstract = 8,
        NoArgConstructor = 16
    }
}