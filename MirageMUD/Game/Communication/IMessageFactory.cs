using System;
namespace Mirage.Game.Communication
{
    public interface IMessageFactory
    {
        void Clear();
        IMessage GetMessage(string messageName);
        IMessage GetMessage(string messageName, string messageText);
        IMessage GetMessage(MessageName name, string messageText);
        IMessage GetMessage(MessageType type, string messageName, string messageText);
        IMessage GetMessage(MessageType type, MessageName name, string messageText);
    }
}
