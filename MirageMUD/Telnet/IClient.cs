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

        /// <summary>
        /// Sets whether the echo option is on
        /// </summary>
        bool EchoOn { get; set; }
    }

    /// <summary>
    /// Optional interface to implement if NAWS (Window size) is supported
    /// </summary>
    public interface IClientNaws : IClient
    {
        /// <summary>
        /// The width of the client window
        /// </summary>
        int WindowWidth { get; set; }

        /// <summary>
        /// The height of the client window
        /// </summary>
        int WindowHeight { get; set; }
    }
}
