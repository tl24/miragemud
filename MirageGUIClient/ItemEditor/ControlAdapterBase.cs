using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Mirage.Data;

namespace MirageGUI.ItemEditor
{
    /// <summary>
    /// ControlAdapterBase is an adapter from an object property and the windows form controls
    /// needed to edit the property.  Subclasses will create the approriate label and editor
    /// control and handle updates to and from the object to the screen.
    /// </summary>
    public abstract class ControlAdapterBase : IComparable<ControlAdapterBase>
    {
        protected PropertyInfo _property;
        protected Label _label;
        protected Control _editControl;
        protected int _priority;
        protected bool _isReadonly;
        protected bool _isKey;
        protected EditMode _mode;

        public ControlAdapterBase(PropertyInfo property)
        {
            _property = property;
            string labelText = _property.Name;
            if (_property.IsDefined(typeof(EditorAttribute), true))
            {
                EditorAttribute attr = (EditorAttribute)_property.GetCustomAttributes(typeof(EditorAttribute), true)[0];
                if (attr.Label != null && attr.Label.Length > 0)
                    labelText = attr.Label;
                _isKey = attr.IsKey;
                _isReadonly = attr.IsReadonly;
                _priority = attr.Priority;                
            }

            _label = CreateLabel(labelText);
            _editControl = CreateEditControl();
        }

        /// <summary>
        /// Sets the current editing mode of the control
        /// </summary>
        public EditMode Mode
        {
            get { return _mode; }
            set { 
                _mode = value;
                OnModeChange();
            }
        }

        /// <summary>
        /// Child classes should override this to set their enabled state
        /// for the edit control
        /// </summary>
        protected virtual void OnModeChange()
        {
            SetLabelStyleValid();
            switch(Mode) {
                case EditMode.ViewMode:
                    this.EditControl.Enabled = false;
                    break;
                case EditMode.NewMode:
                    this.EditControl.Enabled = true;
                    break;
                case EditMode.EditMode:
                    this.EditControl.Enabled = !this._isKey;
                    break;
            }
        }

        /// <summary>
        /// Creates the label control for the given label text for
        /// this property.
        /// </summary>
        /// <param name="labelText">text for the label control</param>
        /// <returns>a label control</returns>
        protected virtual Label CreateLabel(string labelText)
        {
            Label l = new Label();
            l.Text = labelText;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            return l;
        }

        /// <summary>
        /// Sets the label style to its default state, indicating no errors
        /// are present
        /// </summary>
        protected virtual void SetLabelStyleValid()
        {
        }

        /// <summary>
        /// Sets the label style to an invalid state, indicating that there
        /// are errors present in the edit control
        /// </summary>
        protected virtual void SetLabelStyleInvalid()
        {
        }

        /// <summary>
        /// Child classes should create the approriate edit control
        /// for the property
        /// </summary>
        /// <returns>a windows form control to edit the property value</returns>
        protected abstract Control CreateEditControl();

        /// <summary>
        /// Update this instance edit control value from the object property
        /// it is bound to
        /// </summary>
        /// <param name="instance">the object instance to update from</param>
        public virtual void UpdateControlFromObject(object instance)
        {
            object value = _property.GetValue(instance,null);
            ControlValue = value;
        }

        /// <summary>
        /// Update the object from this edit control instance value
        /// </summary>
        /// <param name="instance">the object instance to update</param>
        public virtual void UpdateObjectFromControl(object instance)
        {
            if (!_isReadonly)
            {
                object value = ControlValue;
                _property.SetValue(instance, value, null);
            }
        }

        /// <summary>
        /// Validates the contents of the edit control.  If any invalid
        /// input is detected an error will be thrown
        /// </summary>
        public void Validate()
        {
            try
            {
                OnValidate();
                SetLabelStyleValid();
            }
            catch (Exception e)
            {
                SetLabelStyleInvalid();
                throw e;
            }
        }

        protected virtual void OnValidate()
        {
        }

        /// <summary>
        /// Get or set the control's value
        /// </summary>
        protected abstract object ControlValue
        {
            get;
            set;
        }

        /// <summary>
        /// Get the label control tied to this property
        /// </summary>
        public virtual Label LabelControl
        {
            get { return _label; }
        }

        /// <summary>
        /// Get the edit control tied to this property
        /// </summary>
        public virtual Control EditControl
        {
            get { return this._editControl; }
        }

        #region IComparable<ItemProperty> Members

        public virtual int CompareTo(ControlAdapterBase other)
        {
            int thisPriority = _priority;
            if (thisPriority == 0)
                thisPriority = int.MaxValue;

            int otherPriority = other._priority;
            if (otherPriority == 0)
                otherPriority = int.MaxValue;

            return thisPriority - otherPriority;
        }

        #endregion
 

    }

    public class StringTextBoxAdapter : ControlAdapterBase
    {

        public StringTextBoxAdapter(PropertyInfo property) : base(property)
        {
        }

        public new TextBoxBase EditControl {
            get { return (TextBoxBase)base.EditControl; }
        }

        protected override Control CreateEditControl()
        {
            return new TextBox();
        }

        protected override object ControlValue
        {
            get
            {
                return EditControl.Text;
            }
            set
            {
                EditControl.Text = value == null ? "" : value.ToString();
            }
        }
    }

    public class MultilineTextBoxAdapter : StringTextBoxAdapter
    {
        public MultilineTextBoxAdapter(PropertyInfo property)
            : base(property)
        {
        }

        protected override Control CreateEditControl()
        {
            TextBox tb = (TextBox) base.CreateEditControl();
            tb.Multiline = true;
            // start off with three lines
            tb.Height = tb.Height * 3;
            return tb;
        }
    }

    public class EnumComboAdapter : ControlAdapterBase
    {
        
        public EnumComboAdapter(PropertyInfo property)
            : base(property)
        {
        }

        protected override Control CreateEditControl()
        {
            ComboBox cb = new ComboBox();
            Array enumValues = Enum.GetValues(_property.PropertyType);
            foreach (object o in enumValues)
            {
                cb.Items.Add(o);
            }
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            return cb;
        }

        protected override object ControlValue
        {
            get
            {
                object value = EditControl.SelectedItem;
                return Enum.Parse(_property.PropertyType, value.ToString(), true);
            }
            set
            {
                EditControl.SelectedItem = value;
            }
        }

        public new ComboBox EditControl
        {
            get { return (ComboBox)base.EditControl; }
        }
    }
}
