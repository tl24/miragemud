using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shoop.Data;
using Shoop.IO;
using System.Reflection;
using Shoop.Attributes;
using Shoop.Communication;

namespace Shoop.Command
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
            else {
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

    /// <summary>
    ///     A class for interpreting commands from players.
    /// Use the execute Command method.
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
        public static IInterpret getDefaultInterpreter()
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
        public static void executeCommand(Player actor, string input)
        {
            if (actor == null)
            {
                return;
            }

            bool success = false;
            if (actor.Interpreter != null)
            {
                if (!actor.Interpreter.execute(actor, input))
                {
                    actor.Write(new StringMessage(MessageType.PlayerError, "InvalidCommand", "Huh?\n\r"));
                }
            }
            else
            {
                getDefaultInterpreter().execute(actor, input);
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
        public bool execute(Player actor, string input)
        {
            return MethodInvoker.interpret(actor, input);
        }

        /// <summary>
        ///     Say something to everyone in the room
        /// </summary>
        /// <param name="actor">the player speaking</param>
        /// <param name="args">the message to speak</param>
        /// <param name="extraArgs"></param>
        [Command]
        public static string speak([ArgumentType(ArgumentType.Self)] Player player, [ArgumentType(ArgumentType.ToEOL)] string rest)
        {
            return "You said " + rest + "\r\n";
        }


        [Command]
        public static void quit([ArgumentType(ArgumentType.Self)] Player player)
        {
            player.Write(new StringMessage(MessageType.Information, "Goodbye", "Goodbye!\r\n"));
            if (player.Client.State == ConnectedState.Playing)
            {
                player.save();
            }
            player.FirePlayerEvent(Player.PlayerEventType.Quiting);
            player.Client.Close();
        }

        [Command]
        public static string look([ArgumentType(ArgumentType.Self)] Player player)
        {
            string result = "";
            IViewable viewableContainer = player.Container as IViewable;
            if (viewableContainer != null)
            {
                result += viewableContainer.Title + "\n\r";
                result += viewableContainer.ShortDescription + "\n\r";
                result += "\n\r";
            }
            if (player.Container is Room)
            {
                Room room = player.Container as Room;
                result += "Players:\n\r";
                foreach (Animate animate in room.Animates)
                {
                    if (animate != player)
                    {
                        result += animate.Title + "\r\n";
                    }
                }
            }
            return result;
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
        bool execute(Player actor, string input);
    }

    public class ConfirmationInterpreter : IInterpret
    {
        private string _message = "Are you sure? (y\\n) ";
        private string _cancellationMessage = "Command cancelled\r\n";
        private ICommand _method;
        private IInterpret priorInterpreter;
        private Player _player;
        private string _invokedName;
        private object _context;
        private string[] args;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string CancellationMessage
        {
            get { return _cancellationMessage; }
            set { _cancellationMessage = value; }
        }

        public ConfirmationInterpreter(Player player, ICommand method, string invokedName, string[] arguments, object context)
        {
            setPlayer(player);
            this._method = method;
            this._invokedName = invokedName;
            this._context = context;
            this.args = arguments;
        }

        private void setPlayer(Player player)
        {
            this._player = player;
            priorInterpreter = player.Interpreter;
            this._player.Interpreter = this;
        }

        #region IInterpret Members

        public bool execute(Player actor, string input)
        {
            bool success = false;
            input = input.ToLower();
            if (input.Equals("yes") || input.Equals("y"))
            {
                object st = _method.Invoke(_invokedName, actor, args, _context);
                if (st != null)
                {
                    if (st is Message)
                    {
                        actor.Write((Message)st);
                    }
                    else
                    {
                        actor.Write(new StringMessage(MessageType.Information, "MethodResult." + _method.Name, st.ToString()));
                    }
                }
                success = true;
            }
            else if (input.Equals("no") || input.Equals("n"))
            {
                actor.Write(new StringMessage(MessageType.Information, "Cancellation", _cancellationMessage));
                success = true;
            }
            else
            {
                success = false;
            }
            actor.Interpreter = priorInterpreter;
            return success;
        }

        #endregion

        public void requestConfirmation()
        {
            _player.Write(new StringMessage(MessageType.Prompt, "ConfirmationPrompt", _message)); 
        }
    }
}
