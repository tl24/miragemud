using System;
using System.Collections.Generic;
using System.Text;

namespace MirageGUI.Controls
{
    public class RootTag : ITreeViewTagData
    {
        private bool _loaded;
        private object _data;

        public RootTag()
        {
        }

        #region ITreeViewTagData Members

        public bool IsLoaded
        {
            get { return _loaded; }
        }

        public object Data
        {
            get { return _data; }
        }

        public string GetCommand
        {
            get { return "GetAreas"; }
        }

        public string GetResponse
        {
            get { return "AreaList"; }
        }

        public string UpdateCommand
        {
            get { return null; }
        }

        public string UpdateResponse
        {
            get { return null; }
        }

        #endregion


    }
}
