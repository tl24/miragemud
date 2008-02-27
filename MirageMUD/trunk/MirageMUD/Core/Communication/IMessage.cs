using System;
using System.Collections.Generic;
namespace Mirage.Core.Communication
{
    public interface IMessage
    {
        /// <summary>
        /// Checks to see if the message matches on namespace or qualified name
        /// </summary>
        /// <param name="baseNamespace">namespace or qualified name to check</param>
        /// <returns>true if it matches</returns>
        bool IsMatch(string baseNamespace);

        /// <summary>
        /// Checks to see if the message matches the specified namespace and name
        /// </summary>
        /// <param name="baseNamespace">the base namespace to check</param>
        /// <param name="name">the message name</param>
        /// <returns>true if it matches</returns>
        bool IsMatch(string baseNamespace, string name);

        /// <summary>
        /// Checks to see if the message matches the specified namespace and name
        /// </summary>
        /// <param name="baseNamespace">the base namespace to check</param>
        /// <param name="name">the message name</param>
        /// <returns>true if it matches</returns>
        bool IsMatch(MessageName name);

        /// <summary>
        /// Checks to see if the message matches the specified type
        /// </summary>
        /// <param name="type">the message type to check</param>
        /// <returns>true if it matches</returns>
        bool IsMatch(MessageType type);

        /// <summary>
        /// The general type of the message such as error or confirmation
        /// </summary>
        MessageType MessageType { get; set; }

        /// <summary>
        /// The name of the message which does not include the namespace
        /// </summary>
        MessageName Name { get; set; }

        /// <summary>
        /// Checks to see if this message can be transferred from server to
        /// client.  If not it will return false and GetTransferMessageShould be called
        /// to get a message that can be sent to the client.
        /// </summary>
        bool CanTransferMessage { get; }

        /// <summary>
        /// Gets a message that can be sent to the client.  Messages that contain references
        /// to server-side components cannot be transferred to a client.
        /// </summary>
        /// <returns>a new transferrable message</returns>
        IMessage GetTransferMessage();

        /// <summary>
        /// Converts the message to a displayable string
        /// </summary>
        /// <returns>display message string</returns>
        string Render();

        /// <summary>
        /// Copies the current object or returns the current object if the object has
        /// no mutable properties.  The message factory uses this method to create new
        /// copies of messages.
        /// </summary>
        /// <returns></returns>
        IMessage Copy();

        /// <summary>
        /// The replacement parameters for the template
        /// </summary>
        IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Gets and sets parameters for the template
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string key] { get; set; }
    }
}
