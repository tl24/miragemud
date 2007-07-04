using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Command
{
    /// <summary>
    ///     Attribute applied to a method to identify it as a Command that is callable
    ///     by the user.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : System.Attribute
    {
        private int _level;
        private string _description;
        private string[] _roles;
        private string[] _aliases;

        /// <summary>
        ///     Creates an instance of the attribute
        /// </summary>
        public CommandAttribute()
        {
            _level = 1;
        }

        /// <summary>
        ///     The required level to use the Command
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        ///     A helpful description of the Command
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// A list of roles required to call this command.  A player must
        /// have at least one role in the list to call the command.
        /// </summary>
        public string[] Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        /// <summary>
        /// Aliases that may be used to call the command.  These override the method name
        /// </summary>
        public string[] Aliases
        {
            get { return _aliases; }
            set { _aliases = value; }
        }
    }
}
