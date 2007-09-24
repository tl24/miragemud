using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using Mirage.Core.Data.Attribute;

namespace Mirage.Core.Command
{
    public class Movement
    {
        [Command]
        public static IMessage north([Actor]Living actor)
        {
            return Go(actor, DirectionType.North);
        }

        [Command]
        public static IMessage south([Actor]Living actor)
        {
            return Go(actor, DirectionType.South);
        }
        [Command]
        public static IMessage east([Actor]Living actor)
        {
            return Go(actor, DirectionType.East);
        }
        [Command]
        public static IMessage west([Actor]Living actor)
        {
            return Go(actor, DirectionType.West);
        }
        [Command]
        public static IMessage up([Actor]Living actor)
        {
            return Go(actor, DirectionType.Up);
        }
        [Command]
        public static IMessage down([Actor]Living actor)
        {
            return Go(actor, DirectionType.Down);
        }

        public static IMessage Go(Living actor, DirectionType direction)
        {
            if (actor.Container is Room)
            {
                string dirName = direction.ToString().ToLower();
                Room room = (Room)actor.Container;
                if (room.Exits.ContainsKey(direction))
                {
                    RoomExit exit = room.Exits[direction];
                    if (exit.HasAttribute(typeof(ILockable)))
                    {
                        ILockable lockObj = (ILockable)exit.GetAttribute(typeof(ILockable));
                        if (lockObj.IsLocked())
                        {
                            return new ErrorMessage("Error.ExitLocked", "The door is locked.\r\n");
                        }
                    }
                    if (exit.HasAttribute(typeof(IOpenable)))
                    {
                        IOpenable openObj = (IOpenable) exit.GetAttribute(typeof(IOpenable));
                        if (!openObj.IsOpen())
                        {
                            return new ErrorMessage("Error.ExitClosed", "The door is closed.\r\n");
                        }
                    }
                    // create the messages
                    MovementMessage departMessage = new MovementMessage(dirName,
                        MovementMessage.MovementType.Departure,
                        actor.Title);
                    MovementMessage arrivalMessage = new MovementMessage(null,
                        MovementMessage.MovementType.Arrival,
                        actor.Title);

                    try
                    {
                        Containers.Transfer(actor, exit.TargetRoom);
                        foreach (Living oldRoomPeople in room.Animates)
                        {
                            if (oldRoomPeople != actor)
                            {
                                oldRoomPeople.Write(departMessage);
                            }
                        }
                        foreach (Living newRoomPeople in exit.TargetRoom.Animates)
                        {
                            if (newRoomPeople != actor)
                            {
                                newRoomPeople.Write(arrivalMessage);
                            }
                        }
                        return new StringMessage(MessageType.Confirmation, "Movement." + dirName, "You go " + dirName + ".\r\n");
                    }
                    catch (ContainerAddException e)
                    {
                        return new ErrorMessage("Error.Movement", "You can't go that way.\r\n");
                    }
                } else {
                    return new ErrorMessage("Error.Movement", "You can't go that way.\r\n");
                }
            }
            else
            {
                return new ErrorMessage("Error.Movement", "You aren't in a room.\r\nYou need to get out first.\r\n");
            }
        }

        private static int ParseDirection(string dir)
        {
            foreach (string name in Enum.GetNames(typeof(DirectionType)))
            {
                if (name.ToLower().StartsWith(dir.ToLower()))
                    return (int) Enum.Parse(typeof(DirectionType), name);
            }
            return -1;
        }

        [Command(Aliases = new string[] { "open" })]
        public static IMessage OpenDoor([Actor] Living actor, [Const("door")] string door, string direction)
        {
            return OpenCloseDoorHelper(actor, direction, true);
        }

        [Command(Aliases = new string[] { "close" })]
        public static IMessage CloseDoor([Actor] Living actor, [Const("door")] string door, string direction)
        {
            return OpenCloseDoorHelper(actor, direction, false);
        }

        private static IMessage OpenCloseDoorHelper(Living actor, string directionName, bool open)
        {
            int dir = ParseDirection(directionName);
            if (dir == -1)
                return new ErrorMessage("InvalidDirection", "That's not a valid direction");
            DirectionType direction = (DirectionType)dir;

            if (actor.Container is Room)
            {
                string dirName = direction.ToString().ToLower();
                Room room = (Room)actor.Container;
                if (room.Exits.ContainsKey(direction))
                {
                    RoomExit exit = room.Exits[direction];
                    if (exit.HasAttribute(typeof(IOpenable)))
                    {
                        IOpenable openObj = (IOpenable)exit.GetAttribute(typeof(IOpenable));
                        if (openObj.IsOpen() && open)
                            return new ErrorMessage("Error.ExitAlreadyOpen", "The door is already open.\r\n");
                        if (!openObj.IsOpen() && !open)
                            return new ErrorMessage("Error.ExitAlreadyClosed", "The door is already closed.\r\n");

                        if (exit.HasAttribute(typeof(ILockable)))
                        {
                            ILockable lockObj = (ILockable)exit.GetAttribute(typeof(ILockable));
                            if (lockObj.IsLocked() && open)
                            {
                                return new ErrorMessage("Error.ExitLocked", "The door is locked.\r\n");
                            }
                            if (open)
                                lockObj.Open();
                            else
                                lockObj.Close();
                        } else {
                            if (open)
                                openObj.Open();
                            else
                                openObj.Close();
                        }
                        string action = open ? "open" : "close";
                        ResourceMessage mActionSelf = 
                            new ResourceMessage(MessageType.Confirmation, Namespaces.Movement,  "player." + action + ".door.self");
                        ResourceMessage mActionOthers = 
                            new ResourceMessage(MessageType.Information, Namespaces.Movement,  "player." + action + ".door.others");
                        mActionOthers.Parameters["player"] = actor.Title;
                        mActionSelf.Parameters["direction"] = mActionOthers.Parameters["direction"] = direction.ToString();

                        ResourceMessage mActionAnonymous = 
                            new ResourceMessage(MessageType.Information, Namespaces.Movement,  "anonymous." + action + ".door");

                        foreach (Living liv in room.Animates)
                        {
                            if (liv != actor)
                                liv.Write(mActionOthers);
                        }

                        foreach (Living liv in exit.TargetRoom.Animates)
                        {
                            liv.Write(mActionAnonymous);
                        }

                        return mActionSelf;
                    } else {
                        return new ErrorMessage("Error.NoDoor", "The exit in that direction does not have a door.\r\n");
                    }
                }
                else
                {
                    return new ErrorMessage("Error.Movement", "There's no door in that direction.\r\n");
                }
            }
            else
            {
                return new ErrorMessage("Error.Movement", "You aren't in a room.\r\nYou need to get out first.\r\n");
            }
        }        
    }
}
