using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.Communication
{
    public class MessageDefinition
    {
        public MessageDefinition()
        {
        }

        public MessageDefinition(string name, string text)
        {
            this.Name = name;
            this.Text = text;
        }

        public MessageDefinition(MessageType type, string name, string text) : this(name, text)
        {
            this.MessageType = type;
        }


        public string Name { get; set; }
        public string Text { get; set; }
        public MessageType MessageType { get; set; }
    }
}
