using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MirageGUIClient
{
    public class MessageDispatcher
    {
        private BuilderPane _masterForm;
        private ConsoleForm _console;

        public MessageDispatcher(BuilderPane masterForm)
        {
            this._masterForm = masterForm;
            this._console = masterForm.Console;
            masterForm.IOHandler.ResponseReceived +=new ResponseHandler(HandleResponse);
        }

        public void HandleResponse(MudResponse response)
        {
            if (response.Name.StartsWith("Area."))
            {
                SendToForm(_masterForm, response);
            }
            else
            {
                SendToForm(_console, response);
            }
        }

        private void SendToForm(Form form, MudResponse response)
        {
            if (form.InvokeRequired)
                form.Invoke(new ResponseHandler(((IResponseHandler)form).HandleResponse), response);
            else
                ((IResponseHandler)form).HandleResponse(response);
        }
    }
}
