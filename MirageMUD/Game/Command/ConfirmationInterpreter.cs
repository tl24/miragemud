using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;
using Mirage.Game.World;

namespace Mirage.Game.Command
{
    public class ConfirmationInterpreter : IInterpret
    {
        private IMessage _message;
        private IMessage _cancellationMessage;
        private ICommand _method;
        private IInterpret priorInterpreter;
        private IPlayer _actor;
        private string _invokedName;
        private object[] args;

        public IMessage Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public IMessage CancellationMessage
        {
            get { return _cancellationMessage; }
            set { _cancellationMessage = value; }
        }

        public ConfirmationInterpreter(IPlayer player, ICommand method, string invokedName, object[] arguments)
        {
            SetActor(player);
            this._method = method;
            this._invokedName = invokedName;
            this.args = arguments;
        }

        private void SetActor(IPlayer actor)
        {
            this._actor = actor;
            priorInterpreter = actor.Interpreter;
            this._actor.Interpreter = this;
        }

        #region IInterpret Members

        public bool Execute(IActor actor, string input)
        {
            bool success = false;
            input = input.ToLower();
            if (input.Equals("yes") || input.Equals("y"))
            {
                object st = _method.Invoke(_invokedName, actor, args);
                if (st != null)
                {
                    if (st is IMessage)
                    {
                        actor.Write((IMessage)st);
                    }
                    else
                    {
                        actor.Write(new StringMessage(MessageType.Information, "MethodResult." + _method.Name, st.ToString()));
                    }
                }
                success = true;
            }
            else if (input.Equals("no") || input.Equals("n"))
            {
                actor.Write(CancellationMessage);
                success = true;
            }
            else
            {
                success = false;
            }
            if (actor is IPlayer)
            {
                ((IPlayer)actor).Interpreter = priorInterpreter;
            }
            return success;
        }

        #endregion

        public void RequestConfirmation()
        {
            if (Message == null)
                Message = MudFactory.GetObject<IMessageFactory>().GetMessage("system.ConfirmationPrompt");
            if (CancellationMessage == null)
                CancellationMessage = MudFactory.GetObject<IMessageFactory>().GetMessage("system.ConfirmationCancel");
            _actor.Write(Message);
        }
    }
}
