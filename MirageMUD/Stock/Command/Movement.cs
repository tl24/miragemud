using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using Mirage.Core.Data.Attribute;
using Mirage.Stock.Data;
using Mirage.Core.Command;

namespace Mirage.Stock.Command
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
            if (!(actor.Container is Room))
                return MessageFactory.GetMessage("msg:/movement/not.in.room");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("msg:/movement/cant.go.exit");

            RoomExit exit = room.Exits[direction];
            if (LockableAttribute.IsLocked(exit))
                return MessageFactory.GetMessage("msg:/movement/door.locked");

            if (!OpenableAttribute.IsOpen(exit))
                return MessageFactory.GetMessage("msg:/movement/door.closed");

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
                return MessageFactory.GetMessage("msg:/movement/cant.go.exit");
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
                return MessageFactory.GetMessage("msg:/movement/invalid.direction");

            DirectionType direction = (DirectionType)dir;

            if (!(actor.Container is Room))
                return MessageFactory.GetMessage("msg:/movement/not.in.room");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("msg:/movement/no.exit");

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(IOpenable)))
                return MessageFactory.GetMessage("msg:/movement/not.a.door");

            IOpenable openObj = (IOpenable)exit.GetAttribute(typeof(IOpenable));
            if (open)
                openObj.Open();
            else
                openObj.Close();

            string action = open ? "open" : "close";
            ResourceMessage mActionSelf = (ResourceMessage)MessageFactory.GetMessage("msg:/movement/player." + action + ".door.self");
            ResourceMessage mActionOthers = (ResourceMessage)MessageFactory.GetMessage("msg:/movement/player." + action + ".door.others");

            mActionOthers["player"] = actor.Title;
            mActionSelf["direction"] = mActionOthers["direction"] = direction.ToString();

            ResourceMessage mActionAnonymous = (ResourceMessage)MessageFactory.GetMessage("msg:/movement/anonymous." + action + ".door");

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
        }        
    }
}
