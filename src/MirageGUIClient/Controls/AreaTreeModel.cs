using System;
using System.Collections.Generic;
using System.Text;
using MirageGUI.Code;
using MirageGUI.Forms;
using Mirage.Game.Communication;

namespace MirageGUI.Controls
{
    public class AreaTreeModel : ITreeModel, IResponseHandler
    {
        private IOHandler ioHandler;
        private IDictionary<string, string> _responseTypes;
        private MessageDispatcher dispatcher;
        private BuilderPane builder;
        private RootItem _root;
        public AreaTreeModel(IOHandler ioHandler, MessageDispatcher dispatcher, BuilderPane builder)
        {
            this.builder = builder;
            this.ioHandler = ioHandler;
            dispatcher.AddHandler(FormPriority.MasterFormPriority, this);
            _root = new RootItem(this);
        }

        public ProcessStatus HandleResponse(Mirage.Core.Messaging.Message response)
        {
            ProcessStatus result = ProcessStatus.NotProcessed;
            if (response.IsMatch(Namespaces.Area, "AreaList"))
            {
                result = _root.HandleResponse(response);
            }
            else if (response.IsMatch(Namespaces.Area, "AreaAdded"))
            {
                result = _root.HandleResponse(response);
            }
            else if (response.IsMatch(Namespaces.Area, "AreaUpdated"))
            {
                result = _root.HandleResponse(response);
            }
            else if (response is DataMessage)
                result = _root.HandleResponse(response);

            return result;
        }

        public System.Collections.IEnumerable GetChildren(TreePath path)
        {
            if (path.IsEmpty)
                return new object[] { _root };
            else
                return _root.GetChildren(path);
        }

        public int GetChildCount(TreePath path)
        {
            if (path.IsEmpty)
                return 1;
            else
                return _root.GetChildCount(path);
        }

        public event EventHandler<TreeEventArgs> StructureChanged;

        public void OnStructureChanged(TreePath path)
        {
            if (StructureChanged != null)
                StructureChanged(this, new TreeEventArgs(path));
        }

        public event EventHandler<TreeDataEventArgs> DataChanged;

        public void OnDataChanged(TreePath path, bool IsRecursive, object data)
        {
            if (DataChanged != null)
                DataChanged(this, new TreeDataEventArgs(path, IsRecursive, data));
        }

        public MirageGUI.Code.IOHandler IOHandler
        {
            get { return this.ioHandler; }
        }

        public MirageGUI.Code.MessageDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public MirageGUI.Forms.BuilderPane Builder
        {
            get { return this.builder; }
        }

    }
}
