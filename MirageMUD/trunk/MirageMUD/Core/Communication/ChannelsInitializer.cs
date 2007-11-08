using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;
using Mirage.Core.Data;
using Mirage.Core.Command;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Loads and initializes the channels and their associated commands
    /// </summary>
    public class ChannelsInitializer : IInitializer
    {
        private MudRepositoryBase _mudRepository;

        public ChannelsInitializer(MudRepositoryBase MudRespository)
        {
            _mudRepository = MudRespository;
        }

        #region IInitializer Members

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public void Execute()
        {
            // Load the channel definitions
            Serializer serializer = Serializer.GetSerializer(typeof(List<Channel>));
            List<Channel> channels = null;
            using(StreamReader reader = new StreamReader("channels.jsx")) {
                channels = (List<Channel>) serializer.Deserialize(reader);
            }            
            _mudRepository.Channels = channels;

            // create the commands for each channel
            foreach (Channel channel in channels)
            {
                foreach (ICommand command in channel.CreateCommands())
                {
                    MethodInvoker.RegisterCommand(command);
                }
            }
        }

        #endregion
    }
}