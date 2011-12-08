using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World;
using Mirage.Game.World.Query;

namespace Mirage.Game.World
{
    public class StockRepository : MudRepositoryBase
    {
        private LinkedList<Mobile> _mobiles;
        private IRaceRepository _races;

        public StockRepository()
            : base()
        {
            _mobiles = new LinkedList<Mobile>();
            _uriChildCollections.Add("Mobiles", new ChildCollectionPair(_mobiles, QueryHints.DefaultPartialMatch));
        }

        public ICollection<Mobile> Mobiles
        {
            get { return this._mobiles; }
        }

        public IRaceRepository Races
        {
            get { return _races; }
            set { _races = value; }
        }
    }
}
