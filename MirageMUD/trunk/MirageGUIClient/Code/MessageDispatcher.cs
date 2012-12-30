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
        private Handler[] _handlers;
        private int _handlerCount;
        private bool _isDirty;

        private IOHandler _ioHandler;

        public MessageDispatcher(IOHandler ioHandler)
        {
            this._handlers = new Handler[10];

            this._ioHandler = ioHandler;
            _ioHandler.ResponseReceived += new ResponseHandler(HandleResponse);            
        }

        public ProcessStatus HandleResponse(Mirage.Game.Communication.Message msg)
        {
            ProcessStatus result = ProcessStatus.NotProcessed;
            ProcessUpdates();
            //Call each handler until one of them handles it
            //Copy the array reference in case a resize occurs during the loop
            Handler[] tmpHandlers = _handlers;
            int count = _handlerCount;
            for (int i = 0; i < count; i++)
            {
                if (tmpHandlers[i] != null)
                {
                    IResponseHandler responseHandler = tmpHandlers[i].handler;

                    if (responseHandler is Control)
                        result = SendToControl((Control)responseHandler, msg);
                    else
                        result = responseHandler.HandleResponse(msg);

                    if (result == ProcessStatus.SuccessAbort)
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Send a message to a control object.  Handles the logic for cross-thread calls.
        /// </summary>
        /// <param name="form">the form to receive the message</param>
        /// <param name="msg">the message</param>
        /// <returns>the processing status</returns>
        private ProcessStatus SendToControl(Control ctrl, Mirage.Game.Communication.Message msg)
        {
            if (ctrl.IsDisposed)
                return ProcessStatus.NotProcessed;
            try
            {
                if (ctrl.InvokeRequired)
                    return (ProcessStatus)ctrl.Invoke(new ResponseHandler(((IResponseHandler)ctrl).HandleResponse), msg);
                else
                    return ((IResponseHandler)ctrl).HandleResponse(msg);
            }
            catch (ObjectDisposedException ex)
            {
                // sometimes we can't catch this above, just ignore it
                return ProcessStatus.NotProcessed;
            }
        }

        /// <summary>
        /// Process any updates that occurred with the handler list
        /// </summary>
        private void ProcessUpdates()
        {
            if (_isDirty)
            {
                Array.Sort<Handler>(_handlers, new Comparison<Handler>(Handler.Compare));
            }
            _isDirty = false;
        }

        /// <summary>
        /// Adds a response handler to this dispatcher instance with the given 
        /// message handler priority.  The priority controls the order in which
        /// handlers recieve a message.
        /// </summary>
        /// <param name="priority">its priority for receiving messages</param>
        /// <param name="responseHandler">the response handler</param>
        public void AddHandler(int priority, IResponseHandler responseHandler) {
            _isDirty = true;
            for (int i = _handlerCount; i < _handlers.Length; i++)
            {
                if (_handlers[i] == null)
                {
                    _handlers[i] = new Handler(priority, responseHandler);
                    _handlerCount++;
                    _isDirty = true;
                    return;
                }
            }
            // not enough room in the array, make more room
            Handler[] tmpHandlers = new Handler[_handlers.Length * 2];
            Array.Copy(_handlers, tmpHandlers, _handlers.Length);
            _handlers = tmpHandlers;
        }

        /// <summary>
        /// Adds a response handler to this dispatcher instance with the given 
        /// message handler priority.  The priority controls the order in which
        /// handlers recieve a message.
        /// </summary>
        /// <param name="priority">handler priority</param>
        /// <param name="responseHandler">response handler</param>
        public void AddHandler(FormPriority priority, IResponseHandler responseHandler)
        {
            AddHandler((int)priority, responseHandler);
        }

        /// <summary>
        /// Removes a response handler from the dispatcher.  The dispatcher
        /// will no longer dispatch messages to this handler
        /// </summary>
        /// <param name="responseHandler">the handler to remove</param>
        public void RemoveHandler(IResponseHandler responseHandler)
        {
            for (int i = 0; i < _handlers.Length; i++)
            {
                if (_handlers[i] != null && _handlers[i].handler == responseHandler)
                {
                    _handlers[i] = null;
                    _handlerCount--;
                    _isDirty = true;
                    return;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _ioHandler.ResponseReceived -= new ResponseHandler(HandleResponse);
            _handlers = null;
        }

        #endregion

        private class Handler
        {
            public int priority;
            public IResponseHandler handler;

            public Handler(int priority, IResponseHandler handler)
            {
                this.priority = priority;
                this.handler = handler;
            }

            #region IComparable<Handler> Members

            public static int Compare(Handler a, Handler b)
            {
                int ap = a == null ? 0 : a.priority;
                int bp = b == null ? 0 : b.priority;

                return bp - ap;
            }

            #endregion
        }
    }
}
