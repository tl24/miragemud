using System;

namespace Mirage.Game.World
{
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EditorCollectionAttribute : System.Attribute
    {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp

        private Type _itemType;
        private string _keyProperty;

        /// <summary>
        /// Tags a property to show in the tree view of the editor
        /// </summary>
        /// <param name="GetCommand">The server command to get the children</param>
        /// <param name="returnMessage">The fully qualified name of the message that the items will return with</param>
        /// <param name="itemType">The type of the items within the list</param>
        public EditorCollectionAttribute(Type ItemType)
        {
            this._itemType = ItemType;
        }
         
        /// <summary>
        /// The type of the items within the list
        /// </summary>
        public System.Type ItemType
        {
            get { return this._itemType; }
        }

        /// <summary>
        /// If the collection is a dictionary, gets the property on the value type that
        /// is the key
        /// </summary>
        public string KeyProperty
        {
            get { return _keyProperty; }
            set { _keyProperty = value; }
        }
    }

}
