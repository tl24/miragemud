using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    public interface IUriContainer
    {
        object GetChild(string uri);
    }
}
