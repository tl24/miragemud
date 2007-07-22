using System;
using System.Text;
using System.Windows.Forms;
using MirageGUI.Forms;
using NGenerics.DataStructures;

namespace MirageGUI.Code
{
    public enum FormPriority
    {
        ConsoleFormPriority = 1,
        MasterFormPriority = 2,
        ConnectFormPriority = 3
    }
    public class MessageDispatcher : IDisposable
    {
        private SortedList<Handler> _handlers;
        private IOHandler _ioHandler;

        public MessageDispatcher(IOHandler ioHandler)
        {
            this._handlers = new SortedList<Handler>();
            this._ioHandler = ioHandler;
            _ioHandler.ResponseReceived += new ResponseHandler(HandleResponse);            
        }

        public ProcessStatus HandleResponse(Mirage.Communication.Message msg)
        {
            ProcessStatus result = ProcessStatus.NotProcessed;
            //Call each handler until one of them handles it
            foreach (Handler handler in _handlers)
            {
                IResponseHandler responseHandler = handler.handler;
                
                if (responseHandler is Form)
                    result = SendToForm((Form)responseHandler, msg);
                else
                    result = responseHandler.HandleResponse(msg);

                if (result == ProcessStatus.SuccessAbort)
                    break;
            }
            return result;
        }

        private ProcessStatus SendToForm(Form form, Mirage.Communication.Message msg)
        {
            if (form.InvokeRequired)
                return (ProcessStatus)form.Invoke(new ResponseHandler(((IResponseHandler)form).HandleResponse), msg);
            else
                return ((IResponseHandler)form).HandleResponse(msg);
        }

        /// <summary>
        /// Adds a handler to this instance that will receive messages.  The priority will affect
        /// the order that it receives messages comparable to other handlers.
        /// </summary>
        /// <param name="priority">its priority for receiving messages</param>
        /// <param name="responseHandler">the response handler</param>
        public void AddHandler(int priority, IResponseHandler responseHandler) {
            _handlers.Add(new Handler(priority, responseHandler));
        }

        public void AddHandler(FormPriority priority, IResponseHandler responseHandler)
        {
            AddHandler((int)priority, responseHandler);
        }

        public void RemoveHandler(IResponseHandler responseHandler)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                if (_handlers[i].handler == responseHandler)
                {
                    _handlers.RemoveAt(i);
                    break;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _ioHandler.ResponseReceived -= new ResponseHandler(HandleResponse);
            _handlers.Clear();
        }

        #endregion

        private class Handler : IComparable<Handler>
        {
            public int priority;
            public IResponseHandler handler;

            public Handler(int priority, IResponseHandler handler)
            {
                this.priority = priority;
                this.handler = handler;
            }

            #region IComparable<Handler> Members

            public int CompareTo(Handler other)
            {
                return other.priority - priority;
            }

            #endregion
        }
    }
}
