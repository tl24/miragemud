using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mirage.Core.Command.ArgumentConversion
{
    public class Argument
    {
        public Argument(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        /// The System.Reflection.ParameterInfo behind this argument
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        /// <summary>
        /// The handler that converts this argument
        /// </summary>
        public Func<Argument, ArgumentConversionContext, object> Handler { get; set; }

        public object Convert(ArgumentConversionContext context)
        {
            return Handler(this, context);
        }
    }

}
