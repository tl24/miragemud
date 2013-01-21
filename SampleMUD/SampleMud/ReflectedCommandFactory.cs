using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;

namespace SampleMud
{
    public class ReflectedCommandFactory : IReflectedCommandFactory
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
            return cmd;
        }

        #endregion
    }
}
