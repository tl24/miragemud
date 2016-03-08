using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mirage.Game.Command;
using Mirage.Game.World.Query;
using System.Reflection;
using Mirage.Game.World;
using Mirage.Game.Communication.BuilderMessages;

namespace MirageGUI.Controls
{
    public class DictionaryItem : CollectionItem
    {
        private string _keyProperty;

        public DictionaryItem(BaseItem parent, object data, string name, Type itemType, string keyProp, PropertyInfo property)
            : base(parent, data, name, itemType, property)
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
            } else if (itemData is ISupportUri)
            {
                newKey = ((ISupportUri)itemData).Uri;
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
                    OnNewItem(child, itemData);
                    children.Add(child);
                    this.OnStructureChanged();
                    child.SetDirty();
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
                    child.SetDirty();
                    break;
                case ChangeType.Delete:
                    children.Remove(child);
                    dict.Remove(newKey);
                    this.OnStructureChanged();
                    this.SetDirty();
                    break;
            }
        }

        /// <summary>
        /// Check to see if the property for the object can be handled by this class,
        /// if so create a new instance and return through the newItem parameter;
        /// </summary>
        /// <param name="ParentType">The parent type for the property</param>
        /// <param name="property">the property</param>
        /// <param name="parent">the parent node</param>
        /// <param name="ParentData">data for the parent node</param>
        /// <param name="newItem">the new item</param>
        /// <returns>true if the new item was created, false if this class does not handle it</returns>
        public static bool IsType(Type ParentType, PropertyInfo property, BaseItem parent, object ParentData, out BaseItem newItem)
        {
            newItem = null;
            EditorCollectionAttribute attr = ReflectionUtils.GetSingleAttribute<EditorCollectionAttribute>(property);
            if (typeof(IDictionary).IsAssignableFrom(property.PropertyType)
                || property.PropertyType.GetInterface(typeof(IDictionary<,>).FullName, false) != null
                || (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                string keyProp = null;
                Type itemType = null;
                if (attr != null)
                {
                    keyProp = attr.KeyProperty;
                    itemType = attr.ItemType;
                }
                newItem = new DictionaryItem(parent, property.GetValue(ParentData, null), property.Name, itemType, keyProp, property);
                return true;
            }
            return false;
        }
    }
}
