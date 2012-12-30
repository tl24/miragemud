using System.Collections.Generic;

namespace Mirage.Game.World
{
    public class MobileRepository : IMobileRepository
    {
        private MudWorld _mudRepository;
        public MobileRepository(MudWorld MudRepository)
        {
            _mudRepository = MudRepository;
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
