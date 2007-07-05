using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data;
using Mirage.IO;

using Mirage.Communication;
using Mirage.Data.Query;

namespace Mirage.Command
{

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
                    actor.Write(new ErrorResourceMessage("InvalidCommand", "Error.InvalidCommand"));
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
        [Command(Aliases=new string[]{"'", "say"})]
        public static Message say([Actor] Player player, [CustomParse] string message)
        {
            //speak to all others in the room
            ResourceMessage msgToOthers = new ResourceMessage(MessageType.Communication, "Comm.Say", "Comm.Say.Others");
            msgToOthers.Parameters["player"] = player.Title;
            msgToOthers.Parameters["message"] = message;
            foreach (Animate am in player.Container.Contents(typeof(Animate)))
            {
                if (am != player)
                {
                    am.Write(msgToOthers);
                }
            }

            //repeat message to yourself as confirmation
            ResourceMessage msgToSelf = new ResourceMessage(MessageType.Confirmation, "Command.Say", "Comm.Say.Self");
            msgToSelf.Parameters["message"] = message;
            return msgToSelf;
        }

        [Command]
        public static Message tell([Actor] Player player, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player) QueryManager.GetInstance().Find(new ObjectQuery(null, "/Players", new ObjectQuery(target)));
            if (p == null)
            {
                // couldn't find them, send an error
                ErrorResourceMessage errorMsg = new ErrorResourceMessage("Error.PlayerNotPlaying", "Error.PlayerNotPlaying");
                errorMsg.Parameters["player"] = target;
                return errorMsg;
            }
            else
            {
                // format the messages
                ResourceMessage msgToTarget = new ResourceMessage(MessageType.Communication, "Comm.Tell", "Comm.Tell.Others");
                msgToTarget.Parameters["player"] = player.Title;
                msgToTarget.Parameters["message"] = message;
                p.Write(msgToTarget);

                ResourceMessage msgToSelf = new ResourceMessage(MessageType.Confirmation, "Command.Tell", "Comm.Tell.Self");
                msgToSelf.Parameters["message"] = message;
                msgToSelf.Parameters["target"] = p.Title;

                return msgToSelf;
            }
        }

        [Command]
        public static void quit([Actor] Player player)
        {
            player.Write(new ResourceMessage(MessageType.Information, "Goodbye", "Info.Goodbye"));
            if (player.Client.State == ConnectedState.Playing)
            {
                player.save();
            }
            player.FirePlayerEvent(Player.PlayerEventType.Quiting);
            player.Client.Close();
        }

        [Command]
        public static string look([Actor] Player player)
        {
            string result = "";
            IViewable viewableContainer = player.Container as IViewable;
            if (viewableContainer != null)
            {
                result += viewableContainer.Title + "\r\n";
                result += viewableContainer.ShortDescription + "\r\n";
                result += "\r\n";
            }
            if (player.Container is Room)
            {
                Room room = player.Container as Room;
                if (room.Animates.Count > 1)
                {
                    result += "Players:\r\n";
                    foreach (Animate animate in room.Animates)
                    {
                        if (animate != player)
                        {
                            result += animate.Title + "\r\n";
                        }
                    }
                }

                if (room.Exits.Count > 0)
                {
                    result += "Available Exits: [ ";
                    foreach (DirectionType direction in room.Exits.Keys)
                    {
                        result += direction;
                        result += " ";
                    }
                    result += "]\r\n";
                }
                else
                {
                    result += "Available Exits: none\r\n.";
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
