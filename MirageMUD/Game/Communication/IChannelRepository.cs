using System.Collections.Generic;

namespace Mirage.Game.Communication
{
    public interface IChannelRepository : IEnumerable<Channel>
    {
        ICollection<Channel> Channels { get; }
    }
}
