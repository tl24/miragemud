using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using System.Collections;
using JsonExSerializer;
using Mirage.Core.Data;
using Mirage.Core.Data.Items;
using Mirage.Core.Data.Containers;
using Mirage.Core.Communication;

namespace Mirage.Core.Data
{
    public class Room : ViewableBase, IContainer, IReceiveMessages
    {
        private Area _area;
        private LinkedList<Living> _livingThings;
        private IDictionary<DirectionType, RoomExit> _exits;
        private LinkedList<ItemBase> _items;

        public Room()
            : base()
        {
            _livingThings = new LinkedList<Living>();
            _items = new LinkedList<ItemBase>();
            _uriChildCollections.Add("LivingThings", new BaseData.ChildCollectionPair(_livingThings, QueryHints.DefaultPartialMatch));
            _uriChildCollections.Add("Items", new BaseData.ChildCollectionPair(_items, QueryHints.DefaultPartialMatch));
            _exits = new Dictionary<DirectionType, RoomExit>();
        }

        [EditorParent(2)]
        public Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }

        [JsonExIgnore]
        public ICollection<Living> LivingThings
        {
            get { return this._livingThings; }
        }

        [JsonExIgnore]
        public ICollection<ItemBase> Items {
            get { return this._items; }
        }

        public override string FullUri
        {
            get
            {
                if (this._area != null)
                    return this._area.FullUri + "/Rooms/" + this.Uri;
                else
                    return this.Uri;
            }
        }

        void player_PlayerEvent(object sender, PlayerEventArgs eventArgs)
        {
            Player player = (Player)sender;
            if (player.Container != null)
            {
                player.Container.Remove(player);
            }
        }

        [EditorCollection(typeof(RoomExit), KeyProperty="Direction")]
        public IDictionary<DirectionType, RoomExit> Exits
        {
            get { return this._exits; }
            set { this._exits = value; }
        }

        public void CopyTo(Room newRoom)
        {
            if (this != newRoom)
            {
                // read from a copy so we can modify the list inside the loop
                foreach (Living living in new List<Living>(LivingThings))
                {
                    //TODO: Catch exceptions and exit gracefully?
                    ContainerUtils.Transfer(living, newRoom);
                }
            }
        }

        
        #region IContainer Members

        public void Add(IContainable item)
        {
            if (CanAdd(item))
            {
                if (item is Living)
                {
                    if (item.Container != this || !this._livingThings.Contains((Living)item))
                    {
                        this._livingThings.AddLast((Living)item);
                        item.Container = this;
                        if (item is Player)
                            ((Player)item).PlayerEvent += new PlayerEventHandler(player_PlayerEvent);

                    }
                }
                else if (item is ItemBase)
                {
                    if (item.Container != this || !this._items.Contains((ItemBase)item))
                    {
                        this._items.AddLast((ItemBase)item);
                        item.Container = this;

                    }
                }
            }
            else
            {
                throw new ContainerAddException("item could not be added to the room", this, item);
            }
        }

        public void Remove(IContainable item)
        {
            if (CanAdd(item))
            {
                if (item is Living)
                {
                    this._livingThings.Remove((Living)item);
                    if (item.Container == this)
                        item.Container = null;

                    if (item is Player)
                        ((Player)item).PlayerEvent -= player_PlayerEvent;
                }
                else if (item is ItemBase)
                {
                    this._items.Remove((ItemBase)item);
                    if (item.Container == this)
                        item.Container = null;
                }
            }
        }

        public bool Contains(IContainable item)
        {
            if (item is Living)
                return this._livingThings.Contains((Living)item);
            else if (item is ItemBase)
                return this._items.Contains((ItemBase) item);
            else
                return false;
        }

        public bool CanContain(Type item)
        {
            return typeof(Living).IsAssignableFrom(item)
                || typeof(ItemBase).IsAssignableFrom(item);
        }

        public bool CanAdd(IContainable item)
        {
            return (item is Living) || (item is ItemBase);
        }

        public IEnumerable Contents(Type t)
        {
            if (typeof(Living).IsAssignableFrom(t))
            {
                return _livingThings;
            }
            else if (typeof(ItemBase).IsAssignableFrom(t)) {
                return _items;
            }
            else
            {
                return new List<object>();
            }
        }

        public System.Collections.Generic.IEnumerable<T> Contents<T>()
        {
            foreach (object item in this.Contents(typeof(T)))
            {
                yield return (T)item;
            }
        }

        public IEnumerable Contents()
        {
            throw new Exception("Not Implemented");
        }
        #endregion      
    
        #region IReceiveMessages Members

        public void Write(IMessage message)
        {
            Write(null, message);
        }

        public void Write(object sender, IMessage message)
        {
            foreach (Living living in this.LivingThings)
            {
                if (living != sender)
                    living.Write(sender, message);
            }
        }

        #endregion
    }

}
