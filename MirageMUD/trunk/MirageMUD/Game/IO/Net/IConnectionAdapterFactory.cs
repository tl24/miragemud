using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    public interface IConnectionAdapterFactory
    {
        IConnectionAdapter CreateConnectionAdapter(IConnection connection);
    }
}
