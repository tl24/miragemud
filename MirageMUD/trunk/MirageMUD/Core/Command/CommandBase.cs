using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;
using Mirage.Core.Data;
using System.Security.Principal;

namespace Mirage.Core.Command
{
    /// <summary>
    /// Abstract base class implementation of the ICommand interface
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        #region Member Variables

        protected string _name;
        protected int _argCount;
        protected string[] _aliases;
        protected string[] _roles;
        protected Type[] _clientTypes;

        protected int _priority = 50;
        protected bool _customParse;
        protected int _level;

        #endregion Member Variables

        #region ICommand Members

        /// <summary>
        /// The name of the method
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Aliases for the method name.  If specified, Name property is not used
        /// </summary>
        public virtual string[] Aliases
        {
            get { return _aliases; }
        }

        /// <summary>
        /// Necessary roles to invoke the command
        /// </summary>
        public virtual string[] Roles
        {
            get { return _roles; }
        }

        /// <summary>
        /// The required player level to Execute the Command
        /// </summary>
        public virtual int Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Priority is used to sort the commands when multiple commands match for a given command string.
        /// The highest priority command will be tried first.
        /// </summary>
        public virtual int Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// Number of arguments to the Command, not including private arguments such as "self"
        /// </summary>
        public virtual int ArgCount
        {
            get { return _argCount; }
        }

        /// <summary>
        /// Types of available clients
        /// </summary>
        public virtual Type[] ClientTypes
        {
            get { return _clientTypes; }
        }

        /// <summary>
        /// True if the Command has a ToEOL argument, which means it has a variable number of arguments
        /// </summary>
        public virtual bool CustomParse
        {
            get { return _customParse; }
        }

        /// <summary>
        /// Checks the actor's security, level and client type to see if they
        /// can access the command
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public virtual bool CanInvoke(IActor actor)
        {
            if (Level > actor.Level)
                return false;
            
            if (ClientTypes != null && ClientTypes.Length > 0) {
                IPlayer player = actor as IPlayer;
                if (player == null || player.Client == null)
                    return false;

                Type clientType = player.Client.GetType();
                bool found = false;
                foreach(Type t in ClientTypes) {
                    if (t.IsAssignableFrom(clientType)) {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }

            if (Roles.Length > 0) {
                IPrincipal principal = actor.Principal;
                bool found = false;
                foreach (string role in Roles) {
                    if (principal.IsInRole(role)) {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            // if we make it here we're ok
            return true;
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
