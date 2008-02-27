using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;

namespace Mirage.Core.Communication
{

    /// <summary>
    /// Error message returned when a player invokes a command incorrectly or from
    /// an invalid state.  The exception will contain the suggested error message uri
    /// to get from the message factory.
    /// </summary>
    public class ValidationException : Exception
    {
        private IMessage messageObject;

        /// <summary>
        /// Constructs the validation exception with the given message uri
        /// </summary>
        /// <param name="messageUri">message uri</param>
        public ValidationException(string messageUri) 
            : this(messageUri, null)
        {
        }

        /// <summary>
        /// Constructs the validation exception with the given message uri and inner exception
        /// </summary>
        /// <param name="messageUri">message uri</param>
        /// <param name="innerException">the exception that is the cause of the current exception, or null if there is no inner exception</param>
        public ValidationException(string messageUri, Exception innerException)
            : this(MudFactory.GetObject<IMessageFactory>().GetMessage(messageUri), innerException)
        {
        }

        /// <summary>
        /// Constructs the validation exception with the given message
        /// </summary>
        /// <param name="message">message</param>
        public ValidationException(IMessage message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Constructs the validation exception with the given message and inner exception
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="innerException">the exception that is the cause of the current exception, or null if there is no inner exception</param>
        public ValidationException(IMessage message, Exception innerException)
            : base(null, innerException)
        {
            messageObject = message;
        }


        /// <summary>
        /// Constructor for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ValidationException(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Text for the exception
        /// </summary>
        public override string Message
        {
            get
            {
                return MessageObject.Render();
            }
        }
        
        /// <summary>
        /// The fully qualified message uri for this exception
        /// </summary>
        public string MessageUri
        {
            get { return MessageObject.Name.FullName; }
        }

        /// <summary>
        /// Gets a message object that represents the error for this message
        /// </summary>
        /// <returns></returns>
        public IMessage MessageObject
        {
            get { return this.messageObject; }
        }
    }
}
