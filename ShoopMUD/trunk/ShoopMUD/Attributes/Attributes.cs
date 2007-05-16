using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Attributes
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

        public string[] Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        public string[] Aliases
        {
            get { return _aliases; }
            set { _aliases = value; }
        }
    }

    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class ArgumentTypeAttribute : System.Attribute
    {
        private ArgumentType type;

        public ArgumentType ArgType
        {
            get { return type; }
        }
        private ScopeType scope;

        public ScopeType Scope
        {
            get { return scope; }
            set { scope = value; }
        }

        public ArgumentTypeAttribute(ArgumentType type)
        {
            this.type = type;
            this.scope = ScopeType.Room;
        }


    }

    public enum ArgumentType
    {
        /// <summary>
        ///     Refers to the invoking player
        /// </summary>
        Self,

        /// <summary>
        ///     The argument refers to player invoking the Command
        /// </summary>
        Player,

        /// <summary>
        ///     Use the input argument to lookup an object by name
        /// </summary>
        Object,

        /// <summary>
        ///     Use the input argument to lookup either a Mob or Player by name
        /// </summary>
        Animate,

        /// <summary>
        ///     Use the input argument to lookup a Mob by name
        /// </summary>
        Mobile,

        /// <summary>
        ///     The argument uses the rest of the input arguments as one parameter
        /// </summary>
        ToEOL,

        /// <summary>
        ///     Holder for the actual command name used to invoke the command by the player
        /// </summary>
        CommandName
    }

    public enum ScopeType
    {

        /// <summary>
        ///     Look for an item within the current players inventory
        /// </summary>
        Inventory,

        /// <summary>
        ///     Look for an item or player with the current room
        /// </summary>
        Room,

        /// <summary>
        ///     Look for an item or player anywhere
        /// </summary>
        Global
    }

    /// <summary>
    ///     Attribute applied to a method to identify it as a Command that is callable
    ///     by the user.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ConfirmationAttribute : System.Attribute
    {
        private string _message;
        private string _cancellationMessage;

        /// <summary>
        ///     Creates an instance of the attribute
        /// </summary>
        public ConfirmationAttribute()
        {
        }

        /// <summary>
        ///     Message to show to the user to ask for confirmation
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        ///     A helpful description of the Command
        /// </summary>
        public string CancellationMessage
        {
            get { return _cancellationMessage; }
            set { _cancellationMessage = value; }
        }
    }

}
