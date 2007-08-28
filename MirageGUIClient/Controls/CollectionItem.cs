using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mirage.Command;
using Mirage.Data.Query;

namespace MirageGUI.Controls
{
    public class CollectionItem : KeyValueItem
    {
        protected Type itemType;

        public CollectionItem(BaseItem parent, object data, string name, Type itemType)
            : base(name, parent, data)
        {
            this.itemType = itemType;
        }

        protected override void ProcessData()
        {
            foreach (object item in (IEnumerable) Data)
            {
                children.Add(new ObjectItem(this, item));
            }
            _dataProcessed = true;
        }

        public bool IsOrdered
        {
            get { return !(Data is IDictionary); }
        }

        public virtual Type GetItemType()
        {
            if (itemType == null)
            {
                Type collType = Data.GetType();
                if (collType.GetInterface(typeof(ICollection<>).FullName) != null)
                {
                    Type ICol = collType.GetInterface(typeof(ICollection<>).FullName);
                    itemType = ICol.GetGenericArguments()[0];
                }
                else
                {
                    itemType = typeof(object);
                }
            }
            return itemType;
        }

        public override void UpdateChild(BaseItem child, object itemData, ChangeType changeType)
        {
            switch (changeType)
            {

                case ChangeType.Add:
                    if (Data is IList)
                    {
                        ((IList)Data).Add(itemData);
                    }
                    child = new ObjectItem(this, itemData);
                    children.Add(child);
                    this.OnStructureChanged();
                    break;
                case ChangeType.Edit:
                    child.Data = itemData;
                    break;
            }
            child.SetDirty();
        }
    }

    public class KeyValueItem : ObjectItem
    {
        private object _key;
        public KeyValueItem(object key, BaseItem parent, object data)
            : base(parent, data)
        {
            _key = key;
        }

        public object Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override string ToString()
        {
            return _key.ToString() + (IsDirty ? " *" : "");
        }
    }
}
