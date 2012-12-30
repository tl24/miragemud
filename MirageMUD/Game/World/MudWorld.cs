using System.Collections.Generic;
using JsonExSerializer;
using Mirage.Game.World.Query;
using Mirage.Game.Communication;

namespace Mirage.Game.World
{
    /// <summary>
    /// Facade class for accessing most global collections and commonly used objects
    /// </summary>
    public class MudWorld : BaseData
    {
        private ObjectUriResolver _resolver;

        /// <summary>
        /// Creates the mud repository.  NOTE: this is not meant to be called directly.
        /// This class should be accessed through MudFactory.GetObject<MudRepositoryBase>();
        /// </summary>
        public MudWorld()
            : base()
        {
            _uri = "global";
            Players = new LinkedList<IPlayer>();
            Areas = new Dictionary<string, IArea>();
            RegisterQueryableObject("Players", Players, QueryHints.DefaultPartialMatch);
            RegisterQueryableObject("Areas", Areas, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            Mobiles = new LinkedList<Mobile>();
            _uriChildCollections.Add("Mobiles", new ChildCollectionPair(Mobiles, QueryHints.DefaultPartialMatch));
            _resolver = new ObjectUriResolver(this);
        }

        /// <summary>
        /// The list of actively playing Players
        /// </summary>
        public ICollection<IPlayer> Players { get; private set; }

        /// <summary>
        /// Loaded areas
        /// </summary>
        public IDictionary<string, IArea> Areas { get; private set; }

        /// <summary>
        /// Mud channels
        /// </summary>
        public IChannelRepository Channels { get; set; }

        public ICollection<Mobile> Mobiles { get; private set; }

        public IRaceRepository Races { get; set; }

        /// <summary>
        /// Adds a player to the list of active players
        /// </summary>
        /// <param name="p"></param>
        public void AddPlayer(IPlayer p)
        {
            Players.Add(p);
            p.PlayerEvent += new PlayerEventHandler(OnPlayerEvent);
        }

        /// <summary>
        /// Removes a player from the list of active players
        /// </summary>
        /// <param name="p"></param>
        public void RemovePlayer(IPlayer p)
        {
            Players.Remove(p);            
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

        #region UriSupport
        /// <summary>
        /// Resolves the object for <paramref name="uri"/>
        /// </summary>
        /// <param name="uri">object uri</param>
        /// <returns>the object identified by the uri</returns>
        public object ResolveUri(string uri)
        {
            return _resolver.Resolve(uri);
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>
        /// </summary>
        /// <param name="uri">object uri</param>
        /// <returns>the object identified by the uri</returns>
        public object ResolveUri(ObjectUri uri)
        {
            return _resolver.Resolve(uri);
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>, using
        /// <paramref name="root"/> as the starting point for relative uris.
        /// </summary>
        /// <param name="root">the root object for relative uris</param>
        /// <param name="uri">the uri to resolve</param>
        /// <returns>the object identified by the uri</returns>
        public object ResolveUri(object root, ObjectUri uri)
        {
            return _resolver.Resolve(root, uri);
        }

        /// <summary>
        /// Resolves the object for <paramref name="uri"/>, using
        /// <paramref name="root"/> as the starting point for relative uris.
        /// </summary>
        /// <param name="root">the root object for relative uris</param>
        /// <param name="uri">the uri to resolve</param>
        /// <returns>the object identified by the uri</returns>
        public object ResolveUri(object root, string uri)
        {
            return _resolver.Resolve(root, uri);
        }

        #endregion

        /// <summary>
        /// Registers an object to be exposed to the Uri Query system
        /// </summary>
        /// <param name="ObjectUri">The Uri for the collection</param>
        /// <param name="QueryableObject">The collection</param>
        /// <param name="Hints">Any query hints for the query manager</param>
        public void RegisterQueryableObject(string ObjectUri, object QueryableObject, QueryHints Hints)
        {
            _uriChildCollections.Add(ObjectUri, new ChildCollectionPair(QueryableObject, Hints));
        }

        /// <summary>
        /// Registers an object to be exposed to the Uri Query system
        /// </summary>
        /// <param name="ObjectUri">The Uri for the collection</param>
        /// <param name="QueryableObject">The collection</param>
        public void RegisterQueryableObject(string ObjectUri, object QueryableObject)
        {
            RegisterQueryableObject(ObjectUri, QueryableObject, 0);
        }

    }
}
