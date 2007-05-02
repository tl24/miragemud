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
            _uriProperties.Add("Players", new QueryableCollectionAdapter<Player>(_players, "Players", QueryCollectionFlags.DefaultPartialMatch));
            _uriProperties.Add("Areas", new QueryableDictionaryAdapter<Area>("Areas", _areas));
        }

        public ICollection<Player> Players
        {
            get { return this._players; }
        }

        public IDictionary<string, Area> Areas
        {
            get { return this._areas; }
        }


    }
}
