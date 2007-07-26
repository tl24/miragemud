using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Mirage.Data;

namespace MirageGUI.ItemEditor
{
    public enum EditMode
    {
        ViewMode,
        NewMode,
        EditMode
    }

    public class EditorControlFactory
    {
        private object _instance;
        private Type _instanceType;
        private List<ControlAdapterBase> _controlAdapters;
        private EditMode _mode;

        /// <summary>
        /// Constructs an editor control factory to produce
        /// controls to edit the object
        /// </summary>
        /// <param name="instance">the object instance</param>
        public EditorControlFactory(Type instanceType)
        {
            this._instanceType = instanceType;
            _controlAdapters = new List<ControlAdapterBase>();
            InitControls();
        }

        private void InitControls() {
            foreach (PropertyInfo prop in _instanceType.GetProperties())
            {
                string EditorType = "";
                bool IsReadonly = false;
                if (prop.IsDefined(typeof(EditorAttribute), false))
                {
                    EditorAttribute eAttr = (EditorAttribute)prop.GetCustomAttributes(typeof(EditorAttribute), false)[0];
                    EditorType = eAttr.EditorType;
                    IsReadonly = eAttr.IsReadonly;
                }
                if (prop.CanRead && (prop.CanWrite || IsReadonly) && (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string)))
                {
                    _controlAdapters.Add(CreateControlAdapter(prop, EditorType));
                }
            }
            _controlAdapters.Sort();
        }

        private ControlAdapterBase CreateControlAdapter(PropertyInfo property, string ControlType)
        {
            Type adapterType;
            if ("Multiline" == ControlType)
                adapterType = typeof(MultilineTextBoxAdapter);
            else
                adapterType = typeof(StringTextBoxAdapter);

            return (ControlAdapterBase) Activator.CreateInstance(adapterType, property);
        }

        /// <summary>
        /// The object instance being edited
        /// </summary>
        public object Instance
        {
            get { return this._instance; }
            set { this._instance = value; }
        }

        /// <summary>
        /// Updates the specified object from the edit controls
        /// </summary>
        public void UpdateObjectFromControls()
        {
            _controlAdapters.ForEach(
                delegate(ControlAdapterBase c)
                {
                    c.Validate();
                }
            );
            // if we make it here, validation passed
            _controlAdapters.ForEach(
                delegate(ControlAdapterBase c)
                {
                    c.UpdateObjectFromControl(_instance);
                }
            );
        }

        /// <summary>
        /// Updates the control values from the object
        /// </summary>
        public void UpdateControlsFromObject()
        {
            _controlAdapters.ForEach(
                delegate(ControlAdapterBase c)
                {
                    c.UpdateControlFromObject(_instance);
                }
            );
        }

        /// <summary>
        /// Returns the number of controls for this instance.  This
        /// will correspond to the number of editable properties and readonly
        /// properties on the object type.
        /// </summary>
        public int ControlCount
        {
            get { return _controlAdapters.Count; }
        }

        /// <summary>
        /// The label control for a given row
        /// </summary>
        /// <param name="row">the row index</param>
        /// <returns>the label control</returns>
        public Label LabelControl(int row)
        {
            return _controlAdapters[row].LabelControl;
        }

        /// <summary>
        /// The edit control for a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Control EditControl(int row)
        {
            return _controlAdapters[row].EditControl;
        }

        public EditMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _controlAdapters.ForEach(
                    delegate(ControlAdapterBase t)
                    {
                        t.Mode = _mode;
                    }
                );
            }
        }
    }
}
