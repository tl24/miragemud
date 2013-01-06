using System;
using Mirage.Game.World;
using Mirage.Core.Messaging;

namespace Mirage.Game.Communication
{

    /// <summary>
    /// Error message returned when a player invokes a command incorrectly or from
    /// an invalid state.  The exception will contain the suggested error message uri
    /// to get from the message factory.
    /// </summary>
    public class ValidationException : Exception
    {
        private IMessage _messageObject;
        private MessageDefinition _messageDefinition;

        /// <summary>
        /// Constructs the validation exception with the given <paramref name="messageDefinition"/>
        /// </summary>
        /// <param name="messageDefition">message definition</param>
        public ValidationException(MessageDefinition messageDefition)
            : this(messageDefition, null)
        {
        }

        /// <summary>
        /// Constructs the validation exception with the given message uri and inner exception
        /// </summary>
        /// <param name="messageUri">message uri</param>
        /// <param name="innerException">the exception that is the cause of the current exception, or null if there is no inner exception</param>
        public ValidationException(MessageDefinition messageDefition, Exception innerException)
            : base(null, innerException)
        {
            _messageDefinition = messageDefition;
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
            _messageObject = message;
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
                return _messageObject != null ? _messageObject.Render() : _messageDefinition.Text;
            }
        }
       

        /// <summary>
        /// Gets a message object that represents the error for this message
        /// </summary>
        /// <returns></returns>
        public IMessage CreateMessage(IActor actor)
        {
            return _messageObject ?? MessageFormatter.Instance.Format(actor, actor, _messageDefinition);
        }
    }
}
