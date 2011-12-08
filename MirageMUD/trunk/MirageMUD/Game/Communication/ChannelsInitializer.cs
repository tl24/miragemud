using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;
using Mirage.Game.World;
using Mirage.Game.Command;

namespace Mirage.Game.Communication
{
    /// <summary>
    /// Loads and initializes the channels and their associated commands
    /// </summary>
    public class ChannelsInitializer : IInitializer
    {
        private IChannelRepository _channelRepository;

        public ChannelsInitializer(IChannelRepository ChannelRespository)
        {
            _channelRepository = ChannelRespository;
        }

        #region IInitializer Members

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public void Execute()
        {
            // Load the channel definitions


            // create the commands for each channel
            foreach (Channel channel in _channelRepository)
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