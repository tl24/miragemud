using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mirage.Command
{
    /// <summary>
    /// Class for tokenizing input into arguments.  The parser
    /// splits on whitespace except when embedded within single or
    /// double quotes.
    /// </summary>
    public class ArgumentParser
    {
        private string current;
        private Regex parser;
        private bool isDone;

        /// <summary>
        ///     Create an ArgumentParser to parse the given input
        /// </summary>
        /// <param name="input">the input containing arguments to parse</param>
        public ArgumentParser(string input)
        {
            this.current = input;
            //this.parser = new Regex("('[^']*')|(\"[^\"]*\")|\\w+");
            this.parser = new Regex(@" |\b");
            isDone = false;
        }

        /// <summary>
        /// Gets the next argument in the list
        /// </summary>
        /// <returns>the next argument</returns>
        public string getNextArgument()
        {

            string value;
            if (current == string.Empty)
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
                string[] results = parser.Split(current, 3);
                value = results[1];
                if (results.Length > 2)
                {
                    current = results[2];
                }
                else
                {
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
        public bool isEmpty()
        {
            return isDone || current == null || current.Trim().Length == 0;
        }

        /// <summary>
        /// Get the rest of the input, disregarding any spaces or quotes
        /// </summary>
        /// <returns>the remaining input</returns>
        public string getRest()
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
    }
}
