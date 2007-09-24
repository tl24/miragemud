using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Command
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=true)]
    public class ConstAttribute : System.Attribute
    {
        private string _constant;

        /// <summary>
        /// Indicates an argument that will always be a constant string
        /// </summary>
        /// <param name="constant">constant text</param>
        public ConstAttribute(string constant)
        {
            _constant = constant;
        }

        /// <summary>
        /// The constant text
        /// </summary>
        public string Constant
        {
            get { return _constant; }
        }
    }
}
