using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Command
{
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
