using System;
using System.Collections.Generic;
using System.Text;
using MirageGUI.ItemEditor;
using MirageGUI.Controls;

namespace MirageGUI.Code
{
    public interface IMasterPresenter
    {
        /// <summary>
        /// Starts the edit process for editing the item by opening up an edit form
        /// </summary>
        /// <param name="data">the data to edit</param>
        /// <param name="mode">the edit mode: View, Add, Edit</param>
        /// <param name="name">the name of the item</param>
        /// <param name="changeHandler">event handler for changes</param>
        void StartEditItem(object data, EditMode mode, string name, ItemChangedHandler changeHandler);        
    }
}
