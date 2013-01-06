using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core;
using Mirage.IO.Serialization;

namespace Mirage.Game.World
{
    [CastleComponent("PlayerRepository", LifestyleType.Singleton, typeof(IPlayerRepository))]
    public class PlayerRepository<T> : IPlayerRepository
    {
        private IPersistenceManager persistenceManager;
        private MudWorld mudRepository;

        public PlayerRepository(MudWorld mudRepository)
        {
            persistenceManager = ObjectStorageFactory.GetPersistenceManager(typeof(T));
            this.mudRepository = mudRepository;
        }

        #region IPlayerRepository Members

        public IPlayer Load(string name)
        {
            try
            {
                return (IPlayer)persistenceManager.Load(name);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public void Save(IPlayer player)
        {
            persistenceManager.Save(player, player.Name);
        }

        public void Add(IPlayer player)
        {
            // just use the repository for now
            mudRepository.AddPlayer((Player)player);
        }

        public void Remove(IPlayer player)
        {
            mudRepository.RemovePlayer((Player)player);
        }

        public IPlayer Find(string name)
        {
            return Find(name, false);
        }

        public IPlayer Find(string name, bool loadIfNotFound)
        {
            foreach (IPlayer p in this)
            {
                if (p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return p;
            }
            if (loadIfNotFound)
                return Load(name);
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
