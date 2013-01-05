using System;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Containers;
using Mirage.Game.Command.Infrastructure;
using Mirage.Core.Messaging;

namespace Mirage.Game.Command
{
    public class MovementCommands : CommandGroupBase
    {
        public class Messages
        {
            /* go */
            public static readonly MessageDefinition Arrival = new MessageDefinition("movement.go.arrived", "${actor} has arrived.");
            public static readonly MessageDefinition Departure = new MessageDefinition("movement.go.depart", "${actor} leaves ${direction}.");
            public static readonly MessageDefinition GoSelf = new MessageDefinition("movement.go.self", "You go ${direction}.");
            public static readonly MessageDefinition CantGoExit = new MessageDefinition("movement.go.self.error.cantgoexit", "You can't go that way.");
            public static readonly MessageDefinition DoorLocked = new MessageDefinition("movement.go.self.error.doorlocked", "The door is locked.");
            public static readonly MessageDefinition DoorClosed = new MessageDefinition("movement.go.self.error.doorclosed", "The door is closed.");

            /* lock/unlock */
            public static readonly MessageDefinition UnlockDoorSelf = new MessageDefinition("movement.unlock.door.self", "You unlock the ${direction} door.");
            public static readonly MessageDefinition UnlockDoorOthers = new MessageDefinition("movement.unlock.door.self", "${actor} unlocks the ${direction} door.");
            public static readonly MessageDefinition LockDoorSelf = new MessageDefinition("movement.lock.door.self", "You lock the ${direction} door.");
            public static readonly MessageDefinition LockDoorOthers = new MessageDefinition("movement.lock.door", "${actor} locks the ${direction} door.");

            /* open/close */
            public static readonly MessageDefinition OpenDoor = new MessageDefinition("movement.open.door", "${actor} opens the ${direction} door.");
            public static readonly MessageDefinition OpenDoorSelf = new MessageDefinition("movement.open.door.self", "You open the ${direction} door.");
            public static readonly MessageDefinition OpenDoorAnonymous = new MessageDefinition("movement.open.door.anonymous", "A door opens.");
            public static readonly MessageDefinition CloseDoor = new MessageDefinition("movement.close.door", "${actor} closes the ${direction} door.");
            public static readonly MessageDefinition CloseDoorSelf = new MessageDefinition("movement.close.door.self", "You close the ${direction} door.");
            public static readonly MessageDefinition CloseDoorAnonymous = new MessageDefinition("movement.close.door.anonymous", "A door closes.");

            /* common errors */
            public static readonly MessageDefinition NotInRoom = new MessageDefinition("movement.common.error.notinroom", "You aren't in a room.\r\nYou need to get out first.");
            public static readonly MessageDefinition NoExit = new MessageDefinition("movement.common.error.noexit", "There's no door in that direction.");
            public static readonly MessageDefinition NotADoor = new MessageDefinition("movement.common.error.notadoor", "The exit in that direction does not have a door.");
            public static readonly MessageDefinition DoorNotLockable = new MessageDefinition("movement.common.error.doornotlockable", "The exit in that direction can not be locked.");
            public static readonly MessageDefinition InvalidDirection = new MessageDefinition("movement.common.error.invaliddirection", "That's not a valid direction.");

        }

        [CommandAttribute(Priority=80)]
        public IMessage north([Actor]Living actor)
        {
            return Go(actor, DirectionType.North);
        }

        [CommandAttribute(Priority = 80)]
        public IMessage south([Actor]Living actor)
        {
            return Go(actor, DirectionType.South);
        }
        [CommandAttribute(Priority = 80)]
        public IMessage east([Actor]Living actor)
        {
            return Go(actor, DirectionType.East);
        }
        [CommandAttribute(Priority = 80)]
        public IMessage west([Actor]Living actor)
        {
            return Go(actor, DirectionType.West);
        }
        [CommandAttribute(Priority = 80)]
        public IMessage up([Actor]Living actor)
        {
            return Go(actor, DirectionType.Up);
        }
        [CommandAttribute(Priority = 80)]
        public IMessage down([Actor]Living actor)
        {
            return Go(actor, DirectionType.Down);
        }

        public IMessage Go(Living actor, DirectionType direction)
        {
            if (actor.Room == null)
                return actor.ForSelf(Messages.NotInRoom);

            string dirName = direction.ToString().ToLower();
            Room inRoom = actor.Room;
            if (!inRoom.Exits.ContainsKey(direction))
                return actor.ForSelf(Messages.CantGoExit);

            RoomExit exit = inRoom.Exits[direction];
            if (LockableAttribute.IsLocked(exit))
                return actor.ForSelf(Messages.DoorLocked);

            if (!OpenableAttribute.IsOpen(exit))
                return actor.ForSelf(Messages.DoorClosed);

            try
            {
                ContainerUtils.Transfer(actor, exit.TargetRoom);
                actor.ToRoom(inRoom, Messages.Departure, null, new { direction = dirName });
                actor.ToRoom(exit.TargetRoom, Messages.Arrival);
                actor.ToSelf(Messages.GoSelf, new { direction = dirName });
                if (actor is IPlayer)
                    Interpreter.ExecuteCommand(actor, "look");
                return null;
            }
            catch (ContainerAddException)
            {
                return actor.ForSelf(Messages.CantGoExit);
            }
        }

