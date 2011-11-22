using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Mirage.Telnet
{

    public class LibTelnet {
        //Some constants to remove later
        private const int __LINE__ = 0;
        private const string __func__ = "";

        /// <summary>
        /// RFC1143 option negotiation state
        /// </summary>
         struct telnet_rfc1143_t {
	        public int telopt;
	        public byte state;

             public byte Us {
                 get {
                    return state & 0x0F;
                 }
             }

             public byte Him {
                 get {
                     return ((state & 0xF0) >> 4);
                 }
             }

             public void Make(byte us, byte him) {
                 this.state = (us) | ((him) << 4);
             }
         }

        /// <summary>
        /// helper for the negotiation routines
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="cmd"></param>
        /// <param name="opt"></param>
        static void NEGOTIATE_EVENT(telnet_t telnet,telnet_event_type_t cmd,byte opt) {
        	telnet_event_t ev;
	        telnet_rfc1143_t q;

	        ev.type = cmd;
	        ev.neg.telopt = opt;
	        telnet.eh((telnet), ev, telnet.ud);
        }

        /// <summary>
        /// telnet state codes 
        /// </summary>
        enum telnet_state_t {
	        TELNET_STATE_DATA = 0,
	        TELNET_STATE_IAC,
	        TELNET_STATE_WILL,
	        TELNET_STATE_WONT,
	        TELNET_STATE_DO,
	        TELNET_STATE_DONT,
	        TELNET_STATE_SB,
	        TELNET_STATE_SB_DATA,
	        TELNET_STATE_SB_DATA_IAC
        }

        /// <summary>
        /// telnet state tracker
        /// </summary>
        class telnet_t {
            public telnet_t() {
                this.q = new List<telnet_rfc1143_t>();
                this.buffer = new MemoryStream();                
            }

            /// <summary>
            /// user data 
            /// </summary>
	        public object ud;
	        /* telopt support table */
	        public telnet_telopt_t[] telopts;
	        /* event handler */
	        public telnet_event_handler_t eh;

        	
            /// <summary>
            /// zlib (mccp2) compression
            /// </summary>
	        public DeflateStream z;

	        /* RFC1143 option negotiation states */
	        public List<telnet_rfc1143_t> q;
            /// <summary>
            /// sub-request buffer */ 
            /// </summary>
	        public MemoryStream buffer;

            /// <summary>
            /// Gets the contents of the buffer as a byte array and then clears the buffer
            /// </summary>
            /// <returns>contents of buffer as byte array</returns>
            public byte[] GetAndClearBuffer() {
                byte[] bytes = new byte[buffer.Length];
                buffer.Read(bytes, 0, bytes.Length);
                buffer.Seek(0, SeekOrigin.Begin);
                return bytes;
            }

            /// <summary>
            /// current state 
            /// </summary>
	        public telnet_state_t state;

            /// <summary>
            /// option flags 
            /// </summary>
	        public telnet_state flags;
        	
            /// <summary>
            /// current subnegotiation telopt 
            /// </summary>
	        public byte sb_telopt;
        	
            /// <summary>
            /// length of RFC1143 queue 
            /// </summary>
	        public byte q_size;
        }


        /* RFC1143 state names */
        private const int Q_NO = 0;
        private const int Q_YES = 1;
        private const int Q_WANTNO = 2;
        private const int Q_WANTYES = 3;
        private const int Q_WANTNO_OP = 4;
        private const int Q_WANTYES_OP = 5;

        /* buffer sizes */
        private const int[] _buffer_sizes = { 0, 512, 2048, 8192, 16384 };
        private const int _buffer_sizes_count = _buffer_sizes.Length;

        /* error generation function */
        
        static telnet_error_t _error(telnet_t telnet, int line,
		        string func, telnet_error_t err, int fatal, string fmt,
		        params object[] args) {

	        // format informational text
            string msg = string.Format(fmt, args);

	        // send error event to the user 
            error_t ev = new error_t(fatal == 1 ? telnet_event_type_t.TELNET_EV_ERROR : telnet_event_type_t.TELNET_EV_WARNING,
                msg,
                err);
	        telnet->eh(telnet, ev, telnet.ud);
        	
	        return err;
        }
        

        /// <summary>
        /// initialize the zlib box for a telnet box; if deflate is non-zero, it
        /// initializes zlib for deflating (compression), otherwise for inflating 
        /// (decompression).  returns TELNET_EOK on success, something else on 
        /// failure.
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="deflate"></param>
        /// <param name="err_fatal"></param>
        /// <returns></returns>
        void _init_zlib(telnet_t telnet, bool deflate, bool err_fatal) {
	        DeflateStream z;
	        int rs;
            //TODO: pass in inputStream
            Stream inputStream = null;
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

	        /* if compression is already enabled, fail loudly */
	        if (telnet.z != null)
                throw new InvalidOperationException("cannot initialize compression twice");

	        /* initialize */
	        if (deflate) {
                z = new DeflateStream(inputStream, CompressionMode.Compress);
		        telnet.flags |= TELNET_PFLAG_DEFLATE;
	        } else {
                z = new DeflateStream(inputStream, CompressionMode.Decompress);
		        telnet.flags &= ~TELNET_PFLAG_DEFLATE;
	        }

	        telnet.z = z;
        }


        /// <summary>
        /// push bytes out, compressing them first if need be 
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        static void _send(telnet_t telnet, byte[] buffer, int size) {
	        telnet_event_t ev;

	        /* if we have a deflate (compression) zlib box, use it */
	        if (telnet.z != 0 && telnet.flags & TELNET_PFLAG_DEFLATE) {
		        byte[] deflate_buffer = new byte[1024];
		        int rs;

		        /* initialize z state */
                telnet.z.Write(buffer, 0, size);


			        /* send event */
                //TODO: Nothing to send since we are writing directly to the stream
                /*
                    data_t ev = new data_t(telnet_event_type_t.TELNET_EV_SEND,
			        ev.type = TELNET_EV_SEND;
			        ev.data.buffer = deflate_buffer;
			        ev.data.size = sizeof(deflate_buffer) - telnet->z->avail_out;
			        telnet->eh(telnet, &ev, telnet->ud);
                */
        		

		        /* do not continue with remaining code */
		        return;
	        }

            data_t ev = new data_t(telnet_event_type_t.TELNET_EV_SEND, buffer, size);
	        telnet.eh(telnet, ev, telnet.ud);
        }

         /* to send bags of unsigned chars */
        /*
        #define _sendu(t, d, s) _send((t), (const char*)(d), (s))
        */
        
        /// <summary>
        /// check if we support a particular telopt; if us is non-zero, we 
        /// check if we (local) supports it, otherwise we check if he (remote) 
        /// supports it.  return non-zero if supported, zero if not supported. 
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        static bool _check_telopt(telnet_t telnet, byte telopt, bool us) {

	        // if we have no telopts table, we obviously don't support it
	        if (telnet.telopts == null)
		        return false;

	        // loop until found or end
            foreach(var opt in telnet.telopts) {
		        if (opt.telopt == telopt) {
			        if (us && opt.us == TELNET_WILL)
				        return true;
			        else if (!us && opt.him == TelnetCommands.TELNET_DO)
				        return true;
			        else
				        return false;
                }
            }

	        // not found, so not supported
	        return false;
        }

        /// <summary>
        /// retrieve RFC1143 option state
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        static telnet_rfc1143_t _get_rfc1143(telnet_t telnet, byte telopt) {
	        telnet_rfc1143_t empty;
	        

	        // search for entry
	        foreach (var opt in telnet.q) {
		        if (opt.telopt == telopt) {
			        return opt;
		        }
	        }

	        // not found, return empty value
 	        empty.telopt = telopt;
	        empty.state = 0;
	        return empty;
        }

        /// <summary>
        /// save RFC1143 option state
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="?"></param>
        static void _set_rfc1143(telnet_t telnet, byte telopt, byte us, byte him) {
	        telnet_rfc1143_t qtmp;
	        int i;

	        // search for entry
	        foreach (var opt in telnet.q) {
		        if (opt.telopt == telopt) {
			        opt.Make(us, him);
                    return;
		        }
	        }

            telnet_rfc1143_t qtmp = new telnet_rfc1143_t();
            qtmp.telopt = telopt;
            qtmp.Make(us,him);
            telnet.q.Add(qtmp);
        }

        /// <summary>
        /// send negotiation bytes
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="cmd"></param>
        /// <param name="telopt"></param>
        static void _send_negotiate(telnet_t telnet, byte cmd, byte telopt) {
            byte[] bytes = new byte[] { TelnetCommands.TELNET_IAC, cmd, (byte) telopt };
	        _send(telnet, bytes, 3);
        }

        /// <summary>
        /// negotiation handling magic for RFC1143
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="telopt"></param>
        static void _negotiate(telnet_t telnet, byte telopt) {
	        telnet_event_t ev;
	        telnet_rfc1143_t q;

	        // in PROXY mode, just pass it thru and do nothing
	        if (telnet.flags & TELNET_FLAG_PROXY) {
		        switch (telnet.state) {
		        case telnet_state_t.TELNET_STATE_WILL:
			        NEGOTIATE_EVENT(telnet, telnet_state_t.TELNET_EV_WILL, telopt);
			        break;
		        case telnet_state_t.TELNET_STATE_WONT:
			        NEGOTIATE_EVENT(telnet, telnet_state_t.TELNET_EV_WONT, telopt);
			        break;
		        case telnet_state_t.TELNET_STATE_DO:
			        NEGOTIATE_EVENT(telnet, telnet_state_t.TELNET_EV_DO, telopt);
			        break;
		        case telnet_state_t.TELNET_STATE_DONT:
			        NEGOTIATE_EVENT(telnet, telnet_state_t.TELNET_EV_DONT, telopt);
			        break;
		        }
		        return;
	        }

	        // lookup the current state of the option
	        telnet_rfc1143_t q = _get_rfc1143(telnet, telopt);

	        // start processing...
	        switch (telnet.state) {
	        // request to enable option on remote end or confirm DO
	        case telnet_state_t.TELNET_STATE_WILL:
		        switch (q.Him) {
		        case Q_NO:
			        if (_check_telopt(telnet, telopt, false)) {
				        _set_rfc1143(telnet, telopt, q.Us, Q_YES);
				        _send_negotiate(telnet, TelnetCommands.TELNET_DO, telopt);
				        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WILL, telopt);
			        } else
				        _send_negotiate(telnet, TelnetCommands.TELNET_DONT, telopt);
			        break;
		        case Q_WANTNO:
			        _set_rfc1143(telnet, telopt, q.Us, Q_NO);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WONT, telopt);
			        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
					        "DONT answered by WILL");
			        break;
		        case Q_WANTNO_OP:
			        _set_rfc1143(telnet, telopt, q.Us, Q_YES);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WILL, telopt);
			        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
					        "DONT answered by WILL");
			        break;
		        case Q_WANTYES:
			        _set_rfc1143(telnet, telopt, q.Us, Q_YES);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WILL, telopt);
			        break;
		        case Q_WANTYES_OP:
			        _set_rfc1143(telnet, telopt, q.Us, Q_WANTNO);
			        _send_negotiate(telnet, TelnetCommands.TELNET_DONT, telopt);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WILL, telopt);
			        break;
		        }
		        break;

	        /* request to disable option on remote end, confirm DONT, reject DO */
	        case telnet_state_t.TELNET_STATE_WONT:
		        switch (q.Him) {
		        case Q_YES:
			        _set_rfc1143(telnet, telopt, q.Us, Q_NO);
			        _send_negotiate(telnet, TelnetCommands.TELNET_DONT, telopt);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WONT, telopt);
			        break;
		        case Q_WANTNO:
			        _set_rfc1143(telnet, telopt, q.Us, Q_NO);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WONT, telopt);
			        break;
		        case Q_WANTNO_OP:
			        _set_rfc1143(telnet, telopt, q.Us, Q_WANTYES);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DO, telopt);
			        break;
		        case Q_WANTYES:
		        case Q_WANTYES_OP:
			        _set_rfc1143(telnet, telopt, q.Us, Q_NO);
			        break;
		        }
		        break;

	        /* request to enable option on local end or confirm WILL */
	        case telnet_state_t.TELNET_STATE_DO:
		        switch (q.Us) {
		        case Q_NO:
			        if (_check_telopt(telnet, telopt, 1)) {
				        _set_rfc1143(telnet, telopt, Q_YES, q.Him);
				        _send_negotiate(telnet, TELNET_WILL, telopt);
				        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DO, telopt);
			        } else
				        _send_negotiate(telnet, TELNET_WONT, telopt);
			        break;
		        case Q_WANTNO:
			        _set_rfc1143(telnet, telopt, Q_NO, q.Him);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DONT, telopt);
			        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
					        "WONT answered by DO");
			        break;
		        case Q_WANTNO_OP:
			        _set_rfc1143(telnet, telopt, Q_YES, q.Him);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DO, telopt);
			        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
					        "WONT answered by DO");
			        break;
		        case Q_WANTYES:
			        _set_rfc1143(telnet, telopt, Q_YES, q.Him);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DO, telopt);
			        break;
		        case Q_WANTYES_OP:
			        _set_rfc1143(telnet, telopt, Q_WANTNO, q.Him);
			        _send_negotiate(telnet, TELNET_WONT, telopt);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DO, telopt);
			        break;
		        }
		        break;

	        /* request to disable option on local end, confirm WONT, reject WILL */
	        case telnet_state_t.TELNET_STATE_DONT:
		        switch (q.Us) {
		        case Q_YES:
			        _set_rfc1143(telnet, telopt, Q_NO, q.Him);
			        _send_negotiate(telnet, TELNET_WONT, telopt);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_DONT, telopt);
			        break;
		        case Q_WANTNO:
			        _set_rfc1143(telnet, telopt, Q_NO, q.Him);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WONT, telopt);
			        break;
		        case Q_WANTNO_OP:
			        _set_rfc1143(telnet, telopt, Q_WANTYES, q.Him);
			        NEGOTIATE_EVENT(telnet, telnet_event_type_t.TELNET_EV_WILL, telopt);
			        break;
		        case Q_WANTYES:
		        case Q_WANTYES_OP:
			        _set_rfc1143(telnet, telopt, Q_NO, q.Him);
			        break;
		        }
		        break;
	        }
        }

        /// <summary>
        /// process an ENVIRON/NEW-ENVIRON subnegotiation buffer         
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="type"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static int _environ(telnet_t telnet, byte type, byte[] buffer) {
        	
	        var values = new List<telnet_environ_t>();

	        // if we have no data, just pass it through
	        if (buffer.Length == 0) {
		        return 0;
	        }

	        // first byte must be a valid command
	        if (buffer[0] != TelnetSubOptionCodes.TELNET_ENVIRON_SEND &&
			        buffer[0] != TelnetSubOptionCodes.TELNET_ENVIRON_IS && 
			        buffer[0] != TelnetSubOptionCodes.TELNET_ENVIRON_INFO) {
		        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
				        "telopt {0} subneg has invalid command", type);
		        return 0;
	        }
            environ_t ev;
            byte cmd = buffer[0];
	        // store ENVIRON command 
        	

	        // if we have no arguments, send an event with no data end return
	        if (buffer.Length == 1) {
                // no list of variables given
                ev = new environ_t(telnet_event_type_t.TELNET_EV_ENVIRON, values, cmd);

		        // invoke event with our arguments
		        telnet.eh(telnet, ev, telnet.ud);
		        return 1;
	        }

	        /* very second byte must be VAR or USERVAR, if present */
	        if (buffer[1] != TelnetSubOptionCodes.TELNET_ENVIRON_VAR &&
			        buffer[1] != TelnetSubOptionCodes.TELNET_ENVIRON_USERVAR) {
		        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
				        "telopt {0} subneg missing variable type", type);
		        return 0;
	        }

	        /* ensure last byte is not an escape byte (makes parsing later easier) */
	        if (buffer[buffer.Length - 1] == TelnetSubOptionCodes.TELNET_ENVIRON_ESC) {
		        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
				        "telopt {0} subneg ends with ESC", type);
		        return 0;
	        }


            byte[] destBuffer = new byte[buffer.Length];
            int destIndex = 0;
            int bufIndex = 0;
            telnet_environ_t current = null;
            while (bufIndex++ < buffer.Length) {
                byte currByte = buffer[bufIndex];
                if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_ESC) {
                    bufIndex++;
                    continue;
                } else if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_USERVAR || currByte == TelnetSubOptionCodes.TELNET_ENVIRON_VAR) {
                    // new variable
                    if (current != null) {
                        if (destIndex == 0)
                            current.value = "";
                        else {
                            current.value = BytesToString(destBuffer,destIndex);
                        }                
                        values.Add(current);
                    }
                    destIndex = 0;
                    current = new telnet_environ_t();
                    current.type == currByte;
                } else if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_VALUE) {
                    if (destIndex == 0)
                        current.var = "";
                    else {
                        current.var = BytesToString(destBuffer,destIndex);
                    }
                    destIndex = 0;
                } else {
                    destBuffer[destIndex++] = currByte;
                }
            }
            if (current != null) {
                current.var = current.var ?? "";
                current.value = current.value ?? "";
                if (destIndex > 0) {
                    if (string.IsNullOrEmpty(current.var)) {
                        current.var = BytesToString(destBuffer,destIndex);
                    } else {
                        current.value = BytesToString(destBuffer,destIndex);
                    }
                }
                values.Add(current);
            }

            ev = new environ_t(telnet_event_type_t.TELNET_EV_ENVIRON, values, cmd);

	        // invoke event with our arguments
	        telnet.eh(telnet, ev, telnet.ud);
	        return 1;
        }

        /// <summary>
        /// Convert entire byte array into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        /// <returns>string</returns>
        static string BytesToString(byte[] bytes) {
            return BytesToString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert the specified number bytes into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="length">number of bytes to convert</param>
        /// <returns>string</returns>
        static string BytesToString(byte[] bytes, int length) {
            return BytesToString(bytes, 0, length);
        }

        /// <summary>
        /// Convert the specified bytes into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="offset">the starting offset to start the conversion</param>
        /// <param name="length">number of bytes to convert</param>
        /// <returns>string</returns>
        static string BytesToString(byte[] bytes, int offset, int length) {
            // for now just ascii
            return Encoding.ASCII.GetString(bytes, offset, length);
        }

        /// <summary>
        /// process an MSSP subnegotiation buffer 
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static int _mssp(telnet_t telnet, byte[] buffer) {
	        var values = new List<telnet_environ_t>();

	        // if we have no data, just pass it through
	        if (buffer == null || buffer.Length == 0) {
		        return 0;
	        }

	        // first byte must be a VAR
	        if (buffer[0] != TelnetSubOptionCodes.TELNET_MSSP_VAR) {
		        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
				        "MSSP subnegotiation has invalid data");
		        return 0;
	        }

            //TODO: rewrite this without a second buffer,
            // we don't need it since we don't need to escape
            byte[] destBuffer = new byte[buffer.Length];
            int destIndex = 0;
            int bufIndex = 0;
            telnet_environ_t current = null;
            while (bufIndex++ < buffer.Length) {
                byte currByte = buffer[bufIndex];
                if (currByte == TelnetSubOptionCodes.TELNET_MSSP_VAR) {
                    // new variable
                    if (current != null) {
                        if (destIndex == 0)
                            current.value = "";
                        else {
                            current.value = BytesToString(destBuffer,destIndex);
                        }                
                        values.Add(current);
                    }
                    destIndex = 0;
                    current = new telnet_environ_t();
                    current.type == currByte;
                } else if (currByte == TelnetSubOptionCodes.TELNET_MSSP_VAL) {
                    if (destIndex == 0)
                        current.var = "";
                    else {
                        current.var = BytesToString(destBuffer,destIndex);
                    }
                    destIndex = 0;
                } else {
                    destBuffer[destIndex++] = currByte;
                }
            }
            if (current != null) {
                current.var = current.var ?? "";
                current.value = current.value ?? "";
                if (destIndex > 0) {
                    if (string.IsNullOrEmpty(current.var)) {
                        current.var = BytesToString(destBuffer,destIndex);
                    } else {
                        current.value = BytesToString(destBuffer,destIndex);
                    }
                }
                values.Add(current);
            }
    
            var ev = new mssp_t(telnet_event_type_t.TELNET_EV_MSSP, values);

            // invoke event with our arguments
            telnet.eh(telnet, ev, telnet.ud);
            return 1;    
        }

        /// <summary>
        /// parse ZMP command subnegotiation buffers
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static int _zmp(telnet_t telnet, byte[] buffer) {

            /* make sure this is a valid ZMP buffer */
            if (size == 0 || buffer[buffer.Length - 1] != 0) {
	            _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
			            "incomplete ZMP frame");
	            return 0;
            }
            var argv = new List<string>();
            int charStart = 0;
            for (int i = 0; i < buffer.Length; i++) {
                if (buffer[i] == 0) {
                    argv.Add(BytesToString(buffer, charStart, i-charStart));
                    charStart = i+1;
                }
            }

            telnet.eh(telnet, new zmp_t(telnet_event_type_t.TELNET_EV_ZMP, argv), telnet.ud);
            return 1;
        }

        /// <summary>
        /// parse TERMINAL-TYPE command subnegotiation buffers
        /// </summary>
        /// <param name="telnet"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static int _ttype(telnet_t telnet, byte[] buffer) {
	        telnet_event_t ev;

	        /* make sure request is not empty */
	        if (buffer.Length == 0) {
		        _error(telnet, __LINE__, __func__, telnet_error_t.TELNET_EPROTOCOL, 0,
				        "incomplete TERMINAL-TYPE request");
		        return 0;
	        }

	        /* make sure request has valid command type */
	        if (buffer[0] != TelnetSubOptionCodes.TELNET_TTYPE_IS &&
			        buffer[0] != TelnetSubOptionCodes.TELNET_TTYPE_SEND) {
		        _error(telnet, __LINE__, __func__, TELNET_EPROTOCOL, 0,
				        "TERMINAL-TYPE request has invalid type");
		        return 0;
	        }

	        /* send proper event */
	        if (buffer[0] == TelnetSubOptionCodes.TELNET_TTYPE_IS) {
		        string name = BytesToString(buffer, 1, buffer.Length - 1);

                var ev = new ttype_t(telnet_event_type_t.TELNET_EV_TTYPE, TelnetSubOptionCodes.TELNET_TTYPE_IS, name);
		        telnet.eh(telnet, ev, telnet.ud);

	        } else {
                var ev = new ttype_t(telnet_event_type_t.TELNET_EV_TTYPE, TelnetSubOptionCodes.TELNET_TTYPE_SEND, null);
		        telnet.eh(telnet, ev, telnet.ud);
	        }

	        return 1;
        }

