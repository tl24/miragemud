using System.Collections.Generic;
using System.IO;
using Mirage.Core.IO.Serialization;

namespace Mirage.Game.World
{
    public class AreaRepository<T> : IAreaRepository
    {
        private MudWorld mudRepository;
        private IPersistenceManager persistenceManager;

        public AreaRepository(MudWorld mudRepository)
        {
            this.mudRepository = mudRepository;
            persistenceManager = ObjectStorageFactory.GetPersistenceManager(typeof(T));
        }

        #region IAreaRepository Members

        public IArea Load(string areaUri)
        {
            try
            {
                IArea area = (IArea)persistenceManager.Load(areaUri);
                Add(area);
                return area;

            } catch (FileNotFoundException)
            {
                return null;
            }
        }

        public void LoadAll()
        {
            // No area file at the moment
        }

        public void Save(IArea area)
        {
            persistenceManager.Save(area, area.Uri);
        }

        public void Save(string areaUri)
        {
            Save(Areas[areaUri]);
        }

        public void Add(IArea area)
        {
            Areas[area.Uri] = area;
        }

        public void Update(IArea area)
        {
            Areas[area.Uri] = area;
        }

        public void Remove(IArea area)
        {
            Areas.Remove(area.Uri);
        }

        public IDictionary<string, IArea> Areas
        {
            get { return mudRepository.Areas; }
        }

        #endregion

        #region IEnumerable<IArea> Members

        public IEnumerator<IArea> GetEnumerator()
        {
            return Areas.Values.GetEnumerator();
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
