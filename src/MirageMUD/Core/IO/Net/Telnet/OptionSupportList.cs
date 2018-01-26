using System.Collections.Generic;

namespace Mirage.Core.IO.Net.Telnet
{
    public class OptionSupportList
    {
        Dictionary<byte, OptionSupportEntry> _options = new Dictionary<byte, OptionSupportEntry>();
        public OptionSupportList()
        {
        }

        public OptionSupportList(IEnumerable<OptionSupportEntry> options)
        {
            AddRange(options);
        }

        public void AddRange(IEnumerable<OptionSupportEntry> options)
        {
            foreach (var option in options)
                Add(option);
        }

        public void Add(OptionSupportEntry option)
        {
            _options[option.TelnetOption] = option;
        }

        public bool IsSupportedLocally(byte telnetOption)
        {
            OptionSupportEntry option;
            if (_options.TryGetValue(telnetOption, out option))
            {
                return option.SupportedLocally;
            }
            return false;
        }

        public bool IsSupportedRemotely(byte telnetOption)
        {
            OptionSupportEntry option;
            if (_options.TryGetValue(telnetOption, out option))
            {
                return option.SupportedRemotely;
            }
            return false;
        }

    }
}
