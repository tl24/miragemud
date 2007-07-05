using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    /// <summary>
    ///     Turn input echoing back on after turning it off
    /// </summary>
    /// <see cref="echoOff"/>
    public class EchoOnMessage : StringMessage
    {
        public EchoOnMessage()
            : base(MessageType.UIControl, "EchoOn", "\x1B[0m")
        {
        }
    }
}
