using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Communication
{
    /// <summary>
    /// Interface for an object that can receive messages
    /// </summary>
    public interface IReceiveMessages
    {
        void Write(IMessage message);
        void Write(object sender, IMessage message);
    }
}
