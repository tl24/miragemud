using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Code;
using MirageGUI.ItemEditor;
using Mirage.Data;
using MirageGUI.Forms;
using Mirage.Data.Attribute;

namespace MirageGUI.Controls
{
    public class ObjectNodeRenderer : DefaultNodeRenderer
    {
        private bool TreeEventsAdded = false;
        private IMasterPresenter presenter;

        public ObjectNodeRenderer(IMasterPresenter presenter)
        {
            this.presenter = presenter;
        }

        public override void Render(System.Windows.Forms.TreeNode Node, object Data)
        {
            base.Render(Node, Data);
            if (!TreeEventsAdded)
            {
                AddTreeEvents(Node.TreeView);
            }
            if (Node.ContextMenuStrip == null)
            {
                if (Data is CollectionItem)
                {
                    Node.ContextMenuStrip = BuildListMenu();
                    Node.ContextMenuStrip.Tag = Node;
                }
                else if (Data is ObjectItem)
                {
                    Node.ContextMenuStrip = BuildItemMenu();
                    Node.ContextMenuStrip.Tag = Node;
                    if (((ObjectItem)Data).Data is Area)
                    {
                        Node.ContextMenuStrip.Items.Add("Save");
                        Node.ContextMenuStrip.Items.Add("Commit");
                    }
                }
            }
        }

        private void AddTreeEvents(TreeView tree)
        {
            tree.NodeMouseDoubleClick +=new TreeNodeMouseClickEventHandler(tree_NodeMouseDoubleClick);
            TreeEventsAdded = true;
        }

        void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            object data = Controller.GetNodeData(e.Node);
            if (data is CollectionItem)
            {
            }
            else if (data is ObjectItem)
            {
                ObjectItem oi = (ObjectItem)data;
                EditHandler handler = new EditHandler(oi.Parent, oi);
                presenter.StartEditItem(oi.Data, EditMode.EditMode, oi.ToString(), new ItemChangedHandler(handler.ItemChanged));
            }
        }

        private ContextMenuStrip BuildListMenu()
        {
            ContextMenuStrip listMenuStrip = new ContextMenuStrip();
            listMenuStrip.Items.Add("Add New");
            listMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(listMenuStrip_Clicked);
            return listMenuStrip;
        }

        private ContextMenuStrip BuildItemMenu()
        {
            ContextMenuStrip itemMenuStrip = new ContextMenuStrip();
            itemMenuStrip.Items.Add("View");
            itemMenuStrip.Items.Add("Edit");
            itemMenuStrip.Items.Add("Delete");
            itemMenuStrip.ItemClicked +=  new ToolStripItemClickedEventHandler(itemMenuStrip_Clicked);
            return itemMenuStrip;
        }

        protected void listMenuStrip_Clicked(object sender, ToolStripItemClickedEventArgs e)
        {
            object data = Controller.GetNodeData((TreeNode)((ContextMenuStrip)sender).Tag);
            switch (e.ClickedItem.Text)
            {
                case "Add New":
                    CollectionItem c = (CollectionItem)data;
                    Type itemType = null;
                    if ((itemType = c.GetItemType()) != typeof(object))
                    {
                        object newItem = Activator.CreateInstance(itemType);
                        EditHandler handler = new EditHandler(c);
                        presenter.StartEditItem(newItem, EditMode.NewMode, "New " + itemType.Name, new ItemChangedHandler(handler.ItemChanged));
                    }
                    else
                    {
                        SelectTypeDialog dlg = new SelectTypeDialog();
                        dlg.TypeFilterFlags = TypeFilterFlags.NoArgConstructor | TypeFilterFlags.Instantiable;
                        dlg.Assemblies.Add("MirageMUD");
                        TypeSelectionArgs targs = c.GetTypeSelectionArgs();

                        dlg.BaseType = targs.BaseType;
                        dlg.DefaultNamespace = targs.DefaultNamespace;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            itemType = dlg.SelectedType;
                            object newItem = Activator.CreateInstance(itemType);
                            EditHandler handler = new EditHandler(c);
                            presenter.StartEditItem(newItem, EditMode.NewMode, "New " + itemType.Name, new ItemChangedHandler(handler.ItemChanged));
                        }
                    }
                    break;

            }
        }

        protected void itemMenuStrip_Clicked(object sender, ToolStripItemClickedEventArgs e)
        {
            object data = Controller.GetNodeData((TreeNode)((ContextMenuStrip)sender).Tag);
            BaseItem oi = (BaseItem) data;
            switch (e.ClickedItem.Text)
            {
                case "View":
                    {
                        EditHandler handler = new EditHandler(oi.Parent, oi);
                        presenter.StartEditItem(oi.Data, EditMode.ViewMode, oi.ToString(), new ItemChangedHandler(handler.ItemChanged));
                    }
                    break;
                case "Edit":
                    {
                        EditHandler handler = new EditHandler(oi.Parent, oi);
                        presenter.StartEditItem(oi.Data, EditMode.EditMode, oi.ToString(), new ItemChangedHandler(handler.ItemChanged));
                    }
                    break;
                case "Delete":
                    string text = string.Format("Are you sure you want to delete {0} ({1})?", oi.ToString(), oi.Data.GetType().Name);
                    if (MessageBox.Show(text, "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        oi.Parent.UpdateChild(oi, oi.Data, Mirage.Command.ChangeType.Delete);
                    }
                    break;
                case "Save":
                    // areas only
                    ((AreaItem)oi).Save();
                    break;
                case "Commit":
                    // areas only
                    ((AreaItem)oi).Commit();
                    break;
            }
        }

        private struct EditHandler
        {
            private BaseItem parent;
            private BaseItem originalItem;
            public EditHandler(BaseItem parent, BaseItem item)
            {
                this.parent = parent;
                this.originalItem = item;
            }

            public EditHandler(BaseItem parent)
                : this(parent, null)
            {
            }

            public void ItemChanged(object sender, MirageGUI.Code.ItemChangedEventArgs e)
            {
                parent.UpdateChild(originalItem, e.Data, e.ChangeType);
            }
        }
    }
}
