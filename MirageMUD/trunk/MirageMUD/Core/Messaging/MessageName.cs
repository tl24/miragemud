using System;
using JsonExSerializer;
using JsonExSerializer.TypeConversion;

namespace Mirage.Core.Messaging
{
    /// <summary>
    /// The name for a message, consisting of a namespace and name
    /// </summary>
    [JsonConvert(typeof(StringConverter))]
    public struct MessageName
    {
        public static MessageName Empty = new MessageName();

        private string _fullName;
        private const char NamespaceSeparator = '.';

        /// <summary>
        /// Constructs a message name instance from a fully-qualified name including namespace
        /// </summary>
        /// <param name="FullName"></param>
        public MessageName(string FullName)
        {
            _fullName = FullName;
        }

        /// <summary>
        /// Constructs a message name instance from namespace and name
        /// </summary>
        /// <param name="Namespace">namespace for the message</param>
        /// <param name="Name">the name of the message</param>
        public MessageName(string Namespace, string Name)
        {
            if (!String.IsNullOrEmpty(Namespace))
                _fullName = Namespace + NamespaceSeparator + Name;
            else
                _fullName = Name;
        }

        /// <summary>
        /// The message name without the namespace
        /// </summary>
        public string Name
        {
            get { return GetName(); }
        }

        /// <summary>
        /// The fully-qualified name of the message including the namespace
        /// </summary>
        [ConstructorParameter]
        public string FullName
        {
            get { return _fullName; }
        }

        /// <summary>
        /// The namespace for the message
        /// </summary>
        public string Namespace
        {
            get { return GetNamespace(); }
        }

        private string GetName()
        {
            int pos = _fullName.LastIndexOf(NamespaceSeparator);
            if (pos >= 0)
                return _fullName.Substring(pos + 1);
            else
                return _fullName;
        }

        private string GetNamespace()
        {
            int pos = _fullName.LastIndexOf(NamespaceSeparator);
            if (pos >= 0)
                return _fullName.Substring(0,pos);
            else
                return String.Empty;
        }

        /// <summary>
        /// Compare 2 messages for equality.  Two message names are equal if their fully qualified names are equal (case-insensitive)
        /// </summary>
        /// <param name="name">the message name to compare</param>
        /// <returns></returns>
        public bool Equals(MessageName name)
        {
            return _fullName.Equals(name._fullName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageName)
                return Equals((MessageName)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _fullName.GetHashCode();
        }
        public bool IsPartOfNamespace(string Namespace)
        {
            return this.Namespace.StartsWith(Namespace);
        }

        public bool IsSameAs(string FullName)
        {
            return new MessageName(FullName).Equals(this);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
