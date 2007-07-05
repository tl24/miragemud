using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.IO
{
    /// <summary>
    ///     The connection state for a player.  A player goes through
    /// various states before they are completely logged in
    /// </summary>
    public enum ConnectedState
    {
        /// <summary>
        ///     The player is connecting
        /// </summary>
        Connecting,
        /// <summary>
        ///     The player is idle
        /// </summary>
        Idle,
        /// <summary>
        ///     The player is playing, completely logged in and not idle
        /// </summary>
        Playing,

        /// <summary>
        /// The player has disconnected and should be cleaned up
        /// </summary>
        Disconnected
    }
}
