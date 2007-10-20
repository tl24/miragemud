using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using Mirage.Core.Communication;

namespace Mirage.Core.Data
{
    public class MudRepositoryBase : BaseData
    {
        private ICollection<IPlayer> _players;
        private IDictionary<string, IArea> _areas;
        private ICollection<Channel> _channels;

        /// <summary>
        /// Creates the mud repository.  NOTE: this is not meant to be called directly.
        /// This class should be accessed through MudFactory.GetObject<MudRepositoryBase>();
        /// </summary>
        public MudRepositoryBase()
            : base()
        {
            _uri = "global";
            _players = new LinkedList<IPlayer>();
            _areas = new Dictionary<string, IArea>();
            _channels = new List<Channel>();
            _uriChildCollections.Add("Players", new BaseData.ChildCollectionPair(_players, QueryHints.DefaultPartialMatch));
            _uriChildCollections.Add("Areas", new BaseData.ChildCollectionPair(_areas, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems));
        }

        public ICollection<IPlayer> Players
        {
            get { return this._players; }
        }

        public void AddPlayer(IPlayer p)
        {
            this._players.Add(p);
            p.PlayerEvent += new PlayerEventHandler(OnPlayerEvent);
        }

        public void RemovePlayer(IPlayer p)
        {
            this._players.Remove(p);            
            p.PlayerEvent -= OnPlayerEvent;
        }

        private void OnPlayerEvent(object sender, PlayerEventArgs eventArgs)
        {
            IPlayer player = (IPlayer)sender;
            if (eventArgs.EventType == PlayerEventType.Quiting)
            {
                RemovePlayer(player);
            }
        }

        public IDictionary<string, IArea> Areas
        {
            get { return this._areas; }
        }

        public ICollection<Channel> Channels
        {
            get { return this._channels; }
            set { _channels = value; }
        }
    }
}
