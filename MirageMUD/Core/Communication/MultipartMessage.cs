using System;
using System.Collections;
using System.Text;

namespace Mirage.Core.Communication
{

    /// <summary>
    /// A message consisting of multiple parts, each part is a subclass of Message.
    /// </summary>
    public class MultipartMessage : Message
    {
        private ArrayList _parts;

        public MultipartMessage()
        {
        }

        public MultipartMessage(MessageType messageType, Uri Namespace, string name) : this(messageType, Namespace, name, null) { }

        public MultipartMessage(MessageType messageType, Uri Namespace, string name, ICollection parts)
            : base(messageType, Namespace, name)
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

        public ArrayList Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (IMessage part in _parts)
            {
                sb.Append(part.ToString());
                //TODO: Add Line Endings?
            }

            return sb.ToString();
        }

        protected override IMessage MakeCopy()
        {
            return new MultipartMessage(MessageType, Namespace, Name);
        }
    }


}
