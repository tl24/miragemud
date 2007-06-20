using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;
using System.Collections;
using JsonExSerializer;

namespace Shoop.Data
{
    public class Room : BaseData, IViewable, IContainer
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private Area _area;
        private LinkedList<Animate> _animates;
        private IDictionary<DirectionType, RoomExit> _exits;

        public Room()
            : base()
        {
            _animates = new LinkedList<Animate>();
            _uriChildCollections.Add("Animates", new BaseData.ChildCollectionPair(_animates, QueryHints.DefaultPartialMatch));
            _exits = new Dictionary<DirectionType, RoomExit>();
        }

        [JsonExIgnore]
        public Shoop.Data.Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }

        public string LongDescription
        {
            get { return this._longDescription; }
            set { this._longDescription = value; }
        }

        public string ShortDescription
        {
            get { return this._shortDescription; }
            set { this._shortDescription = value; }
        }

        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        [JsonExIgnore]
        public ICollection<Animate> Animates
        {
            get { return this._animates; }
        }

        [JsonExIgnore]
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

        void player_PlayerEvent(object sender, Player.PlayerEventArgs eventArgs)
        {
            Player player = (Player)sender;
            if (player.Container != null)
            {
                player.Container.Remove(player);
            }
        }

        public IDictionary<DirectionType, RoomExit> Exits
        {
            get { return this._exits; }
            set { this._exits = value; }
        }

        #region IContainer Members

        public void Add(IContainable item)
        {
            
            if (CanAdd(item))
            {
                if (item.Container != this || !this._animates.Contains((Animate) item))
                {
                    this._animates.AddLast((Animate) item);
                    item.Container = this;
                    if (item is Player)
                        ((Player)item).PlayerEvent += new Player.PlayerEventHandler(player_PlayerEvent);

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
                this._animates.Remove((Animate) item);
                if (item.Container == this)
                    item.Container = null;

                if (item is Player)
                    ((Player)item).PlayerEvent -= player_PlayerEvent;
            }
        }

        public bool Contains(IContainable item)
        {
            if (item is Animate)
                return this._animates.Contains((Animate)item);
            else
                return false;
        }

        public bool CanContain(Type item)
        {
            return typeof(Animate).IsAssignableFrom(item);
        }

        public bool CanAdd(IContainable item)
        {
            return (item is Animate);
        }

        public IEnumerable Contents(Type t)
        {
            if (t == typeof(Animate))
            {
                return _animates;
            }
            else
            {
                return new List<object>();
            }
        }

        #endregion      
    }

}