/* process a subnegotiation buffer; return non-zero if the current buffer
 * must be aborted and reprocessed due to COMPRESS2 being activated
 */
static int _subnegotiate(telnet_t telnet) {
	telnet_event_t ev;

	/* standard subnegotiation event */
	ev.type = telnet_event_type_t.TELNET_EV_SUBNEGOTIATION;
	ev.sub.telopt = telnet->sb_telopt;
	ev.sub.buffer = telnet->buffer;
	ev.sub.size = telnet->buffer_pos;
    var ev = new subnegotiate_t(telnet_event_type_t.TELNET_EV_SUBNEGOTIATION, 
	telnet->eh(telnet, &ev, telnet->ud);

	switch (telnet->sb_telopt) {
#if defined(HAVE_ZLIB)
	/* received COMPRESS2 begin marker, setup our zlib box and
	 * start handling the compressed stream if it's not already.
	 */
	case TELNET_TELOPT_COMPRESS2:
		if (telnet->sb_telopt == TELNET_TELOPT_COMPRESS2) {
			if (_init_zlib(telnet, 0, 1) != TELNET_EOK)
				return 0;

			/* notify app that compression was enabled */
			ev.type = telnet_event_type_t.TELNET_EV_COMPRESS;
			ev.compress.state = 1;
			telnet->eh(telnet, &ev, telnet->ud);
			return 1;
		}
		return 0;
#endif /* defined(HAVE_ZLIB) */

	/* specially handled subnegotiation telopt types */
	case TELNET_TELOPT_ZMP:
		return _zmp(telnet, telnet->buffer, telnet->buffer_pos);
	case TELNET_TELOPT_TTYPE:
		return _ttype(telnet, telnet->buffer, telnet->buffer_pos);
	case TELNET_TELOPT_ENVIRON:
	case TELNET_TELOPT_NEW_ENVIRON:
		return _environ(telnet, telnet->sb_telopt, telnet->buffer,
				telnet->buffer_pos);
	case TELNET_TELOPT_MSSP:
		return _mssp(telnet, telnet->buffer, telnet->buffer_pos);
	default:
		return 0;
	}
}

/* initialize a telnet state tracker */
telnet_t *telnet_init(const telnet_telopt_t *telopts,
		telnet_event_handler_t eh, unsigned char flags, void *user_data) {
	/* allocate structure */
	struct telnet_t *telnet = (telnet_t*)calloc(1, sizeof(telnet_t));
	if (telnet == 0)
		return 0;

	/* initialize data */
	telnet->ud = user_data;
	telnet->telopts = telopts;
	telnet->eh = eh;
	telnet->flags = flags;

	return telnet;
}

/* free up any memory allocated by a state tracker */
void telnet_free(telnet_t *telnet) {
	/* free sub-request buffer */
	if (telnet->buffer != 0) {
		free(telnet->buffer);
		telnet->buffer = 0;
		telnet->buffer_size = 0;
		telnet->buffer_pos = 0;
	}

#if defined(HAVE_ZLIB)
	/* free zlib box */
	if (telnet->z != 0) {
		if (telnet->flags & TELNET_PFLAG_DEFLATE)
			deflateEnd(telnet->z);
		else
			inflateEnd(telnet->z);
		free(telnet->z);
		telnet->z = 0;
	}
#endif /* defined(HAVE_ZLIB) */

	/* free RFC1143 queue */
	if (telnet->q) {
		free(telnet->q);
		telnet->q = 0;
		telnet->q_size = 0;
	}

	/* free the telnet structure itself */
	free(telnet);
}

/* push a byte into the telnet buffer */
static telnet_error_t _buffer_byte(telnet_t *telnet,
		unsigned char byte) {
	char *new_buffer;
	size_t i;

	/* check if we're out of room */
	if (telnet->buffer_pos == telnet->buffer_size) {
		/* find the next buffer size */
		for (i = 0; i != _buffer_sizes_count; ++i) {
			if (_buffer_sizes[i] == telnet->buffer_size) {
				break;
			}
		}

		/* overflow -- can't grow any more */
		if (i >= _buffer_sizes_count - 1) {
			_error(telnet, __LINE__, __func__, TELNET_EOVERFLOW, 0,
					"subnegotiation buffer size limit reached");
			return TELNET_EOVERFLOW;
		}

		/* (re)allocate buffer */
		new_buffer = (char *)realloc(telnet->buffer, _buffer_sizes[i + 1]);
		if (new_buffer == 0) {
			_error(telnet, __LINE__, __func__, TELNET_ENOMEM, 0,
					"realloc() failed");
			return TELNET_ENOMEM;
		}

		telnet->buffer = new_buffer;
		telnet->buffer_size = _buffer_sizes[i + 1];
	}

	/* push the byte, all set */
	telnet->buffer[telnet->buffer_pos++] = byte;
	return TELNET_EOK;
}

static void _process(telnet_t *telnet, const char *buffer, size_t size) {
	telnet_event_t ev;
	unsigned char byte;
	size_t i, start;
	for (i = start = 0; i != size; ++i) {
		byte = buffer[i];
		switch (telnet->state) {
		/* regular data */
		case TELNET_STATE_DATA:
			/* on an IAC byte, pass through all pending bytes and
			 * switch states */
			if (byte == TELNET_IAC) {
				if (i != start) {
					ev.type = telnet_event_type_t.TELNET_EV_DATA;
					ev.data.buffer = buffer + start;
					ev.data.size = i - start;
					telnet->eh(telnet, &ev, telnet->ud);
				}
				telnet->state = TELNET_STATE_IAC;
			}
			break;

		/* IAC command */
		case TELNET_STATE_IAC:
			switch (byte) {
			/* subnegotiation */
			case TELNET_SB:
				telnet->state = TELNET_STATE_SB;
				break;
			/* negotiation commands */
			case TELNET_WILL:
				telnet->state = TELNET_STATE_WILL;
				break;
			case TELNET_WONT:
				telnet->state = TELNET_STATE_WONT;
				break;
			case TelnetCommands.TELNET_DO:
				telnet->state = TELNET_STATE_DO;
				break;
			case TelnetCommands.TELNET_DONT:
				telnet->state = TELNET_STATE_DONT;
				break;
			/* IAC escaping */
			case TELNET_IAC:
				/* event */
				ev.type = telnet_event_type_t.TELNET_EV_DATA;
				ev.data.buffer = (char*)&byte;
				ev.data.size = 1;
				telnet->eh(telnet, &ev, telnet->ud);

				/* state update */
				start = i + 1;
				telnet->state = TELNET_STATE_DATA;
				break;
			/* some other command */
			default:
				/* event */
				ev.type = telnet_event_type_t.TELNET_EV_IAC;
				ev.iac.cmd = byte;
				telnet->eh(telnet, &ev, telnet->ud);

				/* state update */
				start = i + 1;
				telnet->state = TELNET_STATE_DATA;
			}
			break;

		/* negotiation commands */
		case TELNET_STATE_WILL:
		case TELNET_STATE_WONT:
		case TELNET_STATE_DO:
		case TELNET_STATE_DONT:
			_negotiate(telnet, byte);
			start = i + 1;
			telnet->state = TELNET_STATE_DATA;
			break;

		/* subnegotiation -- determine subnegotiation telopt */
		case TELNET_STATE_SB:
			telnet->sb_telopt = byte;
			telnet->buffer_pos = 0;
			telnet->state = TELNET_STATE_SB_DATA;
			break;

		/* subnegotiation -- buffer bytes until end request */
		case TELNET_STATE_SB_DATA:
			/* IAC command in subnegotiation -- either IAC SE or IAC IAC */
			if (byte == TELNET_IAC) {
				telnet->state = TELNET_STATE_SB_DATA_IAC;
			/* buffer the byte, or bail if we can't */
			} else if (_buffer_byte(telnet, byte) != TELNET_EOK) {
				start = i + 1;
				telnet->state = TELNET_STATE_DATA;
			}
			break;

		/* IAC escaping inside a subnegotiation */
		case TELNET_STATE_SB_DATA_IAC:
			switch (byte) {
			/* end subnegotiation */
			case TELNET_SE:
				/* return to default state */
				start = i + 1;
				telnet->state = TELNET_STATE_DATA;

				/* process subnegotiation */
				if (_subnegotiate(telnet) != 0) {
					/* any remaining bytes in the buffer are compressed.
					 * we have to re-invoke telnet_recv to get those
					 * bytes inflated and abort trying to process the
					 * remaining compressed bytes in the current _process
					 * buffer argument
					 */
					telnet_recv(telnet, &buffer[start], size - start);
					return;
				}
				break;
			/* escaped IAC byte */
			case TELNET_IAC:
				/* push IAC into buffer */
				if (_buffer_byte(telnet, TELNET_IAC) !=
						TELNET_EOK) {
					start = i + 1;
					telnet->state = TELNET_STATE_DATA;
				} else {
					telnet->state = TELNET_STATE_SB_DATA;
				}
				break;
			/* something else -- protocol error.  attempt to process
			 * content in subnegotiation buffer, then evaluate the
			 * given command as an IAC code.
			 */
			default:
				_error(telnet, __LINE__, __func__, TELNET_EPROTOCOL, 0,
						"unexpected byte after IAC inside SB: {0}",
						byte);

				/* enter IAC state */
				start = i + 1;
				telnet->state = TELNET_STATE_IAC;

				/* process subnegotiation; see comment in
				 * TELNET_STATE_SB_DATA_IAC about invoking telnet_recv()
				 */
				if (_subnegotiate(telnet) != 0) {
					telnet_recv(telnet, &buffer[start], size - start);
					return;
				} else {
					/* recursive call to get the current input byte processed
					 * as a regular IAC command.  we could use a goto, but
					 * that would be gross.
					 */
					_process(telnet, (char *)&byte, 1);
				}
				break;
			}
			break;
		}
	}

	/* pass through any remaining bytes */ 
	if (telnet->state == TELNET_STATE_DATA && i != start) {
		ev.type = telnet_event_type_t.TELNET_EV_DATA;
		ev.data.buffer = buffer + start;
		ev.data.size = i - start;
		telnet->eh(telnet, &ev, telnet->ud);
	}
}

/* push a bytes into the state tracker */
void telnet_recv(telnet_t *telnet, const char *buffer,
		size_t size) {
#if defined(HAVE_ZLIB)
	/* if we have an inflate (decompression) zlib stream, use it */
	if (telnet->z != 0 && !(telnet->flags & TELNET_PFLAG_DEFLATE)) {
		char inflate_buffer[1024];
		int rs;

		/* initialize zlib state */
		telnet->z->next_in = (unsigned char*)buffer;
		telnet->z->avail_in = size;
		telnet->z->next_out = (unsigned char *)inflate_buffer;
		telnet->z->avail_out = sizeof(inflate_buffer);

		/* inflate until buffer exhausted and all output is produced */
		while (telnet->z->avail_in > 0 || telnet->z->avail_out == 0) {
			/* reset output buffer */

			/* decompress */
			rs = inflate(telnet->z, Z_SYNC_FLUSH);

			/* process the decompressed bytes on success */
			if (rs == Z_OK || rs == Z_STREAM_END)
				_process(telnet, inflate_buffer, sizeof(inflate_buffer) -
						telnet->z->avail_out);
			else
				_error(telnet, __LINE__, __func__, TELNET_ECOMPRESS, 1,
						"inflate() failed: {0}", zError(rs));

			/* prepare output buffer for next run */
			telnet->z->next_out = (unsigned char *)inflate_buffer;
			telnet->z->avail_out = sizeof(inflate_buffer);

			/* on error (or on end of stream) disable further inflation */
			if (rs != Z_OK) {
				telnet_event_t ev;

				/* disable compression */
				inflateEnd(telnet->z);
				free(telnet->z);
				telnet->z = 0;

				/* send event */
				ev.type = telnet_event_type_t.TELNET_EV_COMPRESS;
				ev.compress.state = 0;
				telnet->eh(telnet, &ev, telnet->ud);

				break;
			}
		}

	/* COMPRESS2 is not negotiated, just process */
	} else
#endif /* defined(HAVE_ZLIB) */
		_process(telnet, buffer, size);
}

/* send an iac command */
void telnet_iac(telnet_t *telnet, unsigned char cmd) {
	unsigned char bytes[2];
	bytes[0] = TELNET_IAC;
	bytes[1] = cmd;
	_sendu(telnet, bytes, 2);
}

/* send negotiation */
void telnet_negotiate(telnet_t *telnet, unsigned char cmd,
		unsigned char telopt) {
	telnet_rfc1143_t q;

	/* if we're in proxy mode, just send it now */
	if (telnet->flags & TELNET_FLAG_PROXY) {
		unsigned char bytes[3];
		bytes[0] = TELNET_IAC;
		bytes[1] = cmd;
		bytes[2] = telopt;
		_sendu(telnet, bytes, 3);
		return;
	}
	
	/* get current option states */
	q = _get_rfc1143(telnet, telopt);

	switch (cmd) {
	/* advertise willingess to support an option */
	case TELNET_WILL:
		switch (q.Us) {
		case Q_NO:
			_set_rfc1143(telnet, telopt, Q_WANTYES, q.Him);
			_send_negotiate(telnet, TELNET_WILL, telopt);
			break;
		case Q_WANTNO:
			_set_rfc1143(telnet, telopt, Q_WANTNO_OP, q.Him);
			break;
		case Q_WANTYES_OP:
			_set_rfc1143(telnet, telopt, Q_WANTYES, q.Him);
			break;
		}
		break;

	/* force turn-off of locally enabled option */
	case TELNET_WONT:
		switch (q.Us) {
		case Q_YES:
			_set_rfc1143(telnet, telopt, Q_WANTNO, q.Him);
			_send_negotiate(telnet, TELNET_WONT, telopt);
			break;
		case Q_WANTYES:
			_set_rfc1143(telnet, telopt, Q_WANTYES_OP, q.Him);
			break;
		case Q_WANTNO_OP:
			_set_rfc1143(telnet, telopt, Q_WANTNO, q.Him);
			break;
		}
		break;

	/* ask remote end to enable an option */
	case TelnetCommands.TELNET_DO:
		switch (q.Him) {
		case Q_NO:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTYES);
			_send_negotiate(telnet, TelnetCommands.TELNET_DO, telopt);
			break;
		case Q_WANTNO:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTNO_OP);
			break;
		case Q_WANTYES_OP:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTYES);
			break;
		}
		break;

	/* demand remote end disable an option */
	case TelnetCommands.TELNET_DONT:
		switch (q.Him) {
		case Q_YES:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTNO);
			_send_negotiate(telnet, TelnetCommands.TELNET_DONT, telopt);
			break;
		case Q_WANTYES:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTYES_OP);
			break;
		case Q_WANTNO_OP:
			_set_rfc1143(telnet, telopt, q.Us, Q_WANTNO);
			break;
		}
		break;
	}
}

