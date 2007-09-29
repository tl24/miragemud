using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;
using Mirage.Core.IO;
using Mirage.Core.Data.Attribute;
using Mirage.Stock.Data;

namespace Mirage.Stock.Command
{
    public static class MiscCommands
    {
        /// <summary>
        ///     Say something to everyone in the room
        /// </summary>
        /// <param name="actor">the player speaking</param>
        /// <param name="args">the message to speak</param>
        /// <param name="extraArgs"></param>
        [Command(Aliases = new string[] { "'", "say" })]
        public static IMessage say([Actor] Living actor, [CustomParse] string message)
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
        public static IMessage tell([Actor] Living actor, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player)QueryManager.GetInstance().Find(new ObjectQuery(null, "/Players", new ObjectQuery(target)));
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
                    foreach (RoomExit exit in room.Exits.Values)
                    {
                        if (OpenableAttribute.IsOpen(exit))
                        {
                            result += exit.Direction;
                            result += " ";
                        }
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
}
