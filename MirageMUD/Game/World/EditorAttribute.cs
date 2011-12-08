using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World
{
    /// <summary>
    /// Editor attribute is used on a property of a class to configure how it is shown
    /// in the graphical editor
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class EditorAttribute : System.Attribute
    {
        private int _priority = 0;
        private string _label;
        private string _editorType;
        private bool _isReadonly;
        private bool _isKey;
        private bool _visible = true;

        // This is a positional argument.
        public EditorAttribute()
        {
        }

        public int Priority
        {
            get { return this._priority; }
            set { this._priority = value; }
        }

        public string Label
        {
            get { return this._label; }
            set { this._label = value == string.Empty ? null : value; }
        }

        public string EditorType
        {
            get { return this._editorType; }
            set { this._editorType = value == string.Empty ? null : value; }
        }

        public bool IsReadonly
        {
            get { return this._isReadonly; }
            set { this._isReadonly = value; }
        }

        public bool IsKey
        {
            get { return this._isKey; }
            set { this._isKey = value; }
        }

        public bool Visible
        {
            get { return this._visible; }
            set { this._visible = value; }
        }


    }

}
