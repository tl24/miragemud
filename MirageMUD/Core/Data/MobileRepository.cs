using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;

namespace Mirage.Core.Data
{
    public class MobileRepository : IMobileRepository
    {
        private StockRepository _mudRepository;
        public MobileRepository(MudRepositoryBase MudRepository)
        {
            _mudRepository = (StockRepository)MudRepository;
        }

        #region IMobileRepository Members

        public ICollection<Mobile> Mobiles
        {
            get { return _mudRepository.Mobiles; }
        }

        #endregion

        #region IEnumerable<Mobile> Members

        public IEnumerator<Mobile> GetEnumerator()
        {
            return Mobiles.GetEnumerator();
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
