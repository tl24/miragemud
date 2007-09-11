using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public sealed class EditorTypeSelectorAttribute : System.Attribute
    {
        private string _baseTypeName;
        private Type _baseType;
        private string _defaultNamespace;

        public EditorTypeSelectorAttribute()
        {
        }

        public EditorTypeSelectorAttribute(string defaultNamespace)
        {
            this._defaultNamespace = defaultNamespace;
        }

        public EditorTypeSelectorAttribute(Type baseType)
        {
            this._baseType = baseType;
        }

        public EditorTypeSelectorAttribute(string baseTypeName, string defaultNamespace)
        {
            this._defaultNamespace = defaultNamespace;
            this._baseTypeName = baseTypeName;
        }

        public EditorTypeSelectorAttribute(Type baseType, string defaultNamespace)
        {
            this._baseType = baseType;
            this._defaultNamespace = defaultNamespace;
        }

        /// <summary>
        /// The name of the base type, any class selected must inherit from this type
        /// or implement the interface if the base type is an interface
        /// </summary>
        public string BaseTypeName
        {
            get { return this._baseTypeName; }
        }

        /// <summary>
        /// The base type, any class selected must inherit from this type
        /// or implement the interface if the base type is an interface
        /// </summary>
        public System.Type BaseType
        {
            get { return this._baseType; }
        }

        /// <summary>
        /// The default namespace, if specified, the tree will default open to this location
        /// </summary>
        public string DefaultNamespace
        {
            get { return this._defaultNamespace; }
        }
    }
}