/* send non-command data (escapes IAC bytes) */
void telnet_send(telnet_t *telnet, const char *buffer,
		size_t size) {
	size_t i, l;

	for (l = i = 0; i != size; ++i) {
		/* dump prior portion of text, send escaped bytes */
		if (buffer[i] == (char)TELNET_IAC) {
			/* dump prior text if any */
			if (i != l) {
				_send(telnet, buffer + l, i - l);
			}
			l = i + 1;

			/* send escape */
			telnet_iac(telnet, TELNET_IAC);
		}
	}

	/* send whatever portion of buffer is left */
	if (i != l) {
		_send(telnet, buffer + l, i - l);
	}
}

/* send subnegotiation header */
void telnet_begin_sb(telnet_t *telnet, unsigned char telopt) {
	unsigned char sb[3];
	sb[0] = TELNET_IAC;
	sb[1] = TELNET_SB;
	sb[2] = telopt;
	_sendu(telnet, sb, 3);
}


/* send complete subnegotiation */
void telnet_subnegotiation(telnet_t *telnet, unsigned char telopt,
		const char *buffer, size_t size) {
	unsigned char bytes[5];
	bytes[0] = TELNET_IAC;
	bytes[1] = TELNET_SB;
	bytes[2] = telopt;
	bytes[3] = TELNET_IAC;
	bytes[4] = TELNET_SE;

	_sendu(telnet, bytes, 3);
	telnet_send(telnet, buffer, size);
	_sendu(telnet, bytes + 3, 2);

#if defined(HAVE_ZLIB)
	/* if we're a proxy and we just sent the COMPRESS2 marker, we must
	 * make sure all further data is compressed if not already.
	 */
	if (telnet->flags & TELNET_FLAG_PROXY &&
			telopt == TELNET_TELOPT_COMPRESS2) {
		telnet_event_t ev;

		if (_init_zlib(telnet, 1, 1) != TELNET_EOK)
			return;

		/* notify app that compression was enabled */
		ev.type = telnet_event_type_t.TELNET_EV_COMPRESS;
		ev.compress.state = 1;
		telnet->eh(telnet, &ev, telnet->ud);
	}
#endif /* defined(HAVE_ZLIB) */
}

