using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mirage.Game.World;
using Mirage.Game.Communication;

namespace Mirage.Game.Command
{
    public class Argument
    {
        private ParameterInfo _parameter;
        private ConvertArgumentHandler _handler;

        public Argument(ParameterInfo parameter)
        {
            _parameter = parameter;
        }

        /// <summary>
        /// The System.Reflection.ParameterInfo behind this argument
        /// </summary>
        public ParameterInfo Parameter
        {
            get { return this._parameter; }
        }

        /// <summary>
        /// The handler that converts this argument
        /// </summary>
        public ConvertArgumentHandler Handler
        {
            get { return this._handler; }
            set { this._handler = value; }
        }

        public object Convert(ArgumentConversionContext context)
        {
            return Handler(this, context);
        }
    }

    /// <summary>
    /// A list of arguments to be converted for a method
    /// </summary>
    public class ArgumentList : List<Argument>
    {
        public ArgumentList(ParameterInfo[] parameters)
        {
            foreach (ParameterInfo parm in parameters)
            {
                this.Add(new Argument(parm));
            }
        }
    }

    /// <summary>
    /// Provides a context during argument conversion for ReflectedCommand.
    /// This will contain the input arguments and keep track of the current index
    /// in the argument list that is being converted
    /// </summary>
    public class ArgumentConversionContext
    {
        private int _currentIndex = 0;
        private object[] _arguments;
        private IActor _actor;
        private IMessage _errorMessage;
        private string _invokedName;

        /// <summary>
        /// Creates a context for an argument conversion with the specified input parameters
        /// </summary>
        /// <param name="invokedName">the command name used to invoke the command</param>
        /// <param name="actor">the actor invoking the command</param>
        /// <param name="arguments">the input arguments to be converted</param>
        public ArgumentConversionContext(string invokedName, IActor actor, object[] arguments)
        {
            _invokedName = invokedName;
            _arguments = arguments;
            _actor = actor;
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
            get { return this[_currentIndex]; }
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
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        /// <summary>
        /// Gets or sets the error message for this context that
        /// will be returned when a conversion error occurs
        /// </summary>
        public IMessage ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        /// <summary>
        /// Gets the actor invoking the command
        /// </summary>
        public IActor Actor
        {
            get { return this._actor; }
        }

        /// <summary>
        /// Gets the name or alias used to invoke the command
        /// </summary>
        public string InvokedName
        {
            get { return this._invokedName; }
        }


    }
}
