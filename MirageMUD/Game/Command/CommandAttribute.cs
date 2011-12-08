using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Command
{

    /// <summary>
    ///     Attribute applied to a class containing commands to set default values for all commands in the class
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CommandDefaultsAttribute : System.Attribute
    {
        protected int _level;
        protected string _roles;
        protected Type[] _clients;
        
        /// <summary>
        ///     Creates an instance of the attribute
        /// </summary>
        public CommandDefaultsAttribute()
        {
            _level = -1;
        }

        /// <summary>
        ///     The required level to use the Command
        /// </summary>
        public virtual int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        /// A comma-separated list of roles required to call this command.  A player must
        /// have at least one role in the list to call the command.
        /// </summary>
        public virtual string Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        /// <summary>
        /// Client types that are allowed to call this command.  If null or 0-length then
        /// all clients may call the command
        /// </summary>
        public virtual Type[] ClientTypes
        {
            get { return this._clients; }
            set { this._clients = value; }
        }
    }

    /// <summary>
    ///     Attribute applied to a method to identify it as a Command that is callable
    ///     by the user.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : CommandDefaultsAttribute {

        private string _description;
        private string[] _aliases;
        private int _priority = 0;

        /// <summary>
        ///     A helpful description of the Command
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Aliases that may be used to call the command.  These override the method name
        /// </summary>
        public string[] Aliases
        {
            get { return _aliases; }
            set { _aliases = value; }
        }

        public int Priority
        {
            get
            {
                return this._priority;
            }
            set
            {
                this._priority = value;
            }
        }


    }
}
