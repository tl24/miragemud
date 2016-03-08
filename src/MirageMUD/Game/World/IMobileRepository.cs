using System.Collections.Generic;

namespace Mirage.Game.World
{
    public interface IMobileRepository : IEnumerable<Mobile>
    {
        ICollection<Mobile> Mobiles { get; }        
    }
}
