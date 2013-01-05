using System;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Core.Messaging;

namespace Mirage.Game.Command
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
        /// The types of mud clients that are allowed to Execute this command.  Generally this should be
        /// left blank which includes all, unless only certain clients have the capability to provide inputs
        /// to this command or are incapable of recieving the output from it.
        /// </summary>
        Type[] ClientTypes { get; }

        /// <summary>
        /// True if this Command does custom parsing
        /// </summary>
        bool CustomParse { get; }

        /// <summary>
        /// Verifies that the actor meets the necessary requirements to invoke this command
        /// </summary>
        /// <param name="actor">the actor to check</param>
        /// <returns>true if the actor is allowed to invoked the command</returns>
        bool CanInvoke(IActor actor);

        /// <summary>
        /// Converts arguments from their input format into their desired types
        /// </summary>
        /// <param name="invokedName">the command name or alias that was used to invoke this command</param>
        /// <param name="actor">the entity invoking the command (Player or Mobile)</param>
        /// <param name="arguments">the arguments to the command</param>
        /// <param name="convertedArguments">The converted arguments</param>
        /// <param name="errorMessage">an error message if conversion fails</param>
        /// <returns>true if types were converted successfully</returns>
        bool ConvertArguments(string invokedName, IActor actor, object[] arguments, out object[] convertedArguments, out IMessage errorMessage);

        /// <summary>
        /// Invoke the given command
        /// </summary>
        /// <param name="invokedName">the command name or alias that was used to invoke this command</param>
        /// <param name="actor">the entity invoking the command (Player or Mobile)</param>
        /// <param name="arguments">the arguments to the command</param>
        /// <returns>A message to be returned to the player if any</returns>
        IMessage Invoke(string invokedName, IActor actor, object[] arguments);

        string UsageHelp();

        /// <summary>
        /// Returns a single line help description for displaying in a list
        /// </summary>
        /// <returns></returns>
        string ShortHelp();
    }
}
