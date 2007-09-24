using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Mirage.Core.Command
{
    /// <summary>
    /// Class for tokenizing input into arguments.  The parser
    /// splits on whitespace except when embedded within single or
    /// double quotes.
    /// </summary>
    /// <example>
    ///   <para>"arg1 arg2 arg2" => "arg1", "arg2", "arg3"</para>
    ///   <para>"arg1 'space in arg2' arg3" => "arg1", "space in arg2", "arg3"</para>
    /// </example>
    public class ArgumentParser : IEnumerable<string>
    {
        private string current;
        private bool isDone;
        private Regex parser;
        /// <summary>
        ///     Create an ArgumentParser to parse the given input
        /// </summary>
        /// <param name="input">the input containing arguments to parse</param>
        public ArgumentParser(string input)
        {
            this.current = input;
            isDone = false;
            parser = new Regex(
                @"^                       # match at begginning
                   \s*                    # ignore leading whitespace
                   (                      # start alternation
                     '(?<value>[^']*)'    # single quoted (inside quotes only)
                     |                    # or
                     ""(?<value>[^""]*)"" # double quoted value
                     |                    # or
                     (?<value>\S+)        # non-whitespace
                   )                      # end alternation
                 ", RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Parses the next argument from the input string
        /// </summary>
        /// <returns>the next argument</returns>
        public string GetNextArgument()
        {

            string value = null;
            if (current == string.Empty || current == null)
            {
                isDone = true;
                current = null;
            }

            if (isDone)
            {
                return null;
            }
            else
            {

                Match matcher = parser.Match(current);
                if (matcher.Success)
                {
                    value = matcher.Groups["value"].Value;
                    current = matcher.Result("$'");
                } else {
                    value = current;
                    current = null;
                    isDone = true;
                }
            }

            return value;
        }

        /// <summary>
        ///     Returns true if there are no remaining arguments
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsEmpty()
        {
            return isDone || current == null || current.Trim().Length == 0;
        }

        /// <summary>
        /// Get the rest of the input, disregarding any spaces or quotes
        /// </summary>
        /// <returns>the remaining input</returns>
        public string GetRest()
        {
            string value = null;
            if (!isDone)
            {
                value = current.TrimStart();
            }
            isDone = true;
            current = null;
            return value;
        }

        public IEnumerator<string> GetEnumerator()
        {
            ArgumentParser newParser = new ArgumentParser(current);
            while (!newParser.IsEmpty())
            {
                yield return newParser.GetNextArgument();
            }
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
