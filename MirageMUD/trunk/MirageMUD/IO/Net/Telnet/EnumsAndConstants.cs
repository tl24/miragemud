using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.IO.Net.Telnet
{
    /// <summary>
    /// RFC1143 state names 
    /// </summary>
    public enum QState : byte
    {
        Q_NO = 0,
        Q_YES = 1,
        Q_WANTNO = 2,
        Q_WANTYES = 3,
        Q_WANTNO_OP = 4,
        Q_WANTYES_OP = 5
    }


   public static class TelnetSubOptionCodes
    {
        /*Protocol codes for TERMINAL-TYPE commands. */
        public const byte TELNET_TTYPE_IS = 0;
        public const byte TELNET_TTYPE_SEND = 1;

        /* Protocol codes for NEW-ENVIRON/ENVIRON commands. */
        public const byte TELNET_ENVIRON_IS = 0;
        public const byte TELNET_ENVIRON_SEND = 1;
        public const byte TELNET_ENVIRON_INFO = 2;
        public const byte TELNET_ENVIRON_VAR = 0;
        public const byte TELNET_ENVIRON_VALUE = 1;
        public const byte TELNET_ENVIRON_ESC = 2;
        public const byte TELNET_ENVIRON_USERVAR = 3;

        /*Protocol codes for MSSP commands. */
        public const byte TELNET_MSSP_VAR = 1;
        public const byte TELNET_MSSP_VAL = 2;
    }



}
