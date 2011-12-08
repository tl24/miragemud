using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer;
using System.Collections;
using Mirage.Game.World.Query;
using Mirage.Game.Command;
using Mirage.Game.World;
using Mirage.Game.Communication.BuilderMessages;

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

                    BaseItem newChild;

                    if (DictionaryItem.IsType(_dataType, prop, this, Data, out newChild))
                        children.Add(newChild);
                    else if (CollectionItem.IsType(_dataType, prop, this, Data, out newChild))
                        children.Add(newChild);
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
