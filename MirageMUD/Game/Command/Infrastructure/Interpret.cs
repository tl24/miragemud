using Mirage.Game.Communication;
using Mirage.Game.World;

namespace Mirage.Game.Command.Infrastructure
{

    /// <summary>
    ///     A class for interpreting commands from players.
    /// Use the Execute Command method.
    /// </summary>
    public class Interpreter : IInterpret
    {
        private static IInterpret defaultInterpreter;
        private static object lockObject = new object();

        /// <summary>
        ///     Creates an instance of an interpreter
        /// </summary>
        public Interpreter()
        {
        }

        /// <summary>
        /// Gets the default interpreter for a player
        /// </summary>
        /// <returns>the default interpreter</returns>
        public static IInterpret GetDefaultInterpreter()
        {
            lock (lockObject)
            {
                if (defaultInterpreter == null)
                {
                    defaultInterpreter = new Interpreter();
                }
            }
            return defaultInterpreter;
        }

        /// <summary>
        ///     Executes a Command for a player.  The list of interpreters
        /// for the player will be searched until a Command is successfully
        /// executed.
        /// </summary>
        /// <param name="actor">the player</param>
        /// <param name="input">Command and arguments</param>
        /// <returns>true if a Command was executed</returns>
        public static void ExecuteCommand(IActor actor, string input)
        {
            if (actor == null)
            {
                return;
            }

            IPlayer player = actor as IPlayer;
            if (player != null && player.Interpreter != null)
            {
                if (!player.Interpreter.Execute(actor, input))
                {
                    player.ToSelf(CommonMessages.ErrorInvalidCommand);
                }
            }
            else
            {
                GetDefaultInterpreter().Execute(actor, input);
            }
        }

        /// <summary>
        ///     Execute a Command from a player.  The input string will be
        /// parsed, if it contains a valid Command that Command will
        /// be located and executed.
        /// </summary>
        /// <param name="actor">The player executing the Command</param>
        /// <param name="input">the Command and arguments</param>
        /// <returns>true if executed successfully</returns>
        public bool Execute(IActor actor, string input)
        {
            return MethodInvoker.Interpret(actor, input);
        }

        
    }

    public interface IInterpret
    {
        /// <summary>
        /// Interpret the given Command for the player
        /// </summary>
        /// <param name="actor">The player</param>
        /// <param name="input">the input string of arguments</param>
        /// <returns>true if the Command was executed</returns>
        bool Execute(IActor actor, string input);
    }

}
