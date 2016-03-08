using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Messaging;

namespace Mirage.Core.Command.ArgumentConversion
{
    /// <summary>
    /// Provides a context during argument conversion for ReflectedCommand.
    /// This will contain the input arguments and keep track of the current index
    /// in the argument list that is being converted
    /// </summary>
    public class ArgumentConversionContext
    {
        private object[] _arguments;

        /// <summary>
        /// Creates a context for an argument conversion with the specified input parameters
        /// </summary>
        /// <param name="invokedName">the command name used to invoke the command</param>
        /// <param name="actor">the actor invoking the command</param>
        /// <param name="arguments">the input arguments to be converted</param>
        public ArgumentConversionContext(string invokedName, IActor actor, object[] arguments)
        {
            _arguments = arguments;
            InvokedName = invokedName;
            Actor = actor;
        }

        /// <summary>
        /// Gets the argument with the specified index from the arguments collection
        /// </summary>
        /// <param name="index">the index to retrieve</param>
        /// <returns>argument</returns>
        public object this[int index]
        {
            get { return _arguments[index]; }
        }

        /// <summary>
        /// Gets the argument count
        /// </summary>
        public int Count
        {
            get { return _arguments.Length; }
        }

        /// <summary>
        /// Gets the argument at CurrentIndex property
        /// </summary>
        public object Current
        {
            get { return this[CurrentIndex]; }
        }

        /// <summary>
        /// Returns the current argument and increments the CurrentIndex property
        /// </summary>
        /// <returns>the current object</returns>
        public object GetCurrentAndIncrement()
        {
            return this[CurrentIndex++];
        }

        /// <summary>
        /// Gets or sets the index currently being looked at in the argument list
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Gets or sets the error message for this context that
        /// will be returned when a conversion error occurs
        /// </summary>
        public IMessage ErrorMessage { get; set; }

        /// <summary>
        /// Gets the actor invoking the command
        /// </summary>
        public IActor Actor { get; private set; }

        /// <summary>
        /// Gets the name or alias used to invoke the command
        /// </summary>
        public string InvokedName { get; private set; }

    }

}
