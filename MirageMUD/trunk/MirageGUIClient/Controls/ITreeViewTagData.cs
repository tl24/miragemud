using System;
using System.Collections.Generic;
using System.Text;

namespace MirageGUI.Controls
{
    /// <summary>
    /// Interface for object to be stored in the "Tag" property
    /// of a tree view node to manage the data
    /// </summary>
    public interface ITreeViewTagData
    {
        /// <summary>
        /// Flag to check if the data has been loaded yet
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// The data for the node if any
        /// </summary>
        object Data
        {
            get;
        }

        /// <summary>
        /// Server command to get the data for this node
        /// </summary>
        string GetCommand
        {
            get;
        }

        /// <summary>
        /// The response for the get command
        /// </summary>
        string GetResponse
        {
            get;
        }

        /// <summary>
        /// The server command to update the item
        /// </summary>
        string UpdateCommand
        {
            get;
        }

        /// <summary>
        /// The response for updating the item
        /// </summary>
        string UpdateResponse
        {
            get;
        }

    }
}
