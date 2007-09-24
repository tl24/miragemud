using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mirage.Core.Command;
using Mirage.Core.Data.Query;
using System.Reflection;
using Mirage.Core.Data;

namespace MirageGUI.Controls
{
    public class CollectionItem : KeyValueItem
    {
        protected Type itemType;
        protected PropertyInfo property;

        public CollectionItem(BaseItem parent, object data, string name, Type itemType, PropertyInfo property)
            : base(name, parent, data)
        {
            this.itemType = itemType;
            this.property = property;
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

        public virtual TypeSelectionArgs GetTypeSelectionArgs()
        {
            EditorTypeSelectorAttribute attr = ReflectionUtils.GetSingleAttribute<EditorTypeSelectorAttribute>(property);
            if (attr != null)
            {
                if (attr.BaseType != null)
                    return new TypeSelectionArgs(attr.BaseType, attr.DefaultNamespace);
                else
                    return new TypeSelectionArgs(attr.BaseTypeName, attr.DefaultNamespace);
            }
            else
            {
                return new TypeSelectionArgs();
            }
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
                    OnNewItem(child, itemData);
                    children.Add(child);
                    this.OnStructureChanged();
                    child.SetDirty();
                    break;
                case ChangeType.Edit:
                    child.Data = itemData;
                    child.SetDirty();
                    break;
                case ChangeType.Delete:
                    children.Remove(child);
                    ((IList)Data).Remove(child.Data);
                    this.OnStructureChanged();
                    this.SetDirty();
                    break;
            }
        }

        protected virtual void OnNewItem(BaseItem item, object data) {
            Type dataType = data.GetType();
            foreach (PropertyInfo prop in dataType.GetProperties())
            {
                 EditorParentAttribute parentAttr = ReflectionUtils.GetSingleAttribute<EditorParentAttribute>(prop);
                 if (parentAttr != null) {
                     BaseItem pItem = item;
                     int i = 0;
                     while (pItem != null && i < parentAttr.ParentLevel)
                     {
                         pItem = pItem.Parent;
                         i++;
                     }

                     if (pItem != null)
                         prop.SetValue(data, pItem.Data, null);
                 }
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
            if (property.PropertyType != typeof(string) && property.PropertyType.GetInterface("IEnumerable", true) != null)
            {
                Type itemType = null;
                if (attr != null)
                {
                    itemType = attr.ItemType;
                }
                newItem = new CollectionItem(parent, property.GetValue(ParentData, null), property.Name, itemType, property);
                return true;
            }
            return false;
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

    public class TypeSelectionArgs
    {
        private Type _baseType;
        private string _defaultNamespace;

        public TypeSelectionArgs() {
        }

        public TypeSelectionArgs(Type baseType, string defaultNamespace)
        {
            _baseType = baseType;
            _defaultNamespace = defaultNamespace;
        }

        public TypeSelectionArgs(string baseType, string defaultNamespace)
        {
            if (baseType != null && baseType != string.Empty)
                _baseType = Type.GetType(baseType);

            _defaultNamespace = defaultNamespace;
        }

        public Type BaseType
        {
            get { return this._baseType; }
        }

        public string DefaultNamespace
        {
            get { return this._defaultNamespace; }
        }

        
    }
}
