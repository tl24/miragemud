using System;
using System.Security.Principal;
using Mirage.Core.Messaging;
using System.Collections.Generic;
using Mirage.Core.Command.Guards;
using System.Linq;

namespace Mirage.Core.Command
{
    /// <summary>
    /// Abstract base class implementation of the ICommand interface
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            Priority = 50;
            Guards = new List<ICommandGuard>();
        }

        public List<ICommandGuard> Guards { get; private set; }

        #region ICommand Members

        /// <summary>
        /// The name of the method
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Aliases for the method name.  If specified, Name property is not used
        /// </summary>
        public virtual string[] Aliases { get; protected set; }

        /// <summary>
        /// The required player level to Execute the Command
        /// </summary>
        public virtual int Level { get; protected set; }

        /// <summary>
        /// Priority is used to sort the commands when multiple commands match for a given command string.
        /// The highest priority command will be tried first.
        /// </summary>
        public virtual int Priority { get; protected set; }

        /// <summary>
        /// Number of arguments to the Command, not including private arguments such as "self"
        /// </summary>
        public virtual int ArgCount { get; protected set; }

        /// <summary>
        /// True if the Command has a ToEOL argument, which means it has a variable number of arguments
        /// </summary>
        public virtual bool CustomParse { get; protected set; }

        /// <summary>
        /// Checks the actor's security, level and client type to see if they
        /// can access the command
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public virtual bool CanInvoke(IActor actor)
        {
            return Guards.All(g => g.IsSatisified(actor));
        }

        public virtual bool ConvertArguments(string invokedName, IActor actor, object[] arguments, out object[] convertedArguments, out IMessage errorMessage)
        {
            convertedArguments = arguments;
            errorMessage = null;
            return true;
        }

        public abstract IMessage Invoke(string invokedName, IActor actor, object[] arguments);

        public abstract string UsageHelp();

        public abstract string ShortHelp();
        #endregion
    }
}