void telnet_begin_compress2(telnet_t *telnet) {
#if defined(HAVE_ZLIB)
	static const unsigned char compress2[] = { TELNET_IAC, TELNET_SB,
			TELNET_TELOPT_COMPRESS2, TELNET_IAC, TELNET_SE };

	telnet_event_t ev;

	/* attempt to create output stream first, bail if we can't */
	if (_init_zlib(telnet, 1, 0) != TELNET_EOK)
		return;

	/* send compression marker.  we send directly to the event handler
	 * instead of passing through _send because _send would result in
	 * the compress marker itself being compressed.
	 */
	ev.type = telnet_event_type_t.TELNET_EV_SEND;
	ev.data.buffer = (const char*)compress2;
	ev.data.size = sizeof(compress2);
	telnet->eh(telnet, &ev, telnet->ud);

	/* notify app that compression was successfully enabled */
	ev.type = telnet_event_type_t.TELNET_EV_COMPRESS;
	ev.compress.state = 1;
	telnet->eh(telnet, &ev, telnet->ud);
#endif /* defined(HAVE_ZLIB) */
}

/* send formatted data with \r and \n translation in addition to IAC IAC */
int telnet_printf(telnet_t *telnet, const char *fmt, ...) {
    static const char CRLF[] = { '\r', '\n' };
    static const char CRNUL[] = { '\r', '\0' };
	char buffer[1024];
	char *output = buffer;
	va_list va;
	int rs, i, l;

	/* format */
	va_start(va, fmt);
	rs = vsnprintf(buffer, sizeof(buffer), fmt, va);
	if (rs >= sizeof(buffer)) {
		output = (char*)malloc(rs + 1);
		if (output == 0) {
			_error(telnet, __LINE__, __func__, TELNET_ENOMEM, 0,
					"malloc() failed: {0}", strerror(errno));
			return -1;
		}
		rs = vsnprintf(output, rs + 1, fmt, va);
	}
	va_end(va);

	/* send */
	for (l = i = 0; i != rs; ++i) {
		/* special characters */
		if (output[i] == (char)TELNET_IAC || output[i] == '\r' ||
				output[i] == '\n') {
			/* dump prior portion of text */
			if (i != l)
				_send(telnet, output + l, i - l);
			l = i + 1;

			/* IAC -> IAC IAC */
			if (output[i] == (char)TELNET_IAC)
				telnet_iac(telnet, TELNET_IAC);
			/* automatic translation of \r -> CRNUL */
			else if (output[i] == '\r')
				_send(telnet, CRNUL, 2);
			/* automatic translation of \n -> CRLF */
			else if (output[i] == '\n')
				_send(telnet, CRLF, 2);
		}
	}

	/* send whatever portion of output is left */
	if (i != l) {
		_send(telnet, output + l, i - l);
	}

	/* free allocated memory, if any */
	if (output != buffer) {
		free(output);
	}

	return rs;
}

