using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;

namespace Mirage.Core.Data
{
    /// <summary>
    /// represents an object that is capable of invoking a command
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Writes a message to the player's output stream
        /// </summary>
        /// <param name="message">the message to write</param>
        void Write(IMessage message);

        /// <summary>
        /// The level of the actor
        /// </summary>
        //TODO: Get rid of this, but the invoker currently checks this
        int Level { get; }
    }
}
