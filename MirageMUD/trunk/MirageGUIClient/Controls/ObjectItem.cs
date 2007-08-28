using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer;
using System.Collections;
using Mirage.Data.Query;
using Mirage.Command;
using Mirage.Data;

namespace MirageGUI.Controls
{
    public class ObjectItem : BaseItem
    {
        protected bool _loaded = false;
        protected bool _dataProcessed = false;

        
        protected List<BaseItem> children;

        public ObjectItem(BaseItem parent, object data)
            : base(parent)
        {
            _data = data;
            _loaded = _data != null;
            children = new List<BaseItem>();
        }

        
        public override System.Collections.IEnumerable GetChildren(TreePath path)
        {
            if (IsPath(path))
            {
                if (_loaded && !_dataProcessed)
                    ProcessData();
                return children;
            }
            else
            {
                if (path.Count > Level + 1)
                {
                    object searched = path.FullPath[Level + 1];
                    foreach (BaseItem o in children)
                    {
                        if (o.Equals(searched))
                        {
                            return o.GetChildren(path);
                        }
                    }
                }
                return new object[0];
            }
        }

        public override int GetChildCount(TreePath path)
        {
            if (IsPath(path))
            {
                if (!_loaded)
                {
                    return -1;
                }
                else
                {
                    if (!_dataProcessed)
                        ProcessData();
                    return children.Count;
                }
            }
            else
            {
                if (path.Count > Level + 1)
                {
                    object searched = path.FullPath[Level + 1];
                    foreach (BaseItem o in children)
                    {
                        if (o.Equals(searched))
                        {
                            return o.GetChildCount(path);
                        }
                    }
                }
                return 0;
            }
        }

        public override object Data
        {
            get { return base.Data; }
            set
            {
                base.Data = value;
                if (_dataProcessed)
                {
                    _dataProcessed = false;
                    children.Clear();
                    OnStructureChanged();
                }
            }
        }

        public override void UpdateChild(BaseItem child, object data, ChangeType changeType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void ProcessData()
        {
            Type _dataType = _data.GetType();
            foreach (PropertyInfo prop in _dataType.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (prop.IsDefined(typeof(JsonExIgnoreAttribute), false))
                        continue;

                    EditorCollectionAttribute attr = ReflectionUtils.GetSingleAttribute<EditorCollectionAttribute>(prop);
                    if (typeof(IDictionary).IsAssignableFrom(prop.PropertyType)
                        || prop.PropertyType.GetInterface(typeof(IDictionary<,>).FullName, false) != null
                        || (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                    {
                        string keyProp = null;
                        Type itemType = null;
                        if (attr != null)
                        {
                            keyProp = attr.KeyProperty;
                            itemType = attr.ItemType;
                        }
                        children.Add(new DictionaryItem(this, prop.GetValue(_data, null), prop.Name, itemType, keyProp));
                    }
                    else if (prop.PropertyType != typeof(string) && prop.PropertyType.GetInterface("IEnumerable", true) != null)
                    {
                        Type itemType = null;
                        if (attr != null)
                        {
                            itemType = attr.ItemType;
                        }
                        children.Add(new CollectionItem(this, prop.GetValue(_data, null), prop.Name, itemType));
                    }
                }
            }
            _dataProcessed = true;
        }

        public override string ToString()
        {
            string display;
            if (_data is IUri)
                display = ((IUri)_data).Uri;
            else
                display = _data == null ? "" : _data.ToString();

            if (display != string.Empty && IsDirty)
                display += " *";

            return display;
        }

        public override void ClearDirty(bool sendEvent)
        {
            if (_isDirty != false)
            {
                foreach (BaseItem child in children) {
                    child.ClearDirty(false);
                }
            }
            base.ClearDirty(sendEvent);
        }
    }
}
