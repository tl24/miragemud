using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Telnet
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

    /// <summary>
    /// Telnet commands and special values
    /// </summary>
    public static class TelnetCommands
    {
        public const byte TELNET_IAC = 255;
        public const byte TELNET_DONT = 254;
        public const byte TELNET_DO = 253;
        public const byte TELNET_WONT = 252;
        public const byte TELNET_WILL = 251;
        public const byte TELNET_SB = 250;
        public const byte TELNET_GA = 249;
        public const byte TELNET_EL = 248;
        public const byte TELNET_EC = 247;
        public const byte TELNET_AYT = 246;
        public const byte TELNET_AO = 245;
        public const byte TELNET_IP = 244;
        public const byte TELNET_BREAK = 243;
        public const byte TELNET_DM = 242;
        public const byte TELNET_NOP = 241;
        public const byte TELNET_SE = 240;
        public const byte TELNET_EOR = 239;
        public const byte TELNET_ABORT = 238;
        public const byte TELNET_SUSP = 237;
        public const byte TELNET_EOF = 236;
    }

    /// <summary>
    /// Telnet option values
    /// </summary>
    public static class TelnetOptions
    {
        public const byte TELNET_TELOPT_BINARY = 0;
        public const byte TELNET_TELOPT_ECHO = 1;
        public const byte TELNET_TELOPT_RCP = 2;
        public const byte TELNET_TELOPT_SGA = 3;
        public const byte TELNET_TELOPT_NAMS = 4;
        public const byte TELNET_TELOPT_STATUS = 5;
        public const byte TELNET_TELOPT_TM = 6;
        public const byte TELNET_TELOPT_RCTE = 7;
        public const byte TELNET_TELOPT_NAOL = 8;
        public const byte TELNET_TELOPT_NAOP = 9;
        public const byte TELNET_TELOPT_NAOCRD = 10;
        public const byte TELNET_TELOPT_NAOHTS = 11;
        public const byte TELNET_TELOPT_NAOHTD = 12;
        public const byte TELNET_TELOPT_NAOFFD = 13;
        public const byte TELNET_TELOPT_NAOVTS = 14;
        public const byte TELNET_TELOPT_NAOVTD = 15;
        public const byte TELNET_TELOPT_NAOLFD = 16;
        public const byte TELNET_TELOPT_XASCII = 17;
        public const byte TELNET_TELOPT_LOGOUT = 18;
        public const byte TELNET_TELOPT_BM = 19;
        public const byte TELNET_TELOPT_DET = 20;
        public const byte TELNET_TELOPT_SUPDUP = 21;
        public const byte TELNET_TELOPT_SUPDUPOUTPUT = 22;
        public const byte TELNET_TELOPT_SNDLOC = 23;
        public const byte TELNET_TELOPT_TTYPE = 24;
        public const byte TELNET_TELOPT_EOR = 25;
        public const byte TELNET_TELOPT_TUID = 26;
        public const byte TELNET_TELOPT_OUTMRK = 27;
        public const byte TELNET_TELOPT_TTYLOC = 28;
        public const byte TELNET_TELOPT_3270REGIME = 29;
        public const byte TELNET_TELOPT_X3PAD = 30;
        public const byte TELNET_TELOPT_NAWS = 31;
        public const byte TELNET_TELOPT_TSPEED = 32;
        public const byte TELNET_TELOPT_LFLOW = 33;
        public const byte TELNET_TELOPT_LINEMODE = 34;
        public const byte TELNET_TELOPT_XDISPLOC = 35;
        public const byte TELNET_TELOPT_ENVIRON = 36;
        public const byte TELNET_TELOPT_AUTHENTICATION = 37;
        public const byte TELNET_TELOPT_ENCRYPT = 38;
        public const byte TELNET_TELOPT_NEW_ENVIRON = 39;
        public const byte TELNET_TELOPT_MSSP = 70;
        public const byte TELNET_TELOPT_COMPRESS2 = 86;
        public const byte TELNET_TELOPT_ZMP = 93;
        public const byte TELNET_TELOPT_EXOPL = 255;

        public const byte TELNET_TELOPT_MCCP2 = 86;
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

    [Flags]
    public enum telnet_state
    {
        TELNET_FLAG_PROXY = (1<<0),
        TELNET_PFLAG_DEFLATE = (1<<7)
    }

    public enum telnet_error_t
    {
        TELNET_EOK = 0,   /*!< no error */
        TELNET_EBADVAL,   /*!< invalid parameter, or API misuse */
        TELNET_ENOMEM,    /*!< memory allocation failure */
        TELNET_EOVERFLOW, /*!< data exceeds buffer size */
        TELNET_EPROTOCOL, /*!< invalid sequence of special bytes */
        TELNET_ECOMPRESS  /*!< error handling compressed streams */
    }

    public enum telnet_event_type_t
    {
        TELNET_EV_DATA = 0,        /*!< raw text data has been received */
        TELNET_EV_SEND,            /*!< data needs to be sent to the peer */
        TELNET_EV_IAC,             /*!< generic IAC code received */
        TELNET_EV_WILL,            /*!< WILL option negotiation received */
        TELNET_EV_WONT,            /*!< WONT option neogitation received */
        TELNET_EV_DO,              /*!< DO option negotiation received */
        TELNET_EV_DONT,            /*!< DONT option negotiation received */
        TELNET_EV_SUBNEGOTIATION,  /*!< sub-negotiation data received */
        TELNET_EV_COMPRESS,        /*!< compression has been enabled */
        TELNET_EV_ZMP,             /*!< ZMP command has been received */
        TELNET_EV_TTYPE,           /*!< TTYPE command has been received */
        TELNET_EV_ENVIRON,         /*!< ENVIRON command has been received */
        TELNET_EV_MSSP,            /*!< MSSP command has been received */
        TELNET_EV_WARNING,         /*!< recoverable error has occured */
        TELNET_EV_ERROR            /*!< non-recoverable error has occured */
    };

    //These are here for now since they were in libtelnet.h

    /// <summary>
    /// telopt support table element; use telopt of -1 for end marker 
    /// </summary>
    public struct telnet_telopt_t {
        /// <summary>
        /// one of the TELOPT codes
        /// </summary>
	    public short telopt;

        /// <summary>
        /// TELNET_WILL or TELNET_WONT
        /// </summary>
        public byte us;

        /// <summary>
        /// TELNET_DO or TELNET_DONT
        /// </summary>
        public byte him;
    };

    /// <summary>
    /// telnet event args data
    /// </summary>
    public class telnet_event_t : EventArgs {
        protected telnet_event_t()
        {
        }

        public telnet_event_t(telnet_event_type_t type) {
            this.type = type;
        }

        /// <summary>
        /// The type of the event
        /// </summary>
        public telnet_event_type_t type { get; protected set; }
    }

    /// <summary>
    /// data event: for DATA and SEND events 
    /// </summary>
    public class data_t : telnet_event_t {
        public data_t(telnet_event_type_t type, char[] buffer, int size) {
            this.type = type;
            this.buffer = buffer;
            if (size == 0 && buffer != null)
                this.size = buffer.Length;
            else
                this.size = size;
        }
        public char[] buffer { get; protected set; }
        public int size { get; private set; }
    }

    /// <summary>
    /// WARNING and ERROR events  
    /// </summary>
	public class error_t : telnet_event_t {
        public error_t(telnet_event_type_t type, string msg, telnet_error_t errcode) {
            this.type = type;
            this.msg = msg;
            this.errcode = errcode;
        }
		public string msg { get; private set;}                /*!< error message string */
		public telnet_error_t errcode { get; private set; }         /*!< error code */
	}

    /// <summary>
    /// Command event for : IAC
    /// </summary>
    public class iac_t : telnet_event_t {
        public iac_t(telnet_event_type_t type, short cmd) {
            this.type = type;
            this.cmd = cmd;
        }

        /// <summary>
        /// telnet command received
        /// </summary>
        public short cmd { get; protected set; }             
    }

    /// <summary>
    /// negotiation event: WILL, WONT, DO, DONT  
    /// </summary>
    public class negotiate_t : telnet_event_t {
        public negotiate_t(telnet_event_type_t type, short telopt) {
            this.type = type;
            this.telopt = telopt;
        }
        /// <summary>
        /// option being negotiated
        /// </summary>
        public short telopt { get; protected set; } 
    }

    /// <summary>
    /// subnegotiation event  
    /// </summary>
	public class subnegotiate_t : telnet_event_t  {

        public subnegotiate_t(telnet_event_type_t type, char[] buffer, short telopt) {
            this.type = type;
            this.buffer = buffer;
            this.telopt = telopt;
        }

        /// <summary>
        /// data of sub-negotiation
        /// </summary>
        public char[] buffer { get; protected set; }   
        /// <summary>
        /// option code being negotiated
        /// </summary>
        public short telopt { get; protected set; } 
	}

    /// <summary>
    /// ZMP event
    /// </summary>
	public class zmp_t : telnet_event_t {
        public zmp_t(telnet_event_type_t type, IList<string> args)
        {
            this.type = type;
            this.argv = args;
        }
        /// <summary>
        /// list of argument strings
        /// </summary>
        public IList<string> argv { get; private set; }
	}

	/// <summary>
	/// TTYPE event 
	/// </summary>
	public class ttype_t : telnet_event_t {
        public ttype_t(telnet_event_type_t type, short cmd, string name) {
            this.type = type;
            this.cmd = cmd;
            this.name = name;
        }

        /// <summary>
        /// TELNET_TTYPE_IS or TELNET_TTYPE_SEND
        /// </summary>
		public short cmd { get; private set; }
        /// <summary>
        /// terminal type name (IS only)
        /// </summary>
        public string name { get; private set; }
	}

    /// <summary>
    /// Compress event
    /// </summary>
	public class compress_t : telnet_event_t {
        public compress_t(telnet_event_type_t type, bool state) {
            this.type = type;
            this.state = state;
        }
            /// <summary>
            /// Flag indicating whether compression is enabled or not
            /// </summary>
		public bool state { get; private set; }
	}

    /// <summary>
    /// environ/MSSP command information 
    /// </summary>
    public class telnet_environ_t {
        /// <summary>
        /// either TELNET_ENVIRON_VAR or TELNET_ENVIRON_USERVAR
        /// </summary>
	    public byte type;
        /// <summary>
        /// name of the variable being set
        /// </summary>
	    public string var;
        /// <summary>
        /// value of variable being set; empty string if no value
        /// </summary>
	    public string value;
    };

    /// <summary>
    /// ENVIRON/NEW-ENVIRON event 
    /// </summary>
    public class  environ_t : telnet_event_t {

        public environ_t(telnet_event_type_t type, IList<telnet_environ_t> values, int cmd)
        {
            this.type = type;
            this.values = values;
            this.cmd = cmd;
        }
        /// <summary>
        /// list of variable values
        /// </summary>
        public IList<telnet_environ_t> values { get; private set; }
        /// <summary>
        /// SEND, IS, or INFO
        /// </summary>
        public int cmd { get; private set; }
	}
	
    /// <summary>
    /// MSSP event
    /// </summary>
	public class  mssp_t : telnet_event_t {
        public mssp_t(telnet_event_type_t type, telnet_environ_t[] values) {
            this.type = type;
            this.values = values;
        }

        /// <summary>
        /// array of variable values
        /// </summary>
        public telnet_environ_t[] values { get; private set; }
	}
}
