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
        private MudRepositoryBase _mudRepository;

        public ChannelRepository(MudRepositoryBase MudRepository)
            : base("channels.jsx")
        {
            _mudRepository = MudRepository;
        }

        protected override List<Channel> Load()
        {
            List<Channel> channels = base.Load();
            _mudRepository.Channels = this;
            return channels;
        }

        public ICollection<Channel> Channels
        {
            get { return this.Items; }
        }
    }
}
