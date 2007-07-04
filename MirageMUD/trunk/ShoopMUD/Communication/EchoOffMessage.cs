using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Communication
{
    public class EchoOffMessage : Message
    {
        public EchoOffMessage()
            : base(MessageType.UIControl, "EchoOff")
        {
        }

        /// <summary>
        ///     Turn input echoing back on after turning it off
        /// </summary>
        /// <see cref="echoOff"/>
        public override string ToString()
        {
            return "\x1B[0;30;40m";
        }
    }
}
