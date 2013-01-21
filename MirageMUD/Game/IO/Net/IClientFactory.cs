using Mirage.Core.IO.Net;

namespace Mirage.Game.IO.Net
{
    public interface IClientFactory
    {
        IClient<ClientPlayerState> CreateConnectionAdapter(IConnection connection);
    }
}
