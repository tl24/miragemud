using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;

namespace Mirage.Stock.Data
{
    public class StockRepository : MudRepositoryBase
    {
        private LinkedList<Mobile> _mobiles;

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

    }
}