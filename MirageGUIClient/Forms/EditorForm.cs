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
    public partial class EditorForm : Form
    {
        private object data;
        public EditorForm(object data)
        {
            this.data = data;
            InitializeComponent();
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
            // build the form
            Type t = data.GetType();
            bool first = true;
            foreach (PropertyInfo prop in t.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite && (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string)))
                {
                    Label lbl = new Label();
                    lbl.Text = prop.Name;
                    TextBox tb = new TextBox();
                    tb.Text = prop.GetValue(data, null).ToString();

                    int row = 0;
                    if (first)
                    {
                        row = 0;
                        first = false;
                    }
                    else
                    {
                        row = TableLayout.RowCount;
                        TableLayout.RowCount = row + 1;
                    }

                    TableLayout.Controls.Add(lbl, 0, row);
                    TableLayout.Controls.Add(tb, 1, row);
                    
                }
            }
            Button btn = new Button();
            btn.Text = "Save";
            TableLayout.Controls.Add(btn, 1, TableLayout.RowCount);
        }
    }
}