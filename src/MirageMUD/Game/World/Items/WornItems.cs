using System;
using System.Collections.Generic;

namespace Mirage.Game.World.Items
{
    /// <summary>
    /// Collection for worn items
    /// </summary>
    public class WornItems : ICollection<Armor>
    {
        private List<Armor> _items;
        private WearLocations wearFlags;

        public WornItems()
        {
            _items = new List<Armor>();
            wearFlags = (WearLocations)0;
        }

        /// <summary>
        /// Gets the first item found in the specified slot(s)
        /// </summary>
        /// <param name="location">the location to check</param>
        /// <returns>item found</returns>
        public Armor GetItemAt(WearLocations location)
        {
            foreach (Armor item in GetItemsAt(location))
                return item;

            return null;
        }

        public IEnumerable<Armor> GetItemsAt(WearLocations location)
        {
            if (IsOpen(location))
                yield break;
            foreach (Armor item in this)
            {
                if ((item.WearFlags & location) != 0)
                    yield return item;
            }
        }

        /// <summary>
        /// Returns true if the location(s) is open.  If location has multiple WearLocation flags set then
        /// all locations specified must be open.
        /// </summary>
        /// <param name="location">location to check</param>
        /// <returns>true if open</returns>
        public bool IsOpen(WearLocations location)
        {
            return (wearFlags & location) == 0;
        }

        #region ICollection<Armor> Members

        /// <summary>
        /// Adds an item to the worn collection
        /// </summary>
        /// <param name="item">item to add</param>
        /// <exception cref="InvalidOperationException">If an item is already worn in the slot the operation will fail</exception>
        public void Add(Armor item)
        {
            if (Contains(item))
                return;

            if (!IsOpen(item.WearFlags))
                throw new InvalidOperationException("Can't add item, an item is already worn in the same slot: " + item.WearFlags);

            _items.Add(item);
            wearFlags |= item.WearFlags;
        }

        public void Clear()
        {
            _items.Clear();
            wearFlags = (WearLocations)0;
        }

        public bool Contains(Armor item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Armor[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Armor item)
        {
            if (_items.Remove(item))
            {                
                wearFlags &= ~item.WearFlags;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at the existing location
        /// </summary>
        /// <param name="location">the location to remove from</param>
        /// <returns>the item that was at the location</returns>
        public Armor Remove(WearLocations location)
        {
            Armor item = GetItemAt(location);
            if (item != null)
                Remove(item);

            return item;
        }
        #endregion

        #region IEnumerable<Armor> Members

        public IEnumerator<Armor> GetEnumerator()
        {
            return _items.GetEnumerator();
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
