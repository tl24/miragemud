using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.Command.Infrastructure
{
    public class ReflectedCommandGroupFactory : IReflectedCommandFactory
    {
        public IReflectedCommandGroup GetCommandGroup(Type groupType)
        {
            return new ReflectedCommandGroup(groupType);
        }

        #region IReflectedCommandFactory Members

        /// <summary>
        /// Factory method for creating an instance of ReflectedCommand
        /// which is optionally wrapped by ConfirmationCommand if it needs
        /// confirmation.
        /// </summary>
        /// <param name="methInfo">Reflection MethodInfo for the command method</param>
        /// <returns>command</returns>
        public ICommand CreateCommand(System.Reflection.MethodInfo method, IReflectedCommandGroup commandGroup)
        {
            ICommand cmd = new ReflectedCommand(method, commandGroup);
            if (method.IsDefined(typeof(ConfirmationAttribute), false))
            {
                ConfirmationAttribute confAttr = (ConfirmationAttribute)method.GetCustomAttributes(typeof(ConfirmationAttribute), false)[0];
                ConfirmationCommand confCmd = new ConfirmationCommand(cmd, confAttr.Message, confAttr.CancellationMessage);
                cmd = confCmd;
            }
            return cmd;
        }

        #endregion
    }
}
