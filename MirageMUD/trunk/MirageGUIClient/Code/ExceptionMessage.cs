using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;

namespace MirageGUI.Code
{
    /// <summary>
    /// Message type for unhandled exceptions that occur in the GUI,
    /// mainly in the IOHandler receive code.  A message is used so that it
    /// can be processed as an response.
    /// </summary>
    public class ExceptionMessage : StringMessage
    {
        private object data;
        private Exception inner;
        public ExceptionMessage(string name, Exception inner, object data)
            : base(MessageType.SystemError, new MessageName("common.error.GuiError", name), inner.Message)
        {
            this.data = data;
            this.inner = inner;
        }

        public object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public Exception Inner
        {
            get { return this.inner; }
            set { this.inner = value; }
        }


    }
}
