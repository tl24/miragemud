using System;
using System.Collections;
using System.Collections.Generic;
using JsonExSerializer;
using Mirage.Game.Communication;
using Mirage.Game.World.Containers;
using Mirage.Game.World.Items;
using Mirage.Game.World.Query;
using Mirage.Core.Messaging;

namespace Mirage.Game.World
{
    public class Room : ViewableBase, IContainer, IReceiveMessages
    {
        private Area _area;
        private IDictionary<DirectionType, RoomExit> _exits;
        private HeterogenousContainer _containerHelper;

        public Room()
            : base()
        {
            var livingThings = new GenericCollectionContainer<Living>(new LinkedList<Living>(), this);
            livingThings.ItemAdded += new EventHandler<CollectionModifiedEventArgs>(_livingThings_ItemAdded);
            livingThings.ItemRemoved += new EventHandler<CollectionModifiedEventArgs>(_livingThings_ItemRemoved);
            LivingThings = livingThings;
            Items = new GenericCollectionContainer<ItemBase>(new LinkedList<ItemBase>(), this);

            _containerHelper = new HeterogenousContainer();
            _containerHelper.AddContainer(LivingThings);
            _containerHelper.AddContainer(Items);

            _exits = new Dictionary<DirectionType, RoomExit>();
        }

        void _livingThings_ItemAdded(object sender, CollectionModifiedEventArgs e)
        {
            if (e.Item is Player)
                ((Player)e.Item).PlayerEvent += new PlayerEventHandler(player_PlayerEvent);
        }

        void _livingThings_ItemRemoved(object sender, CollectionModifiedEventArgs e)
        {
            if (e.Item is Player)
                ((Player)e.Item).PlayerEvent -= player_PlayerEvent;
        }

        [EditorParent(2)]
        public Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }

        [JsonExIgnore]
        public IContainer<Living> LivingThings  { get; private set; }

        [JsonExIgnore]
        public IContainer<ItemBase> Items { get; private set; }

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
            if (player.Room != null)
            {
                player.Room.Remove(player);
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

        int IContainer.Count
        {
            get
            {
                return _containerHelper.Count;
            }
        }

        public void Add(object item)
        {
            _containerHelper.Add(item);
        }

        public void Remove(object item)
        {
            _containerHelper.Remove(item);
        }

        public bool Contains(object item)
        {
            return _containerHelper.Contains(item);
        }

        public bool CanAdd(object item)
        {
            return _containerHelper.CanAdd(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _containerHelper.GetEnumerator();
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
