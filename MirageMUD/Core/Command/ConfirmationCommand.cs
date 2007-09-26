using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;

namespace Mirage.Core.Command
{
    class ConfirmationCommand : ICommand
    {
        private ICommand _innerCommand;
        private string _promptMessage;
        private string _cancellationMessage;

        public ConfirmationCommand(ICommand innerCommand, string prompt, string cancellationMessage)
        {
            this._innerCommand = innerCommand;
            this._promptMessage = prompt;
            this._cancellationMessage = cancellationMessage;
        }
        #region ICommand Members

        public string Name
        {
            get { return _innerCommand.Name; }
        }

        public string[] Aliases
        {
            get { return _innerCommand.Aliases; }
        }

        public string[] Roles
        {
            get { return _innerCommand.Roles; }
        }

        public int Level
        {
            get { return _innerCommand.Level; }
        }

        public int Priority
        {
            get { return _innerCommand.Priority; }
        }

        public int ArgCount
        {
            get { return _innerCommand.ArgCount; }
        }

        public bool CustomParse
        {
            get { return _innerCommand.CustomParse; }
        }

        public bool ConvertArguments(string invokedName, IActor actor, object[] arguments, out object[] convertedArguments, out IMessage errorMessage)
        {
            return _innerCommand.ConvertArguments(invokedName, actor, arguments, out convertedArguments, out errorMessage);
        }

        public IMessage Invoke(string invokedName, IActor actor, object[] arguments)
        {
            if (actor is IPlayer)
            {
                // create the interpreter
                ConfirmationInterpreter interp = new ConfirmationInterpreter((IPlayer) actor, _innerCommand, invokedName, arguments);
                if (_promptMessage != null)
                    interp.Message = _promptMessage;
                if (_cancellationMessage != null)
                    interp.CancellationMessage = _promptMessage;

                interp.RequestConfirmation();
            }
            else
            {
                // mobile, just execute the command without confirmation
                _innerCommand.Invoke(invokedName, actor, arguments);
            }
            return null;
        }

        #endregion

        #region ICommand Members


        public Type[] ClientTypes
        {
            get { return _innerCommand.ClientTypes; }
        }

        #endregion
    }
}
