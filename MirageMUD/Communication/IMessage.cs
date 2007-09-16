using System;
namespace Mirage.Communication
{
    public interface IMessage
    {
        bool IsMatch(Uri baseNamespace);
        bool IsMatch(Uri baseNamespace, string name);
        bool IsMatch(MessageType type, Uri baseNamespace, string name);
        bool IsMatch(MessageType type);
        MessageType MessageType { get; set; }
        string Name { get; set; }
        Uri Namespace { get; set; }
        string QualifiedName { get; }

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
        string RenderMessage();
    }
}
