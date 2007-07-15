using System;
using System.Collections.Generic;
using System.Text;

namespace MirageGUIClient
{
    public delegate void ResponseHandler(MudResponse response);

    public interface IResponseHandler
    {
        void HandleResponse(MudResponse response);
    }
}
