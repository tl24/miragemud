using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data;
using Shoop.Communication;

namespace Shoop.Command
{
    public interface ICommand
    {
        /// <summary>
        /// A name for the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A list of aliases for this command, if this is specified, Name is ignored
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// The roles required to invoke this command.  A player having any one of the
        /// roles listed can invoke the command
        /// </summary>
        string[] Roles { get; }

        /// <summary>
        /// The minimum level required to invoke this command
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Priority is used to determine the command to invoke when there are multiple matches
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// The number of arguments to this command
        /// </summary>
        int ArgCount { get; }

        /// <summary>
        /// True if this Command does custom parsing
        /// </summary>
        bool CustomParse { get; }

        /// <summary>
        /// Validates the argument array against the expected types.  If validation fails,
        /// false should be returned and the errorMessage property will be populated with an error message.
        /// </summary>
        /// <param name="invokedName">the command name or alias that was used to invoke this command</param>
        /// <param name="self">the player invoking the command</param>
        /// <param name="arguments">the arguments to the command</param>
        /// <param name="context">A custom context object that will be passed to the Invoke Method</param>
        /// <param name="errorMessage">an error message if validation fails</param>
        /// <returns>true if types are correct</returns>
        bool ValidateTypes(string invokedName, Player self, string[] arguments, out object context, out Message errorMessage);

        /// <summary>
        /// Invoke the given command
        /// </summary>
        /// <param name="invokedName">the command name or alias that was used to invoke this command</param>
        /// <param name="self">the player invoking the command</param>
        /// <param name="arguments">the arguments to the command</param>
        /// <returns>A message to be returned to the player if any</returns>
        Message Invoke(string invokedName, Player self, string[] arguments, object context);
    }
}
