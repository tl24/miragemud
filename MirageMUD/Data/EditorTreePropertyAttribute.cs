using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data
{
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EditorTreePropertyAttribute : System.Attribute
    {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp

        private string _getListCommand;
        private string _listReturnMessage;
        private string _getItemCommand;
        private string _itemReturnMessage;
        private Type _itemType;
        private bool _lazyLoad;

        /// <summary>
        /// Tags a property to show in the tree view of the editor
        /// </summary>
        /// <param name="GetCommand">The server command to get the children</param>
        /// <param name="returnMessage">The fully qualified name of the message that the items will return with</param>
        /// <param name="itemType">The type of the items within the list</param>
        public EditorTreePropertyAttribute(string GetListCommand, string ListReturnMessage, string GetItemCommand, string ItemReturnMessage, Type ItemType)
        {
            this._getListCommand = GetListCommand;
            this._listReturnMessage = ListReturnMessage;
            this._getItemCommand = GetItemCommand;
            this._itemReturnMessage = ItemReturnMessage;
            this._itemType = ItemType;
            this._lazyLoad = true;
        }

        /// <summary>
        /// Tags a property to show in the tree view of the editor
        /// </summary>
        /// <param name="itemType">The type of the items within the list</param>
        public EditorTreePropertyAttribute(Type ItemType)
        {
            this._itemType = ItemType;
            this._lazyLoad = false;
        }

        /// <summary>
        /// The server command used to get the list of items
        /// </summary>
        public string GetListCommand
        {
            get
            {
                return this._getListCommand;
            }
        }

        /// <summary>
        /// The namespace and name of the message that will be returned for a list request
        /// </summary>
        public string ListReturnMessage
        {
            get { return this._listReturnMessage; }
        }
         
        /// <summary>
        /// The type of the items within the list
        /// </summary>
        public System.Type ItemType
        {
            get { return this._itemType; }
        }

        /// <summary>
        /// Command to get a single item from the server
        /// </summary>
        public string GetItemCommand
        {
            get { return this._getItemCommand; }
        }

        /// <summary>
        /// The namespace and name of the response for a single item request
        /// </summary>
        public string ItemReturnMessage
        {
            get { return this._itemReturnMessage; }
        }

        /// <summary>
        /// If true the property should be loaded on demand
        /// </summary>
        public bool LazyLoad
        {
            get { return _lazyLoad; }
        }
    }

}
