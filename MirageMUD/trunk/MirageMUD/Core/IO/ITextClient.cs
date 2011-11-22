using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.IO
{
    public interface ITextClient : ITelnetClient
    {
        TextClientOptions Options { get; }

        /// <summary>
        /// Used to write raw bytes to the client such as Telnet IAC sequences
        /// </summary>
        /// <param name="bytes"></param>
        void Write(byte[] bytes);
    }
}
