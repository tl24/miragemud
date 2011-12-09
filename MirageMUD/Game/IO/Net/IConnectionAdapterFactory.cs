using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    public interface IConnectionAdapterFactory
    {
        IConnectionAdapter CreateConnectionAdapter(IConnection connection);
    }
}
