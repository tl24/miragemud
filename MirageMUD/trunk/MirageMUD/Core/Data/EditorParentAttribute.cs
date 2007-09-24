using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Denotes a property that should be set to a parent object upon creation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public sealed class EditorParentAttribute : System.Attribute
    {
        private int _parentLevel;

        public EditorParentAttribute(int parentLevel)
        {
            this._parentLevel = parentLevel;
        }

        /// <summary>
        /// The number of levels above the object that is the parent
        /// </summary>
        public int ParentLevel
        {
            get { return this._parentLevel; }
        }


    }
}
