using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace Mirage.Communication
{

    /// <summary>
    /// Consructs a message from the resource template
    /// </summary>
    public class ResourceMessage : Message /*, IJsonTypeConverter*/
    {
        private IDictionary<string, object> _parameters;

        public ResourceMessage()
        {
        }

        /// <summary>
        /// Constructs the message for the given resource.  The resourceName will be
        /// used to call TemplateManager.GetTemplate to retrieve the template.
        /// </summary>
        /// <param name="messageType">the type of the message</param>
        /// <param name="messageName">the name of the message</param>
        /// <param name="resourceName">the name of the resource template</param>
        public ResourceMessage(MessageType messageType, Uri Namespace, string messageName)
            : this(messageType, Namespace, messageName, new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Constructs a resource message for the given resource and
        /// replacement parameters.
        /// </summary>
        /// <param name="messageType">the type of the message</param>
        /// <param name="messageName">the name of the message</param>
        /// <param name="resourceName">the name of the resource template</param>
        /// <param name="parameters">the replacement parameters for the template</param>
        public ResourceMessage(MessageType messageType, Uri Namespace, string messageName, IDictionary<string, object> parameters)
            : base(messageType, Namespace, messageName)
        {
            this._parameters = parameters;
        }

        /// <summary>
        /// The replacement parameters for the template
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get { return this._parameters; }
            set { this._parameters = value; }
        }

        /// <summary>
        /// Renders the message
        /// </summary>
        /// <returns>string representation of the message</returns>
        public override string ToString()
        {
            return RenderMessage();
        }

        public override string RenderMessage()
        {
            ITemplate template = TemplateManager.GetTemplate(QualifiedName);
            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in Parameters)
                {
                    template[pair.Key] = pair.Value;
                }
            }
            return template.Render();
        }

        public override bool CanTransferMessage
        {
            get { return false; }
        }

        public override IMessage GetTransferMessage()
        {
            return new StringMessage(this.MessageType, this.Namespace, this.Name, this.RenderMessage());
        }

        /*
        #region IJsonTypeConverter Members

        public object Context
        {
            set { return; }
        }

        public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
        {
            return new StringMessage(this.MessageType, this.Name, this.ToString());
        }

        public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
        {
            // one-way conversion, just return the string message
            return item;
        }

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(StringMessage);
        }

        #endregion
         */ 
    }
}
