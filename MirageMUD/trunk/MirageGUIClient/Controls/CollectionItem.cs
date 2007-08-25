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
        private Type itemType;

        public CollectionItem(BaseItem parent, object data, string name)
            : base(name, parent, data)
        {
        }

        protected override void ProcessData()
        {
            if (Data is IDictionary)
            {
                IDictionary dict = (IDictionary)Data;
                foreach (object key in dict.Keys)
                {
                    children.Add(new KeyValueItem(key, this, dict[key]));
                }
            }
            else
            {
                foreach (object item in (IEnumerable) Data)
                {
                    children.Add(new ObjectItem(this, item));
                }
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
                if (collType.GetInterface(typeof(IDictionary<,>).FullName) != null)
                {
                    Type IDict = collType.GetInterface(typeof(IDictionary<,>).FullName);
                    itemType = IDict.GetGenericArguments()[1];
                }
                else if (collType.GetInterface(typeof(ICollection<>).FullName) != null)
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
            if (Data is IDictionary)
            {
                IDictionary dict = (IDictionary) Data;
                switch (changeType)
                {
                    
                    case ChangeType.Add:
                        object key = null;
                        if (itemData is IUri)
                        {
                            key = ((IUri)itemData).Uri;
                            dict[key] = itemData;
                        }
                        else
                        {
                            key = itemData.ToString();
                            dict[key] = itemData;
                        }
                        child = new KeyValueItem(key, this, itemData);
                        children.Add(child);
                        this.OnStructureChanged();
                        break;
                    case ChangeType.Edit:
                        child.Data = itemData;
                        break;
                }
            }
            else
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

        public override string ToString()
        {
            return _key.ToString() + (IsDirty ? " *" : "");
        }
    }
}
