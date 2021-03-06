using Mirage.Core.Extensibility;
using Mirage.Game.Communication;
using Mirage.Core.Messaging;
using Mirage.Core.Command;

namespace Mirage.Game.World.Attribute
{
    public class OpenableAttribute : BaseAttribute, IOpenable
    {
        protected bool _isOpen;

        public class Messages
        {
            public static readonly MessageDefinition ObjectAlreadyOpen = new MessageDefinition("common.open.error.alreadyopen", "It's already open.");
            public static readonly MessageDefinition ObjectAlreadyClosed = new MessageDefinition("common.close.error.alreadyclosed", "It's already closed.");
            public static readonly MessageDefinition NotOpenable = new MessageDefinition("common.open.error.notopenable", "You can't open that.");
            public static readonly MessageDefinition NotCloseable = new MessageDefinition("common.close.error.notcloseable", "You can't close that.");
        }

        public OpenableAttribute(IAttributable target)
            : base(target)
        {
        }

        public OpenableAttribute()
            : base()
        {
        }

        public OpenableAttribute(IAttributable target, bool isOpen)
            : base(target)
        {
            _isOpen = isOpen;
        }

        protected static IMessage ToMessage(MessageDefinition msgDef)
        {
            return new StringMessage(msgDef.MessageType, msgDef.Name, msgDef.Text);
        }
        #region IOpenable Members

        public bool Opened
        {
            get { return _isOpen; }
        }

        public virtual void Open()
        {
            if (Opened)
            {
                throw new ValidationException(ToMessage(Messages.ObjectAlreadyOpen));
            }
            else
            {
                _isOpen = true;
            }
        }

        public virtual void Close()
        {
            if (!Opened)
            {
                throw new ValidationException(ToMessage(Messages.ObjectAlreadyClosed));
            }
            else
            {
                _isOpen = false;
            }
        }

        #endregion

        /// <summary>
        /// Checks to see if the object is open, if the the object does not
        /// support IOpenable interace, then true is returned, otherwise the
        /// IsOpen property is checked.
        /// </summary>
        /// <param name="target">the object to check</param>
        /// <returns>true if open or not supported, false otherwise</returns>
        public static bool IsOpen(object target)
        {
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            return o == null ? true : o.Opened;
        }

        /// <summary>
        /// Checks to see if the given object implements IOpenable and attempts to open it.
        /// If it doesn't implement openable, an exception is thrown.
        /// </summary>
        /// <param name="target">the object to open</param>
        /// <exception cref="ValidationException">if the object does not support IOpenable or the object can't be opened</exception>
        public static void Open(object target)
        {
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            if (o != null)
                o.Open();
            else
                throw new ValidationException(ToMessage(Messages.NotOpenable));
        }

        /// <summary>
        /// Checks to see if the given object implements IOpenable and attempts to close it.
        /// If it doesn't implement IOpenable, an exception is thrown.
        /// </summary>
        /// <param name="target">the object to close</param>
        /// <exception cref="ValidationException">if the object does not support IOpenable or the object can't be closed</exception>
        public static void Close(object target)
        {
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            if (o != null)
                o.Close();
            else
                throw new ValidationException(ToMessage(Messages.NotCloseable));
        }
    }
}
