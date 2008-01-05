using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using JsonExSerializer;
using System.IO;

namespace Mirage.Core.Communication
{
    public class ChannelRepository : IChannelRepository
    {
        private MudRepositoryBase _mudRepository;
        private bool _loaded;

        public ChannelRepository(MudRepositoryBase MudRepository)
        {
            _mudRepository = MudRepository;
        }

        private void Load()
        {
            Serializer serializer = Serializer.GetSerializer(typeof(List<Channel>));
            List<Channel> channels = null;
            using (StreamReader reader = new StreamReader("channels.jsx"))
            {
                channels = (List<Channel>)serializer.Deserialize(reader);
            }
            _mudRepository.Channels = channels;
            _loaded = true;
        }

        #region IChannelRepository Members

        public ICollection<Channel> Channels
        {
            get {
                if (!_loaded)
                    Load();
                return _mudRepository.Channels;
            }
        }

        #endregion

        #region IEnumerable<Channel> Members

        public IEnumerator<Channel> GetEnumerator()
        {
            return Channels.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
