using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data;
using Shoop.Communication;
using Shoop.Data.Attribute;

namespace Shoop.Command
{
    public class Movement
    {
        [Command]
        public static Message north([Actor]Player self)
        {
            return Go(self, DirectionType.North);
        }

        [Command]
        public static Message south([Actor]Player self)
        {
            return Go(self, DirectionType.South);
        }
        [Command]
        public static Message east([Actor]Player self)
        {
            return Go(self, DirectionType.East);
        }
        [Command]
        public static Message west([Actor]Player self)
        {
            return Go(self, DirectionType.West);
        }
        [Command]
        public static Message up([Actor]Player self)
        {
            return Go(self, DirectionType.Up);
        }
        [Command]
        public static Message down([Actor]Player self)
        {
            return Go(self, DirectionType.Down);
        }

        public static Message Go(Animate animate, DirectionType direction)
        {
            if (animate.Container is Room)
            {
                string dirName = direction.ToString().ToLower();
                Room room = (Room)animate.Container;
                if (room.Exits.ContainsKey(direction))
                {
                    RoomExit exit = room.Exits[direction];
                    if (exit.HasAttribute(typeof(IOpenable)))
                    {
                        IOpenable openObj = (IOpenable) exit.GetAttribute(typeof(IOpenable));
                        if (!openObj.IsOpen())
                        {
                            return new ErrorMessage("Error.ExitClosed", "The door is closed.");
                        }
                    }
                    // create the messages
                    MovementMessage departMessage = new MovementMessage(dirName,
                        MovementMessage.MovementType.Departure,
                        animate.Title);
                    MovementMessage arrivalMessage = new MovementMessage(null,
                        MovementMessage.MovementType.Arrival,
                        animate.Title);

                    try
                    {
                        Containers.Transfer(animate, exit.TargetRoom);
                        foreach (Animate oldRoomPeople in room.Animates)
                        {
                            if (oldRoomPeople != animate)
                            {
                                oldRoomPeople.Write(departMessage);
                            }
                        }
                        foreach (Animate newRoomPeople in exit.TargetRoom.Animates)
                        {
                            if (newRoomPeople != animate)
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
    }
}
