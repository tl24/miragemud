using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World.Query
{

    public interface IUriContainer
    {
        object GetChild(string uri);
        QueryHints GetChildHints(string uri);
    }    
}
