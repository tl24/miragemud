using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class Room : BaseData, IViewable
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private Area _area;
        private LinkedList<Animate> _animates;

        public Room()
            : base()
        {
            _animates = new LinkedList<Animate>();
            _uriProperties.Add("Animates", new QueryableCollectionAdapter<Animate>(_animates, "Animates"));
        }

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

        public ICollection<Animate> Animates
        {
            get { return this._animates; }
        }

        public override string FullURI
        {
            get
            {
                if (this._area != null)
                    return this._area.FullURI + "/Rooms/" + this.URI;
                else
                    return this.URI;
            }
        }

        public void MoveTo(Player player)
        {
            if (player.Room != this || !this._animates.Contains(player))
            {
                if (player.Room != null && player.Room != this)
                    player.Room.MoveFrom(player);

                this._animates.AddLast(player);
                player.Room = this;
                player.PlayerEvent += new Player.PlayerEventHandler(player_PlayerEvent);
    
            }
        }

        void player_PlayerEvent(object sender, Player.PlayerEventArgs eventArgs)
        {
            Player player = (Player)sender;
            if (player.Room != null)
            {
                player.Room.MoveFrom(player);
            }
        }

        /// <summary>
        /// Moves the player from this room
        /// </summary>
        /// <param name="player">The player to be removed</param>
        private void MoveFrom(Player player)
        {
            this._animates.Remove(player);
            if (player.Room == this)
                player.Room = null;

            player.PlayerEvent -= player_PlayerEvent;
        }


    }

}
