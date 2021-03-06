using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MirageGUI.ItemEditor;
using MirageGUI.Code;
using Mirage.Game.Communication;
using Mirage.Game.Command;
using Mirage.Game.Communication.BuilderMessages;

namespace MirageGUI.Forms
{
    public partial class EditorForm : Form
    {
        private object data;
        private EditMode initialMode;
        private IOHandler _ioHandler;
        private bool closeFlag = false;

        private EditorControlFactory controlFactory;

        public event ItemChangedHandler ItemChanged;

        public EditorForm(object data, EditMode initialMode)
        {
            this.data = data;
            this.initialMode = initialMode;
            InitializeComponent();
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
            // build the form
            controlFactory = new EditorControlFactory(data.GetType());

            TableLayout.RowCount = controlFactory.ControlCount;
            for (int i = 0; i < controlFactory.ControlCount; i++) {
                TableLayout.Controls.Add(controlFactory.LabelControl(i), 0, i);
                TableLayout.Controls.Add(controlFactory.EditControl(i), 1, i);
                controlFactory.EditControl(i).Dock = DockStyle.Fill;                    
            }
            controlFactory.Mode = initialMode;
            controlFactory.Instance = data;
            controlFactory.UpdateControlsFromObject();
        }

        private void SaveClose_Click(object sender, EventArgs e)
        {
            Save();
            closeFlag = true;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Save();
            closeFlag = false;
            SetMode(EditMode.ViewMode);
        }

        private void Save()
        {
            controlFactory.UpdateObjectFromControls();
            OnItemChanged(new MirageGUI.Code.ItemChangedEventArgs(initialMode == EditMode.NewMode ? ChangeType.Add : ChangeType.Edit, data));
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (initialMode == EditMode.NewMode)
            {
                Close();
            }
            else
            {
                controlFactory.UpdateControlsFromObject();
                SetMode(EditMode.ViewMode);
            }
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            SetMode(EditMode.EditMode);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetMode(EditMode Mode)
        {
            controlFactory.Mode = Mode;
            if (Mode == EditMode.EditMode)
            {
                SaveButton.Visible = true;
                SaveCloseButton.Visible = true;
                CancelButton.Text = "Cancel";
                CancelButton.Click -= new EventHandler(Edit_Click);
                CancelButton.Click += new EventHandler(Cancel_Click);
            }
            else
            {
                SaveButton.Visible = false;
                SaveCloseButton.Visible = false;
                CancelButton.Text = "Edit";
                CancelButton.Click += new EventHandler(Edit_Click);
                CancelButton.Click -= new EventHandler(Cancel_Click);
            }

        }

        protected void OnItemChanged(MirageGUI.Code.ItemChangedEventArgs args)
        {
            if (ItemChanged != null)
                ItemChanged.Invoke(this, args);
        }
    }
}