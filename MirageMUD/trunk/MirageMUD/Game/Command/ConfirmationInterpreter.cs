using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Core.Messaging;
using Mirage.Core.Command;

namespace Mirage.Game.Command
{
    public class ConfirmationInterpreter : IInterpret
    {
        private ICommand _method;
        private IInterpret priorInterpreter;
        private IPlayer _actor;
        private string _invokedName;
        private object[] args;

        public IMessage Message { get; set; }

        public IMessage CancellationMessage { get; set; }

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
                Message = MessageFormatter.Instance.Format(_actor, _actor, CommonMessages.ConfirmationPrompt);
            if (CancellationMessage == null)
                CancellationMessage = MessageFormatter.Instance.Format(_actor, _actor, CommonMessages.ConfirmationCancel);
            _actor.Write(Message);
        }
    }
}
