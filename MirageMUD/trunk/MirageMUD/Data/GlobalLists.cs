using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class GlobalLists : BaseData
    {
        private static GlobalLists _instance;
        private ICollection<Player> _players;
        private IDictionary<string, Area> _areas;

        public static GlobalLists GetInstance()
        {
            if (_instance == null)
            {
                lock (typeof(GlobalLists))
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalLists();
                    }
                }
            }
            return _instance;
        }

        private GlobalLists() : base()
        {
            _uri = "global";
            _players = new LinkedList<Player>();
            _areas = new Dictionary<string, Area>();
            _uriChildCollections.Add("Players", new BaseData.ChildCollectionPair(_players, QueryHints.DefaultPartialMatch));
            _uriChildCollections.Add("Areas", new BaseData.ChildCollectionPair(_areas, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems));
        }

        public ICollection<Player> Players
        {
            get { return this._players; }
        }

        public void AddPlayer(Player p)
        {
            this._players.Add(p);
            p.PlayerEvent += new Player.PlayerEventHandler(OnPlayerEvent);
        }

        public void RemovePlayer(Player p)
        {
            this._players.Remove(p);            
            p.PlayerEvent -= OnPlayerEvent;
        }

        private void OnPlayerEvent(object sender, Player.PlayerEventArgs eventArgs)
        {
            Player player = (Player)sender;
            if (eventArgs.EventType == Player.PlayerEventType.Quiting)
            {
                RemovePlayer(player);
            }
        }

        public IDictionary<string, Area> Areas
        {
            get { return this._areas; }
        }


    }
}
