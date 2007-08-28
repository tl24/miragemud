using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mirage.Command;
using Mirage.Data.Query;

namespace MirageGUI.Controls
{
    public class DictionaryItem : CollectionItem
    {
        private string _keyProperty;

        public DictionaryItem(BaseItem parent, object data, string name, Type itemType, string keyProp)
            : base(parent, data, name, itemType)
        {
            this.itemType = itemType;
            _keyProperty = keyProp;
        }

        protected override void ProcessData()
        {
            IDictionary dict = (IDictionary)Data;
            foreach (object key in dict.Keys)
            {
                children.Add(new KeyValueItem(key, this, dict[key]));
            }
            _dataProcessed = true;
        }

        public override Type GetItemType()
        {
            if (itemType == null)
            {
                Type collType = Data.GetType();
                if (collType.GetInterface(typeof(IDictionary<,>).FullName) != null)
                {
                    Type IDict = collType.GetInterface(typeof(IDictionary<,>).FullName);
                    itemType = IDict.GetGenericArguments()[1];
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
            IDictionary dict = (IDictionary)Data;
            object newKey = null;
            if (_keyProperty != null && _keyProperty != string.Empty)
            {
                newKey = itemData.GetType().GetProperty(_keyProperty).GetValue(itemData, null);
            } else if (itemData is IUri)
            {
                newKey = ((IUri)itemData).Uri;
            }
            else
            {
                newKey = itemData.ToString();
            }

            switch (changeType)
            {
                case ChangeType.Add:
                    // should fail if it already exists
                    dict.Add(newKey, itemData);
                    child = new KeyValueItem(newKey, this, itemData);
                    children.Add(child);
                    this.OnStructureChanged();
                    break;
                case ChangeType.Edit:
                    KeyValueItem keyChild = child as KeyValueItem;
                    keyChild.Data = itemData;
                    if (!keyChild.Key.Equals(newKey))
                    {
                        // changed the key
                        if (dict.Contains(newKey))
                        {
                            throw new ArgumentException("Key " + newKey + " already exists in the list");
                        }
                        dict.Remove(keyChild.Key);
                        keyChild.Key = newKey;
                        dict.Add(newKey, itemData);
                        this.OnDataChanged(keyChild.CreatePath(), false);
                    }
                    
                    break;
            }
            child.SetDirty();
        }

    }
}
