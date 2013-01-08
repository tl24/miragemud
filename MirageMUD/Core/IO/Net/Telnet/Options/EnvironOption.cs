using System.Collections.Generic;

namespace Mirage.Core.IO.Net.Telnet.Options
{
    public struct EnvironValue
    {
        public static EnvironValue Empty = new EnvironValue();

        public bool IsUserValue;
        public string Variable;
        public string Value;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(EnvironValue other)
        {
            return IsUserValue == other.IsUserValue
                && Variable == other.Variable
                && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return (IsUserValue.ToString() + Variable + Value).GetHashCode();
        }

        public static bool operator ==(EnvironValue a, EnvironValue b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EnvironValue a, EnvironValue b)
        {
            return !(a == b);
        }
    }

    public class EnvironEventArgs : SubNegotiationEventArgs
    {
        public EnvironEventArgs(byte environType, IList<EnvironValue> values)
            : base(OptionCodes.ENVIRON)
        {
            this.EnvironTypeCode = environType;
            this.Values = values;
        }

        /// <summary>
        /// The byte code for the eviron command: SEND, IS, or INFO
        /// </summary>
        public byte EnvironTypeCode { get; private set; }


        /// <summary>
        /// Environment values
        /// </summary>
        public IList<EnvironValue> Values { get; protected set; }
    }

    public class EnvironOption : TelnetOption
    {
        public EnvironOption(TelnetOptionProcessor parent)
            : base(parent, OptionCodes.ENVIRON)
        {
        }

        /// <summary>
        /// process an ENVIRON/NEW-ENVIRON subnegotiation buffer         
        /// </summary>
        public override void OnSubNegotiation(byte[] subData)
        {
       // static int _environ(telnet_t telnet, byte type, byte[] buffer) {

            var values = new List<EnvironValue>();

	        // if we have no data, just pass it through
            if (subData.Length == 0)
            {
                return;
	        }
            
	        // first byte must be a valid command
            if (subData[0] != TelnetSubOptionCodes.TELNET_ENVIRON_SEND &&
                    subData[0] != TelnetSubOptionCodes.TELNET_ENVIRON_IS &&
                    subData[0] != TelnetSubOptionCodes.TELNET_ENVIRON_INFO)
            {
                Parent.AppendLog(string.Format("telopt {0} subneg has invalid command", this.OptionCode));
		        return;
	        }

            byte cmd = subData[0];
	        // store ENVIRON command 
        	

	        // if we have no arguments, send an event with no data end return
            if (subData.Length == 1)
            {
                Parent.OnSubNegotiationOccurred(new EnvironEventArgs(cmd, values));
		        return;
	        }

	        /* very second byte must be VAR or USERVAR, if present */
            if (subData[1] != TelnetSubOptionCodes.TELNET_ENVIRON_VAR &&
                    subData[1] != TelnetSubOptionCodes.TELNET_ENVIRON_USERVAR)
            {
                Parent.OnProtocolError(string.Format("telopt {0} subneg missing variable type", cmd));
		        return;
	        }

	        /* ensure last byte is not an escape byte (makes parsing later easier) */
            if (subData[subData.Length - 1] == TelnetSubOptionCodes.TELNET_ENVIRON_ESC)
            {
                Parent.OnProtocolError(string.Format("telopt {0} subneg ends with ESC", cmd));
                return;
	        }


            byte[] destBuffer = new byte[subData.Length];
            int destIndex = 0;
            int bufIndex = 0;
            EnvironValue current = EnvironValue.Empty;
            while (bufIndex++ < subData.Length)
            {
                byte currByte = subData[bufIndex];
                if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_ESC) {
                    bufIndex++;
                    continue;
                } else if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_USERVAR || currByte == TelnetSubOptionCodes.TELNET_ENVIRON_VAR) {
                    // new variable
                    if (current != EnvironValue.Empty) {
                        if (destIndex == 0)
                            current.Value = "";
                        else {
                            current.Value = Parent.BytesToString(destBuffer,destIndex);
                        }                
                        values.Add(current);
                    }
                    destIndex = 0;
                    current = new EnvironValue();
                    current.IsUserValue = currByte == TelnetSubOptionCodes.TELNET_ENVIRON_USERVAR;
                } else if (currByte == TelnetSubOptionCodes.TELNET_ENVIRON_VALUE) {
                    if (destIndex == 0)
                        current.Variable = "";
                    else {
                        current.Variable = Parent.BytesToString(destBuffer, destIndex);
                    }
                    destIndex = 0;
                } else {
                    destBuffer[destIndex++] = currByte;
                }
            }
            if (current != EnvironValue.Empty) {
                current.Variable = current.Variable ?? "";
                current.Value = current.Value ?? "";
                if (destIndex > 0) {
                    if (string.IsNullOrEmpty(current.Variable)) {
                        current.Variable = Parent.BytesToString(destBuffer, destIndex);
                    } else {
                        current.Value = Parent.BytesToString(destBuffer, destIndex);
                    }
                }
                values.Add(current);
            }

            Parent.OnSubNegotiationOccurred(new EnvironEventArgs(cmd, values));
	        return;        
        }
    }
}
