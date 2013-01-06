using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    public interface IClientFactory
    {
        IClient CreateConnectionAdapter(IConnection connection);
    }
}
