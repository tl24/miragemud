using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using JsonExSerializer;
using System.IO;

namespace Mirage.Core.Communication
{
    public class ChannelRepository : JsonSimpleRepository<Channel>, IChannelRepository
    {

        public ChannelRepository()
            : base("channels.jsx")
        {
        }

        protected override List<Channel> Load()
        {
            List<Channel> channels = base.Load();
            return channels;
        }

        public ICollection<Channel> Channels
        {
            get { return this.Items; }
        }
    }
}
