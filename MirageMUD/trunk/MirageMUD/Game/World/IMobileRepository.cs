using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World
{
    public interface IMobileRepository : IEnumerable<Mobile>
    {
        ICollection<Mobile> Mobiles { get; }        
    }
}
