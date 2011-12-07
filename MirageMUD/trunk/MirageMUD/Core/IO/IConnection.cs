using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.IO
{
    public interface IConnection
    {
        /// <summary>
        /// Close the client and its underlying connection
        /// </summary>
        void Close();

        /// <summary>
        /// Checks to see if the client socket is still open
        /// </summary>
        /// <returns></returns>
        bool IsOpen { get; }

        string Address { get; }
    }
}
