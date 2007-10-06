using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using JsonExSerializer;
using System.IO;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Factory class for creating well known message types
    /// </summary>
    public static class MessageFactory
    {
        private static IDictionary<string, NamespaceGroup> _namespaces;

        static MessageFactory()
        {
            _namespaces = new Dictionary<string, NamespaceGroup>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the given message constant
        /// </summary>
        /// <param name="key">the key of the message</param>
        /// <returns>the message</returns>
        public static IMessage GetMessage(string uri)
        {
            Uri msgUri = new Uri(uri);
            string name;
            Uri nmspace = SeparateUri(msgUri, out name);
            NamespaceGroup ngroup = null;
            if (!_namespaces.TryGetValue(nmspace.ToString(), out ngroup))
            {
                ngroup = LoadNamespace(nmspace);                    
            }
            return ngroup.CreateMessage(name);
        }

        /// <summary>
        /// Loads a namespace containing messages from the file
        /// </summary>
        /// <param name="Namespace">the namespace uri to load</param>
        /// <returns>namespace group</returns>
        private static NamespaceGroup LoadNamespace(Uri Namespace)
        {
            NameValueCollection namespaces = (NameValueCollection)ConfigurationManager.GetSection("MirageMUD/MessageNamespaces");
            string value = namespaces[Namespace.ToString()];
            Serializer serializer = Serializer.GetSerializer(typeof(NamespaceGroup), "JsonMessageFactory");
            //serializer.Context.AddTypeBinding(typeof(StringMessage), "StringMessage");
            //serializer.Context.AddTypeBinding(typeof(ResourceMessage), "ResourceMessage");

            NamespaceGroup result = null;
            using (StreamReader reader = new StreamReader(value))
            {
                result = (NamespaceGroup) serializer.Deserialize(reader);
            }
            _namespaces[Namespace.ToString()] = result;
            return result;
        }

        /// <summary>
        /// Separates a uri into its namespace and name
        /// </summary>
        /// <param name="path">the uri to separate</param>
        /// <param name="name">the name part of the uri</param>
        /// <returns>the namespace</returns>
        private static Uri SeparateUri(Uri path, out string name)
        {
            name = path.Segments[path.Segments.Length - 1];
            return new Uri(path.Scheme + ":" + string.Join("", path.Segments, 0, path.Segments.Length - 1));
        }

        //public const string EchoOn = "EchoOn";
        public const string EchoOn = "msg:/system/EchoOn";
        //public const string EchoOff = "EchoOff";
        public const string EchoOff = "msg:/system/EchoOff";
    }

    public class NamespaceGroup 
    {
        private Uri _namespace;
        private Dictionary<string, IMessage> _messages = new Dictionary<string, IMessage>();

        public System.Uri Namespace
        {
            get { return this._namespace; }
            set { this._namespace = value; }
        }

        public Dictionary<string, IMessage> Messages
        {
            get { return this._messages; }
            set { this._messages = value; }
        }

        public IMessage CreateMessage(string Name)
        {
            IMessage newMessage = _messages[Name].Copy();
            newMessage.Namespace = this.Namespace;
            return newMessage;
        }

    }

    
}