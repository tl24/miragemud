using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Stock.Data
{
    public interface IMobileRepository : IEnumerable<Mobile>
    {
        ICollection<Mobile> Mobiles { get; }        
    }
}
