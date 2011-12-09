using System;
using System.Collections.Generic;

namespace Mirage.Game.Communication
{

    /// <summary>
    /// Consructs a message from the resource template
    /// </summary>
    public class ResourceMessage : Message
    {
        private TemplateDefinition templateDefinition;

        public ResourceMessage()
        {
        }

        protected ResourceMessage(MessageType messageType, MessageName name)
            : base(messageType, name)
        {
        }

        public ResourceMessage(MessageType messageType, string messageName)
            : base(messageType, messageName)
        {
        }

        /// <summary>
        /// Creates a new resource message using the given template
        /// </summary>
        /// <param name="messageType">the message type</param>
        /// <param name="Namespace">the namespace</param>
        /// <param name="messageName">the message name</param>
        /// <param name="template">the template to use</param>
        protected ResourceMessage(MessageType messageType, MessageName messageName, TemplateDefinition template)
            :this(messageType, messageName)
        {
            this.templateDefinition = template;
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

            ITemplate template = null;
            if (templateDefinition != null)
                template = new TemplateRenderer(templateDefinition, null);
            else
                template = TemplateManager.GetTemplate(this.Name.FullName);

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
            return new StringMessage(this.MessageType, this.Name, this.Render());
        }

        protected override IMessage MakeCopy()
        {
            if (this.GetType() != typeof(ResourceMessage))
                throw new Exception("Subclass must override the copy method");

            ResourceMessage copy = new ResourceMessage(MessageType, Name, templateDefinition);
            if (this.Parameters.Count > 0)
                copy.Parameters = this.Parameters;
            return copy;
        }

        /// <summary>
        /// The template for this message
        /// </summary>
        public string Template
        {
            get { return (templateDefinition != null) ? templateDefinition.Text : null; }
            set { templateDefinition = new TemplateDefinition(Name.Name, value, false); }
        }
    }
}
