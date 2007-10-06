using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;

namespace Mirage.Core.Communication
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
            return Render();
        }

        public override string Render()
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
            return new StringMessage(this.MessageType, this.Namespace, this.Name, this.Render());
        }

        protected override IMessage MakeCopy()
        {
            if (this.GetType() != typeof(ResourceMessage))
                throw new Exception("Subclass must override the copy method");

            return new ResourceMessage(MessageType, Namespace, Name);
        }
    }
}
