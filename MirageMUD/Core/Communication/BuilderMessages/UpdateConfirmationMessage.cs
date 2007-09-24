using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;

namespace Mirage.Core.Communication.BuilderMessages
{
    public class UpdateConfirmationMessage : Message
    {
        private string _itemUri;
        private ChangeType _changeType;

        public UpdateConfirmationMessage(Uri Namespace, string name, string itemUri, ChangeType changeType)
            : base(MessageType.Confirmation, Namespace, name)
        {
            this._itemUri = itemUri;
            this._changeType = changeType;
        }

        public UpdateConfirmationMessage()
            : base()
        {
        }

        /// <summary>
        /// The Uri of the item that was updated
        /// </summary>
        public string ItemUri
        {
            get { return this._itemUri; }
            set { this._itemUri = value; }
        }

        /// <summary>
        /// The type of change that occurred
        /// </summary>
        public ChangeType ChangeType
        {
            get { return this._changeType; }
            set { this._changeType = value; }
        }


    }
}
