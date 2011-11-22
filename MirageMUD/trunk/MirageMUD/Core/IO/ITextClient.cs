using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.IO
{
    public interface ITextClient : ITelnetClient
    {
        TextClientOptions Options { get; }
    }
}
