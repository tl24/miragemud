
namespace Mirage.IO.Net.Telnet.Options
{
    public class TermTypeEventArgs : SubNegotiationEventArgs
    {
        public TermTypeEventArgs(byte subOptionstring, string name)
            : base(OptionCodes.TTYPE)
        {
            this.Name = name;
        }

        /// <summary>
        /// The name of the terminal type
        /// </summary>
        public string Name { get; protected set; }

    }

    public class TermTypeOption : TelnetOption
    {
        public TermTypeOption(TelnetOptionProcessor parent)
            : base(parent, OptionCodes.TTYPE)
        {
        }

        public override void OnSubNegotiation(byte[] subData)
        {
            /* make sure request is not empty */
            if (subData.Length == 0)
            {
                Parent.LogLine("incomplete TERMINAL-TYPE request");
                return;
            }

            /* make sure request has valid command type */
            if (subData[0] != TelnetSubOptionCodes.TELNET_TTYPE_IS &&
                    subData[0] != TelnetSubOptionCodes.TELNET_TTYPE_SEND)
            {
                Parent.LogLine("TERMINAL-TYPE request has invalid type: " + subData[0]);
                return;
            }

            /* send proper event */
            if (subData[0] == TelnetSubOptionCodes.TELNET_TTYPE_IS)
            {
                string name = Parent.BytesToString(subData, 1, subData.Length - 1);
                Parent.OnSubNegotiationOccurred(new TermTypeEventArgs(TelnetSubOptionCodes.TELNET_TTYPE_IS, name));
            }
            else
            {
                Parent.OnSubNegotiationOccurred(new TermTypeEventArgs(TelnetSubOptionCodes.TELNET_TTYPE_SEND, null));
            }
        }

        public override void OnOptionChanged(bool enabled, bool local)
        {
            base.OnOptionChanged(enabled, local);
            if (enabled)
            {
                Parent.SendSubNegotiate(new byte[] { OptionCodes.TTYPE, TelnetSubOptionCodes.TELNET_TTYPE_SEND });
            }
        }
    }
}
