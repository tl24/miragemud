using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using JsonExSerializer;

namespace Mirage.Game.Communication
{
    /// <summary>
    /// Factory class for creating well known message types
    /// </summary>
    public class MessageFactory : IMessageFactory
    {
        private IDictionary<string, NamespaceGroup> _namespaces;

        public MessageFactory()
        {
            _namespaces = new Dictionary<string, NamespaceGroup>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Constructs and returns the message identified by the message uri
        /// </summary>
        /// <param name="messageUri">the uri that identifies the message</param>
        /// <returns>the message</returns>
        public IMessage GetMessage(string messageName)
        {            
            string name;
            string nmspace = SeparateUri(messageName, out name);
            NamespaceGroup ngroup = null;
            if (!_namespaces.TryGetValue(nmspace.ToString(), out ngroup))
            {
                ngroup = LoadNamespace(nmspace);                    
            }
            return ngroup.CreateMessage(name);
        }

        public IMessage GetMessage(string messageName, string messageText)
        {
            return GetMessage(new MessageName(messageName), messageText);
        }

        public IMessage GetMessage(MessageName name, string messageText)
        {
            MessageType type = MessageType.Information;
            if (name.Namespace.Contains("error"))
                type = MessageType.PlayerError;
            return GetMessage(type, name, messageText);
        }

        public IMessage GetMessage(MessageType type, string messageName, string messageText)
        {
            return GetMessage(type, new MessageName(messageName), messageText);
        }

        public IMessage GetMessage(MessageType type, MessageName name, string messageText)
        {
            if (messageText.Contains("${"))
            {
                ResourceMessage msg = new ResourceMessage(type, name.FullName);
                msg.Template = messageText;
                return msg;
            }
            else
            {
                return new StringMessage(type, name, messageText);
            }
        }

        /// <summary>
        /// Loads a namespace containing messages from the file
        /// </summary>
        /// <param name="Namespace">the namespace uri to load</param>
        /// <returns>namespace group</returns>
        private NamespaceGroup LoadNamespace(string Namespace)
        {
            string namespaceFile = "";
            try
            {
                NameValueCollection namespaces = (NameValueCollection)ConfigurationManager.GetSection("MirageMUD/MessageNamespaces");
                namespaceFile = namespaces[Namespace.ToString()];
                Serializer serializer = new Serializer(typeof(NamespaceGroup), "JsonMessageFactory");

                NamespaceGroup result = null;
                using (StreamReader reader = new StreamReader(namespaceFile))
                {
                    result = (NamespaceGroup)serializer.Deserialize(reader);
                }
                _namespaces[Namespace.ToString()] = result;
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error occurred loading namespace " + Namespace + " from file: " + namespaceFile + " " + e.Message, e); 
            }
        }

        /// <summary>
        /// This will clear all cached messages and namespaces
        /// </summary>
        public void Clear()
        {
            _namespaces.Clear();
        }

        /// <summary>
        /// Separates a uri into its namespace and name
        /// </summary>
        /// <param name="path">the uri to separate</param>
        /// <param name="name">the name part of the uri</param>
        /// <returns>the namespace</returns>
        private string SeparateUri(string path, out string name)
        {
            int pos = path.LastIndexOf('.');
            if (pos >= 0)
            {
                name = path.Substring(pos + 1);
                return path.Substring(0, pos);
            }
            else
            {
                name = path;
                return "";
            }
        }
    }

    public class NamespaceGroup 
    {
        private string _namespace;
        private Dictionary<string, IMessage> _messages = new Dictionary<string, IMessage>();

        public string Namespace
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
            newMessage.Name = new MessageName(this.Namespace, newMessage.Name.Name);
            return newMessage;
        }

    }

    
}
