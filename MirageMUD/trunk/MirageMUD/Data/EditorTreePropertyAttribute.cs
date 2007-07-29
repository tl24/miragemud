using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data
{
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EditorTreePropertyAttribute : System.Attribute
    {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp

        private string _getCommand;
        private string _returnMessage;

        /// <summary>
        /// Tags a property to show in the tree view of the editor
        /// </summary>
        /// <param name="GetCommand">The server command to get the children</param>
        public EditorTreePropertyAttribute(string GetCommand)
        {
            this._getCommand = GetCommand;
        }

        /// <summary>
        /// Tags a property to show in the tree view of the editor
        /// </summary>
        /// <param name="GetCommand">The server command to get the children</param>
        /// <param name="returnMessage">The fully qualified name of the message that the items will return with</param>
        public EditorTreePropertyAttribute(string GetCommand, string ReturnMessage)
        {
            this._getCommand = GetCommand;
            this._returnMessage = ReturnMessage;
        }

        /// <summary>
        /// The server command used to get the items
        /// </summary>
        public string GetCommand
        {
            get
            {
                return this._getCommand;
            }
        }

        /// <summary>
        /// The name of the message that will be returned with the items
        /// </summary>
        public string ReturnMessage
        {
            get { return this._returnMessage; }
        }


    }

}
