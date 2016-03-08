using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MirageGUI.Controls
{
    public interface ITreeModel
    {
        IEnumerable GetChildren(TreePath path);

        int GetChildCount(TreePath path);        

        event EventHandler<TreeEventArgs> StructureChanged;

        event EventHandler<TreeDataEventArgs> DataChanged;
    }
}
