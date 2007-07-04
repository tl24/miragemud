using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Communication
{
    public class EchoOnMessage : Message
    {
        public EchoOnMessage()
            : base(MessageType.UIControl, "EchoOn")
        {
        }

        /// <summary>
        ///     Turn input echoing back on after turning it off
        /// </summary>
        /// <see cref="echoOff"/>
        public override string ToString()
        {
            return "\x1B[0m";
        }
    }
}
