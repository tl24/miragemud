using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data;
using Shoop.Communication;

namespace Shoop.Command
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

        public bool ValidateTypes(string invokedName, Player self, string[] arguments, out object context, out Shoop.Communication.Message errorMessage)
        {
            return _innerCommand.ValidateTypes(invokedName, self, arguments, out context, out errorMessage);
        }

        public Message Invoke(string invokedName, Shoop.Data.Player self, string[] arguments, object context)
        {
            // create the interpreter
            ConfirmationInterpreter interp = new ConfirmationInterpreter(self, _innerCommand, invokedName, arguments, context);
            if (_promptMessage != null)
                interp.Message = _promptMessage;
            if (_cancellationMessage != null)
                interp.CancellationMessage = _promptMessage;

            interp.requestConfirmation();
            return null;
        }

        #endregion
    }
}
