using System;

namespace Mirage.Core.Command
{
    /// <summary>
    ///     Attribute applied to a method to identify it as a Command that is callable
    ///     by the user.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : System.Attribute {

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
