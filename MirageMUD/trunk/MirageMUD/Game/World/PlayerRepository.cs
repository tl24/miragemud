using System;
using System.Collections.Generic;
using System.Text;
using Mirage.IO.Serialization;
using System.IO;
using Castle.Core;

namespace Mirage.Game.World
{
    [CastleComponent("PlayerRepository", typeof(IPlayerRepository), LifestyleType.Singleton)]
    public class PlayerRepository<T> : IPlayerRepository
    {
        private IPersistenceManager persistenceManager;
        private MudRepositoryBase mudRepository;

        public PlayerRepository(MudRepositoryBase mudRepository)
        {
            persistenceManager = ObjectStorageFactory.GetPersistenceManager(typeof(T));
            this.mudRepository = mudRepository;
        }

        #region IPlayerRepository Members

        public IPlayer Load(string playerUri)
        {
            try
            {
                return (IPlayer)persistenceManager.Load(playerUri);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public void Save(IPlayer player)
        {
            persistenceManager.Save(player, player.Uri);
        }

        public void Add(IPlayer player)
        {
            // just use the repository for now
            mudRepository.AddPlayer(player);
        }

        public void Remove(IPlayer player)
        {
            mudRepository.RemovePlayer(player);
        }

        public IPlayer Find(string playerUri)
        {
            return Find(playerUri, false);
        }

        public IPlayer Find(string playerUri, bool loadIfNotFound)
        {
            foreach (IPlayer p in this)
            {
                if (p.Uri.Equals(playerUri, StringComparison.CurrentCultureIgnoreCase))
                    return p;
            }
            if (loadIfNotFound)
                return Load(playerUri);
            else
                return null;
        }

        #endregion

        #region IEnumerable<IPlayer> Members

        public IEnumerator<IPlayer> GetEnumerator()
        {
            return mudRepository.Players.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