        private int ParseDirection(string dir)
        {
            foreach (string name in Enum.GetNames(typeof(DirectionType)))
            {
                if (name.ToLower().StartsWith(dir.ToLower()))
                    return (int) Enum.Parse(typeof(DirectionType), name);
            }
            return -1;
        }

        [Command(Aliases = new string[] { "open" })]
        public IMessage OpenDoor([Actor] Living actor, [Const("door")] string door, DirectionType direction)
        {
            return OpenCloseDoorHelper(actor, direction, true);
        }

        [Command(Aliases = new string[] { "close" })]
        public IMessage CloseDoor([Actor] Living actor, [Const("door")] string door, DirectionType direction)
        {
            return OpenCloseDoorHelper(actor, direction, false);
        }

        private IMessage OpenCloseDoorHelper(Living actor, DirectionType direction, bool open)
        {
            if (actor.Room == null)
                return actor.ForSelf(Messages.NotInRoom);

            string dirName = direction.ToString().ToLower();
            Room room = actor.Room;
            if (!room.Exits.ContainsKey(direction))
                return actor.ForSelf(Messages.NoExit);

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(IOpenable)))
                return actor.ForSelf(Messages.NotADoor);

            IOpenable openObj = (IOpenable)exit.GetAttribute(typeof(IOpenable));
            if (open)
                openObj.Open();
            else
                openObj.Close();

            string action = open ? "Open" : "Close";
            var messageSelf = open ? Messages.OpenDoorSelf : Messages.CloseDoorSelf;
            var messageOthers = open ? Messages.OpenDoor : Messages.CloseDoor;
            var messageAnonymous = open ? Messages.OpenDoorAnonymous : Messages.CloseDoorAnonymous;

            var msgArgs = new { direction = direction.ToString() };
            // notify the people in this room
            actor.ToRoom(messageOthers, null, msgArgs);
            // notify the people in the adjoining room
            actor.ToRoom(exit.TargetRoom, messageAnonymous); 

            return actor.ForSelf(messageSelf, msgArgs);
        }

        [Command(Aliases = new string[] { "unlock" })]
        public IMessage UnlockDoor([Actor] Living actor, [Const("door")] string door, DirectionType direction)
        {
            return UnlockLockDoorHelper(actor, direction, true);
        }

        [Command(Aliases = new string[] { "lock" })]
        public IMessage LockDoor([Actor] Living actor, [Const("door")] string door, DirectionType direction)
        {
            return UnlockLockDoorHelper(actor, direction, false);
        }


        private IMessage UnlockLockDoorHelper(Living actor, DirectionType direction, bool unlock)
        {
            if (actor.Room == null)
                return actor.ForSelf(Messages.NotInRoom);

            string dirName = direction.ToString().ToLower();
            Room room = actor.Room;
            if (!room.Exits.ContainsKey(direction))
                return actor.ForSelf(Messages.NoExit);

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(ILockable)))
                return actor.ForSelf(Messages.DoorNotLockable);


            ILockable lockObj = (ILockable)exit.GetAttribute(typeof(ILockable));
            // we'll do the lock/unlock without a key for now
            if (unlock)
                lockObj.Unlock();
            else
                lockObj.Lock();

            string action = unlock ? "Unlock" : "Lock";
            var selfMessage = unlock ? Messages.UnlockDoorSelf : Messages.LockDoorSelf;
            var othersMessage = unlock ? Messages.UnlockDoorOthers : Messages.LockDoorOthers;
            var args = new { direction = direction.ToString() };
            actor.ToRoom(othersMessage, null, args);
            return actor.ForSelf(selfMessage, null, args);
        }
        /// <summary>
        /// Attempts to convert a string into a direction.  Takes into account abbrievations
        /// </summary>
        /// <param name="argument">the destination argument</param>
        /// <param name="context">conversion context</param>
        /// <returns>converted argument</returns>
        public object DirectionConverter(Argument argument, ArgumentConversionContext context)
        {
            int dir = ParseDirection((string) context.GetCurrentAndIncrement());
            if (dir == -1)
            {
                context.ErrorMessage = context.Actor.ForSelf(Messages.InvalidDirection);
                return null;
            }
            return (DirectionType)dir;
        }

        public override void InitializeArgumentHandlers(ArgumentList arguments)
        {
            foreach (Argument arg in arguments)
            {
                if (arg.Parameter.ParameterType == typeof(DirectionType))
                {
                    arg.Handler = DirectionConverter;
                }
            }
        }
    }
}
