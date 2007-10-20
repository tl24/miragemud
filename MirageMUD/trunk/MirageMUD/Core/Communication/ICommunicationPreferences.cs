using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Util;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Object for querying and setting communication preferences for a player
    /// </summary>
    public interface ICommunicationPreferences
    {
        /// <summary>
        /// Ignores communications from the player
        /// </summary>
        /// <param name="player">name of player to ignore</param>
        void Ignore(string player);

        /// <summary>
        /// Stops ignoring communications from the player
        /// </summary>
        /// <param name="player">name of player to stop ignoring</param>
        void UnIgnore(string player);

        /// <summary>
        /// Checks to see if a player is being ignored
        /// </summary>
        /// <param name="player">the player to check</param>
        /// <returns>true if being ignored</returns>
        bool IsIgnored(string player);

        ISet<string> Ignored { get; }

        /// <summary>
        /// Toggles the channel on or off
        /// </summary>
        /// <param name="channel">the channel name</param>
        /// <returns>returns true if the channel was toggled on, of false if it was toggled off</returns>
        bool ToggleChannel(string channel);

        /// <summary>
        /// Turns the channel on
        /// </summary>
        /// <param name="channel">channel to turn on</param>
        void ChannelOn(string channel);

        /// <summary>
        /// Turns the channel off
        /// </summary>
        /// <param name="channel">channel to turn off</param>
        void ChannelOff(string channel);

        /// <summary>
        /// Returns the list of channels that are currently on
        /// </summary>
        ISet<string> Channels { get; }

        /// <summary>
        /// Returns true if the player has the channel on
        /// </summary>
        /// <param name="channel">channel to turn on</param>
        /// <returns></returns>
        bool IsChannelOn(string channel);
    }
}
