using Mirage.Core.IO.Net;

namespace Mirage.Game.IO.Net
{
    public interface IClientFactory
    {
        IClient CreateConnectionAdapter(IConnection connection);
    }
}
