using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    public interface IMobileRepository : IEnumerable<Mobile>
    {
        ICollection<Mobile> Mobiles { get; }        
    }
}