/* send formatted data through telnet_send */
int telnet_raw_printf(telnet_t *telnet, const char *fmt, ...) {
	char buffer[1024];
	char *output = buffer;
	va_list va;
	int rs;

	/* format; allocate more space if necessary */
	va_start(va, fmt);
	rs = vsnprintf(buffer, sizeof(buffer), fmt, va);
	if (rs >= sizeof(buffer)) {
		output = (char*)malloc(rs + 1);
		if (output == 0) {
			_error(telnet, __LINE__, __func__, TELNET_ENOMEM, 0,
					"malloc() failed: {0}", strerror(errno));
			return -1;
		}
		rs = vsnprintf(output, rs + 1, fmt, va);
	}
	va_end(va);

	/* send out the formatted data */
	telnet_send(telnet, output, rs);

	/* release allocated memory, if any */
	if (output != buffer) {
		free(output);
	}

	return rs;
}

/* begin NEW-ENVIRON subnegotation */
void telnet_begin_newenviron(telnet_t *telnet, unsigned char cmd) {
	telnet_begin_sb(telnet, TELNET_TELOPT_NEW_ENVIRON);
}

/* send a NEW-ENVIRON value */
void telnet_newenviron_value(telnet_t *telnet, unsigned char type,
		const char *string) {
	telnet_send(telnet, (char*)&type, 1);

	if (string != 0) {
		telnet_send(telnet, string, strlen(string));
	}
}

