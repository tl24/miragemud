using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Util;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Communication preferences for channels
    /// </summary>
    public class CommunicationPreferences : ICommunicationPreferences
    {
        private System.Collections.Generic.HashSet<string> _ignored;
        private System.Collections.Generic.HashSet<string> _channels;

        public CommunicationPreferences()
        {
            _ignored = new System.Collections.Generic.HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            _channels = new System.Collections.Generic.HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        }

        #region ICommunicationPreferences Members

        /// <summary>
        /// Ignore a player
        /// </summary>
        /// <param name="player">player to ignore</param>
        public void Ignore(string player)
        {
            _ignored.Add(player);
        }

        /// <summary>
        /// Unignore a player
        /// </summary>
        /// <param name="player">player to unignore</param>
        public void UnIgnore(string player)
        {
            _ignored.Remove(player);
        }

        /// <summary>
        /// Checks to see if a player is ignored
        /// </summary>
        /// <param name="player">player to check</param>
        public bool IsIgnored(string player)
        {
            return _ignored.Contains(player);
        }

        /// <summary>
        /// List of ignored players
        /// </summary>
        public System.Collections.Generic.HashSet<string> Ignored
        {
            get { return _ignored; }
            set
            {
                _ignored.Clear();
                _ignored.UnionWith(value);
            }
        }

        /// <summary>
        /// Toggles the channel state to either on or off
        /// </summary>
        /// <param name="channel">channel to toggle</param>
        /// <returns>true if the new state is on, false if its off</returns>
        public bool ToggleChannel(string channel)
        {
            if (IsChannelOn(channel))
            {
                ChannelOff(channel);
                return false;
            }
            else
            {
                ChannelOn(channel);
                return true;
            }
        }

        /// <summary>
        /// Turns a channel on
        /// </summary>
        /// <param name="channel">the channel to turn on</param>
        public void ChannelOn(string channel)
        {
            _channels.Add(channel);
        }

        /// <summary>
        /// Turns a channel off
        /// </summary>
        /// <param name="channel">the channel to turn off</param>
        public void ChannelOff(string channel)
        {
            _channels.Remove(channel);
        }

        /// <summary>
        /// Returns the set of channels that are turned on
        /// </summary>
        public System.Collections.Generic.HashSet<string> Channels
        {
            get { return _channels; }
            set
            {
                _channels.Clear();
                _channels.UnionWith(value);
            }
        }

        /// <summary>
        /// Checks to see if the channel is on
        /// </summary>
        /// <param name="channel">channel to check</param>
        /// <returns></returns>
        public bool IsChannelOn(string channel)
        {
            return _channels.Contains(channel);
        }

        #endregion
    }
}
