using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Command
{
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

}
