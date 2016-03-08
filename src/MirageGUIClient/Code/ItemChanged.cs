using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Command;
using Mirage.Game.Communication.BuilderMessages;

namespace MirageGUI.Code
{

    /// <summary>
    /// Event args for the item changed event
    /// </summary>
    public class ItemChangedEventArgs : System.EventArgs
    {
        private ChangeType _changeType;
        private object _data;

        public ItemChangedEventArgs(ChangeType changeType, object data)
        {
            this._changeType = changeType;
            this._data = data;
        }

        public ChangeType ChangeType
        {
            get { return this._changeType; }
        }

        public object Data
        {
            get { return this._data; }
        }

    }

    /// <summary>
    /// Event handler for an item changed event when an object being edited has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ItemChangedHandler(object sender, ItemChangedEventArgs e);
}
