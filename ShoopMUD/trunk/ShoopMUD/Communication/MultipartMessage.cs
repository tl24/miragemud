using System;
using System.Collections;
using System.Text;

namespace Shoop.Communication
{

    /// <summary>
    /// A message consisting of multiple parts, each part is a subclass of Message.
    /// </summary>
    public class MultipartMessage : Message
    {
        private ArrayList _parts;

        public MultipartMessage(MessageType messageType, string name) : this(messageType, name, null) { }

        public MultipartMessage(MessageType messageType, string name, ICollection parts)
            : base(messageType, name)
        {
            if (parts != null)
            {
                _parts = new ArrayList(parts);
            }
            else
            {
                _parts = new ArrayList();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (Message part in _parts)
            {
                sb.Append(part.ToString());
                //TODO: Add Line Endings?
            }

            return sb.ToString();
        }
    }


}
