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
        public static void ExecuteCommand(Living actor, string input)
        {
            if (actor == null)
            {
                return;
            }

            bool success = false;
            Player player = actor as Player;
            if (player != null && player.Interpreter != null)
            {
                if (!player.Interpreter.Execute(actor, input))
                {
                    actor.Write(new ErrorResourceMessage("InvalidCommand"));
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
        public bool Execute(Living actor, string input)
        {
            return MethodInvoker.Interpret(actor, input);
        }

        /// <summary>
        ///     Say something to everyone in the room
        /// </summary>
        /// <param name="actor">the player speaking</param>
        /// <param name="args">the message to speak</param>
        /// <param name="extraArgs"></param>
        [Command(Aliases=new string[]{"'", "say"})]
        public static Message say([Actor] Living actor, [CustomParse] string message)
        {
            //speak to all others in the room
            ResourceMessage msgToOthers = new ResourceMessage(MessageType.Communication, Namespaces.Communication, "say.others");
            msgToOthers.Parameters["player"] = actor.Title;
            msgToOthers.Parameters["message"] = message;
            foreach (Living am in actor.Container.Contents(typeof(Living)))
            {
                if (am != actor)
                {
                    am.Write(msgToOthers);
                }
            }

            //repeat message to yourself as confirmation
            ResourceMessage msgToSelf = new ResourceMessage(MessageType.Confirmation, Namespaces.Communication, "say.self");
            msgToSelf.Parameters["message"] = message;
            return msgToSelf;
        }

        [Command]
        public static Message tell([Actor] Living actor, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player) QueryManager.GetInstance().Find(new ObjectQuery(null, "/Players", new ObjectQuery(target)));
            if (p == null)
            {
                // couldn't find them, send an error
                ErrorResourceMessage errorMsg = new ErrorResourceMessage("PlayerNotPlaying");
                errorMsg.Parameters["player"] = target;
                return errorMsg;
            }
            else
            {
                // format the messages
                ResourceMessage msgToTarget = new ResourceMessage(MessageType.Communication, Namespaces.Communication, "tell.others");
                msgToTarget.Parameters["player"] = actor.Title;
                msgToTarget.Parameters["message"] = message;
                p.Write(msgToTarget);

                ResourceMessage msgToSelf = new ResourceMessage(MessageType.Confirmation, Namespaces.Communication, "tell.self");
                msgToSelf.Parameters["message"] = message;
                msgToSelf.Parameters["target"] = p.Title;

                return msgToSelf;
            }
        }

        [Command]
        public static void quit([Actor] Player player)
        {
            player.Write(new ResourceMessage(MessageType.Information, Namespaces.System, "goodbye"));
            if (player.Client.State == ConnectedState.Playing)
            {
                player.save();
            }
            // comment out for now
            //player.FirePlayerEvent(Player.PlayerEventType.Quiting);
            player.Client.Close();
        }

        [Command]
        public static string look([Actor] Living actor)
        {
            string result = "";
            IViewable viewableContainer = actor.Container as IViewable;
            if (viewableContainer != null)
            {
                result += viewableContainer.Title + "\r\n";
                result += viewableContainer.ShortDescription + "\r\n";
                result += "\r\n";
            }
            if (actor.Container is Room)
            {
                Room room = actor.Container as Room;
                if (room.Animates.Count > 1)
                {
                    result += "Players:\r\n";
                    foreach (Living animate in room.Animates)
                    {
                        if (animate != actor)
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
        bool Execute(Living actor, string input);
    }

}
