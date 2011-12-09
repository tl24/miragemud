using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.IO.Net.Telnet
{

    /// <summary>
    /// Specifies the remote and local support of a telnet option
    /// </summary>
    public struct OptionSupportEntry
    {
        byte option;
        byte value;
        private const byte btrue = 1;
        private const byte bfalse = 0;

        /// <summary>
        /// Creates a new OptionSupportEntry instance
        /// </summary>
        /// <param name="telnetOption">the byte code of the telnet option</param>
        /// <param name="localSupport">flag indicating local support for the option</param>
        /// <param name="remoteSupport">flag indicating remote support for the option</param>
        public OptionSupportEntry(byte telnetOption, bool localSupport, bool remoteSupport)
        {
            option = telnetOption;
            value = GetValue(localSupport, remoteSupport);
        }

        private static byte GetValue(bool localSupport, bool remoteSupport)
        {
            return (byte) ((localSupport ? btrue : bfalse) << 1 | (remoteSupport ? btrue : bfalse));
        }

        /// <summary>
        /// The byte code of the telnet option
        /// </summary>
        public byte TelnetOption
        {
            get
            {
                return option;
            }
        }

        /// <summary>
        /// Flag indicating whether the option is supported locally.  If not supported, DO requests are
        /// answered by WONT
        /// </summary>
        public bool SupportedLocally
        {
            get
            {
                return (value & 2) > 0;
            }
        }

        /// <summary>
        /// Flag indicating whether the option is supported remotely.
        /// </summary>
        public bool SupportedRemotely {
            get {
                return (value & 1) > 0;
            }
        }

        public override int GetHashCode()
        {
            return option;
        }

        public override bool Equals(object obj)
        {
            if (obj is OptionSupportEntry)
                return option == ((OptionSupportEntry)obj).option;
            else
                return false;
        }
    }
}
