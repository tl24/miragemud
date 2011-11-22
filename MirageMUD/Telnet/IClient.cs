using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Telnet
{
    public interface IClient
    {
        /// <summary>
        /// Writes bytes back to the client
        /// </summary>
        /// <param name="bytes"></param>
        void Write(byte[] bytes);

    }
}
