using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    /// <summary>
    ///     Turn input echoing back on after turning it off
    /// </summary>
    /// <see cref="echoOff"/>
    public class EchoOffMessage : StringMessage
    {
        public EchoOffMessage()
            : base(MessageType.UIControl, "EchoOff", "\x1B[0;30;40m")
        {
        }
    }
}
