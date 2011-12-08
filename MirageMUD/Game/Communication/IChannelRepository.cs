using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Communication
{
    public interface IChannelRepository : IEnumerable<Channel>
    {
        ICollection<Channel> Channels { get; }
    }
}
