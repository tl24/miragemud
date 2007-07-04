using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{

    /// <summary>
    /// Consructs a message from the resource template
    /// </summary>
    public class ResourceMessage : Message
    {
        private string _resourceName;
        private IDictionary<string, object> _parameters;


        /// <summary>
        /// Constructs the message for the given resource.  The resourceName will be
        /// used to call TemplateManager.GetTemplate to retrieve the template.
        /// </summary>
        /// <param name="messageType">the type of the message</param>
        /// <param name="messageName">the name of the message</param>
        /// <param name="resourceName">the name of the resource template</param>
        public ResourceMessage(MessageType messageType, string messageName, string resourceName)
            : this(messageType, messageName, resourceName, new Dictionary<string, object>())
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
        public ResourceMessage(MessageType messageType, string messageName, string resourceName, IDictionary<string, object> parameters)
            : base(messageType, messageName)
        {
            this._resourceName = resourceName;
            this._parameters = parameters;
        }

        /// <summary>
        /// The name of the resource template
        /// </summary>
        public string ResourceName
        {
            get { return this._resourceName; }
            set { this._resourceName = value; }
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
            ITemplate template = TemplateManager.GetTemplate(ResourceName);
            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in Parameters)
                {
                    template[pair.Key] = pair.Value;
                }
            }
            return template.Render();
        }

    }
}
