using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;

namespace Mirage.Game.World
{
    public interface IPlayerRepository : IEnumerable<IPlayer>
    {
        /// <summary>
        /// Loads a player object from persistent storage
        /// </summary>
        /// <param name="playerUri">the uri for the player to load</param>
        /// <returns>the player object if a player exists with the specified Uri or null if it doesn't exist</returns>
        IPlayer Load(string playerUri);

        /// <summary>
        /// Saves changes made to the player to persistent storage
        /// </summary>
        /// <param name="player"></param>
        void Save(IPlayer player);

        /// <summary>
        /// Stores the player in the list of currently playing players
        /// </summary>
        /// <param name="player"></param>
        void Add(IPlayer player);

        /// <summary>
        /// Removes the player from the list of currently playing players
        /// </summary>
        /// <param name="player"></param>
        void Remove(IPlayer player);

        /// <summary>
        /// Finds a player that is currently playing
        /// </summary>
        /// <param name="playerUri">the uri of the player to load</param>
        /// <returns>the player if found</returns>
        IPlayer Find(string playerUri);

        /// <summary>
        /// Finds a player that is currently playing.  If the player is not playing and the loadIfNotFound
        /// flag is set then the player will be loaded from storage.
        /// </summary>
        /// <param name="playerUri"></param>
        /// <param name="loadIfNotFound"></param>
        /// <returns></returns>
        IPlayer Find(string playerUri, bool loadIfNotFound);

        
    }
}
