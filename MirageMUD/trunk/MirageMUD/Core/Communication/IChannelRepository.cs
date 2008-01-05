using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Communication
{
    public interface IChannelRepository : IEnumerable<Channel>
    {
        ICollection<Channel> Channels { get; }
    }
}
