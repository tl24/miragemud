using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mirage.Game.Communication;
using MirageGUI.Code;
using Mirage.Game.World;
using Mirage.Game.Communication.BuilderMessages;

namespace MirageGUI.Controls
{
    public class RootItem : CollectionItem
    {
        private AreaTreeModel model;

        public RootItem(AreaTreeModel model)
            : base(null, null, "Areas", typeof(Area), null)
        {
            this.model = model;
        }

        /// <summary>
        /// Gets the model associated with this item
        /// </summary>
        public AreaTreeModel Model
        {
            get { return model; }
        }

        public override IEnumerable GetChildren(TreePath path)
        {
            if (IsPath(path))
            {
                if (!_loaded)
                    Model.IOHandler.SendString("GetAreas Areas");
                return children;
            }
            else
            {
                return base.GetChildren(path);
            }
        }

        public override int GetChildCount(TreePath path)
        {
            if (IsPath(path)) {
                return _loaded ? children.Count : -1;
            } else {
                return base.GetChildCount(path);
            }
        }

        protected override void ProcessData()
        {
            
        }

        public override Type GetItemType()
        {
            return typeof(Area);
        }

        public override MirageGUI.Code.ProcessStatus HandleResponse(Mirage.Core.Messaging.Message response)
        {
            if (response.IsMatch(Namespaces.Area, "AreaList"))
            {
                DataMessage dm = response as DataMessage;
                foreach (string areaName in (List<string>)dm.Data)
                {
                    children.Add(new LeafItem(this, areaName));
                }
                _loaded = true;
                OnStructureChanged();
                foreach (string areaName in (List<string>)dm.Data)
                {
                    Model.IOHandler.SendString("GetArea Areas/" + areaName);
                }
                return ProcessStatus.SuccessAbort;
            }
            else if (response.IsMatch(Namespaces.Area, "Area"))
            {
                DataMessage dm = response as DataMessage;
                Area area = (Area) dm.Data;
                string name = area.Uri;
                bool found = false;
                AreaItem areaItem = new AreaItem(this, area, false);
                for (int i = 0; i < children.Count; i++)
                {
                    object o = children[i];
                    if (o.ToString() == name)
                    {
                        children[i] = areaItem;
                        found = true;
                    }
                }
                if (!found)
                    children.Add(areaItem);

                //OnStructureChanged(CreatePath().Append(areaItem));
                OnStructureChanged();
            }
            else if (response.IsMatch(Namespaces.Area, "AreaAdded")
                || response.IsMatch(Namespaces.Area, "AreaUpdated"))
            {
                UpdateConfirmationMessage msg = (UpdateConfirmationMessage)response;

                for (int i = 0; i < children.Count; i++)
                {
                    AreaItem aItem = children[i] as AreaItem;
                    if (aItem != null && aItem.Data.FullUri == msg.ItemUri)
                    {
                        aItem.ClearDirty();
                        break;
                    }
                }
                return ProcessStatus.SuccessContinue;   // let the console pick it up as a confirmation
            }
            return base.HandleResponse(response);
        }

        public override void SetDirty()
        {
            
        }
    }

    public class LeafItem : BaseItem {

        private object[] children = new object[0];
        private object data;

        public LeafItem(BaseItem Parent, object data) : base(Parent) {
            this.data = data;
        }
        public override IEnumerable GetChildren(TreePath path)
        {
            return children;
        }

        public override int GetChildCount(TreePath path)
        {
            return 0;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public override void UpdateChild(BaseItem child, object data, ChangeType changeType)
        {
            // no children
        }
    }
}