/* send TERMINAL-TYPE SEND command */
void telnet_ttype_send(telnet_t *telnet) {
    static const unsigned char SEND[] = { TELNET_IAC, TELNET_SB,
			TELNET_TELOPT_TTYPE, TELNET_TTYPE_SEND, TELNET_IAC, TELNET_SE };
	_sendu(telnet, SEND, sizeof(SEND));
}

/* send TERMINAL-TYPE IS command */
void telnet_ttype_is(telnet_t *telnet, const char* ttype) {
	static const unsigned char IS[] = { TELNET_IAC, TELNET_SB,
			TELNET_TELOPT_TTYPE, TELNET_TTYPE_IS };
	_sendu(telnet, IS, sizeof(IS));
	_send(telnet, ttype, strlen(ttype));
	telnet_finish_sb(telnet);
}

/* send ZMP data */
void telnet_send_zmp(telnet_t *telnet, size_t argc, const char **argv) {
	size_t i;

	/* ZMP header */
	telnet_begin_zmp(telnet, argv[0]);

	/* send out each argument, including trailing NUL byte */
	for (i = 1; i != argc; ++i)
		telnet_zmp_arg(telnet, argv[i]);

	/* ZMP footer */
	telnet_finish_zmp(telnet);
}

/* send ZMP data using varargs  */
void telnet_send_zmpv(telnet_t *telnet, ...) {
	va_list va;
	const char* arg;

	/* ZMP header */
	telnet_begin_sb(telnet, TELNET_TELOPT_ZMP);

	/* send out each argument, including trailing NUL byte */
	va_start(va, telnet);
	while ((arg = va_arg(va, const char *)) != 0)
		telnet_zmp_arg(telnet, arg);
	va_end(va);

	/* ZMP footer */
	telnet_finish_zmp(telnet);
}

/* begin a ZMP command */
void telnet_begin_zmp(telnet_t *telnet, const char *cmd) {
	telnet_begin_sb(telnet, TELNET_TELOPT_ZMP);
	telnet_zmp_arg(telnet, cmd);
}

/* send a ZMP argument */
void telnet_zmp_arg(telnet_t *telnet, const char* arg) {
	telnet_send(telnet, arg, strlen(arg) + 1);
}
}
}
