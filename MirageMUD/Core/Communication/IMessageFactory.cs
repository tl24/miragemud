using System;
namespace Mirage.Core.Communication
{
    public interface IMessageFactory
    {
        void Clear();
        IMessage GetMessage(string messageUri);
    }
}
