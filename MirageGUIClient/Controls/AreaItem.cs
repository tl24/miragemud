using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Data;
using Mirage.Stock.Data;
using Mirage.Core.Communication.BuilderMessages;

namespace MirageGUI.Controls
{
    public class AreaItem : ObjectItem
    {
        private bool _isNew;

        public AreaItem(BaseItem parent, object data, bool isNew)
            : base(parent, data)
        {
            _isNew = isNew;
            _isDirty = isNew;
        }

        public AreaItem(BaseItem parent, object data)
            : base(parent, data)
        {
            _isNew = true;
            _isDirty = true;
        }

        public void Save()
        {
            FindModel().IOHandler.SendObject("UpdateItem", new object[] { _isNew ? ChangeType.Add : ChangeType.Edit, Data });
        }

        public void Commit()
        {
            FindModel().IOHandler.SendString("SaveArea " + Data.Uri );
        }

        public new Area Data
        {
            get { return (Area)base.Data; }
            set { base.Data = value; }
        }
    }
}
