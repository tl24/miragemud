using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Telnet
{
    /// <summary>
    /// Base telnet option class which handles state for a single option.  Derived classes
    /// provide custom behavior for specific options such as sub negotiation
    /// </summary>
    public class TelnetOption 
    {
        private int state = 0;

        public TelnetOption(TelnetOptionProcessor parent, TelnetCommands optionCode) : this(parent, (byte) optionCode)
        {
        }

        public TelnetOption(TelnetOptionProcessor parent, byte optionCode)
        {
            this.Parent = parent;
            this.OptionCode = optionCode;
        }

        public byte OptionCode { get; private set; }

        public TelnetOptionProcessor Parent { get; private set; }

        /// <summary>
        /// The state of the option on the local end (Us value)
        /// </summary>
        public QState LocalState
        {
            get
            {
                return (QState)(state >> 4);
            }
            set
            {
                state = ((int)value) << 4 | (state & 0x0F);

            }
        }
        /// <summary>
        /// The state of the option on the remote end (Him value)
        /// </summary>
        public QState RemoteState
        {
            get
            {
                return (QState)(state & 0x0F);
            }
            set
            {
                state = ((state << 4) & 0xF0) | (((int)value) & 0x0F);
            }
        }

        /// <summary>
        /// Called when the state of an option changes
        /// </summary>
        /// <param name="enabled">true if the option was enabled, false if disabled</param>
        /// <param name="local">true if the change in state is for the local side of the option or remote</param>
        public virtual void OnOptionChanged(bool enabled, bool local)
        {
            Parent.OnOptionStateChanged(new OptionStateChangedEventArgs(OptionCode, enabled, local));
        }

        protected void SendResponse(TelnetCommands optionCode)
        {
            Parent.LogLine(OptionCode.ToString("d"));
            Parent.LogLine(string.Format("Sending IAC {0:g} {1:d}", optionCode, OptionCode));
            Parent.WriteRaw(new byte[] { (byte)TelnetCommands.IAC, (byte)optionCode, OptionCode });
        }

        public virtual void OnSubNegotiation(byte[] subData)
        {
            Parent.LogLine(string.Format("Option {0} does not support sub negotiation.  Received Byte Data: {1}", OptionCode, subData));

        }
    }

}
