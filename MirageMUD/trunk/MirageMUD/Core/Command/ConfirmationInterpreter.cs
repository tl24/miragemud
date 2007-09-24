using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;
using Mirage.Core.Data;

namespace Mirage.Core.Command
{
    public class ConfirmationInterpreter : IInterpret
    {
        private string _message = "Are you sure? (y\r\n) ";
        private string _cancellationMessage = "Command cancelled\r\n";
        private ICommand _method;
        private IInterpret priorInterpreter;
        private Player _actor;
        private string _invokedName;
        private object[] args;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string CancellationMessage
        {
            get { return _cancellationMessage; }
            set { _cancellationMessage = value; }
        }

        public ConfirmationInterpreter(Player player, ICommand method, string invokedName, object[] arguments)
        {
            SetActor(player);
            this._method = method;
            this._invokedName = invokedName;
            this.args = arguments;
        }

        private void SetActor(Player actor)
        {
            this._actor = actor;
            priorInterpreter = actor.Interpreter;
            this._actor.Interpreter = this;
        }

        #region IInterpret Members

        public bool Execute(Living actor, string input)
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
                actor.Write(new StringMessage(MessageType.Information, "Cancellation", _cancellationMessage));
                success = true;
            }
            else
            {
                success = false;
            }
            if (actor is Player)
            {
                ((Player)actor).Interpreter = priorInterpreter;
            }
            return success;
        }

        #endregion

        public void RequestConfirmation()
        {
            _actor.Write(new StringMessage(MessageType.Prompt, "ConfirmationPrompt", _message));
        }
    }
}
