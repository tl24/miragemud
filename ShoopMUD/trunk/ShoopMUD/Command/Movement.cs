using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data;
using Shoop.Communication;
using Shoop.Attributes;

namespace Shoop.Command
{
    public class Movement
    {
        [Shoop.Attributes.Command]
        public static Message north([ArgumentType(ArgumentType.Self)]Player self)
        {
            return Go(self, DirectionType.North);
        }

        [Shoop.Attributes.Command]
        public static Message south([ArgumentType(ArgumentType.Self)]Player self)
        {
            return Go(self, DirectionType.South);
        }
        [Shoop.Attributes.Command]
        public static Message east([ArgumentType(ArgumentType.Self)]Player self)
        {
            return Go(self, DirectionType.East);
        }
        [Shoop.Attributes.Command]
        public static Message west([ArgumentType(ArgumentType.Self)]Player self)
        {
            return Go(self, DirectionType.West);
        }
        [Shoop.Attributes.Command]
        public static Message up([ArgumentType(ArgumentType.Self)]Player self)
        {
            return Go(self, DirectionType.Up);
        }
        [Shoop.Attributes.Command]
        public static Message down([ArgumentType(ArgumentType.Self)]Player self)
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
