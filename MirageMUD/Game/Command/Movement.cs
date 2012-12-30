using System;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Containers;

namespace Mirage.Game.Command
{
    public class Movement : CommandGroupBase
    {
        class Messages
        {
            public static ResourceMessage Arrival = new ResourceMessage("movement.go.arrived") { Template = "${player} has arrived.\r\n" };

            public static ResourceMessage Departure = new ResourceMessage("movement.go.depart") { Template = "${player} leaves ${direction}.\r\n" };

            public static ResourceMessage GoSelf = new ResourceMessage("movement.go.self") { Template = "You go ${direction}.\r\n" };

            public static StringMessage InvalidDirection = new StringMessage("movement.go.self.failed.invaliddirection",
                "That's not a valid direction.\r\n");

            public static ResourceMessage CloseDoorAnonymous = new ResourceMessage("movement.door.close.anonymous") { Template = "A door closes.\r\n" };

            public static ResourceMessage OpenDoorAnonymous = new ResourceMessage("movement.door.open.anonymous") { Template = "A door opens.\r\n" };

            public static ResourceMessage CloseDoor = new ResourceMessage("movement.door.close") { Template = "${player} closes the ${direction} door.\r\n" };

            public static ResourceMessage CloseDoorSelf = new ResourceMessage("movement.door.self.close") { Template = "You close the ${direction} door.\r\n" };

            public static ResourceMessage OpenDoor = new ResourceMessage("movement.door.open") { Template = "${player} opens the ${direction} door.\r\n" };

            public static ResourceMessage OpenDoorSelf = new ResourceMessage("movement.door.self.open") { Template = "You open the ${direction} door.\r\n" };

            /*

            "PlayerUnlockDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerUnlockDoorSelf")
            { Template = "You unlock the ${direction} door.\r\n" };

            "PlayerUnlockDoorOthers" :  new ResourceMessage (MessageType.Information, "PlayerUnlockDoorOthers")
            { Template = "${player} unlocks the ${direction} door.\r\n" };

            "PlayerLockDoorSelf" :  new ResourceMessage ("Confirmation", "PlayerLockDoorSelf")
            { Template = "You lock the ${direction} door.\r\n" };

            "PlayerLockDoorOthers" :  new ResourceMessage (MessageType.Information, "PlayerLockDoorOthers")
            { Template = "${player} locks the ${direction} door.\r\n" };
		
		
            "NotInRoom" : new StringMessage("PlayerError", "NotInRoom", "You aren't in a room.\r\nYou need to get out first.\r\n"),
	
            "NoExit" : new StringMessage("PlayerError", "NoExit", "There's no door in that direction.\r\n"),
		
            "CantGoExit" : new StringMessage("PlayerError", "CantGoExit", "You can't go that way.\r\n"),
		
            "NotADoor" : new StringMessage("PlayerError", "NotADoor", "The exit in that direction does not have a door.\r\n"),
		
            "DoorAlreadyOpen" : new StringMessage("PlayerError", "DoorAlreadyOpen", "The door is already open.\r\n"),
		
            "DoorAlreadyClosed" : new StringMessage("PlayerError", "DoorAlreadyClosed", "The door is already closed.\r\n"),
		
            "DoorLocked" : new StringMessage("PlayerError", "DoorLocked", "The door is locked.\r\n"),
		
            "DoorClosed" : new StringMessage("PlayerError", "DoorClosed", "The door is closed.\r\n"),
		
            "DoorNotLockable" : new StringMessage("PlayerError", "DoorNotLockable", "The exit in that direction can not be locked.\r\n")
            */

        }

        private IMessageFactory _messageFactory;

        public IMessageFactory MessageFactory
        {
            get { return _messageFactory; }
            set { _messageFactory = value; }
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
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = actor.Room;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("movement.CantGoExit");

            RoomExit exit = room.Exits[direction];
            if (LockableAttribute.IsLocked(exit))
                return MessageFactory.GetMessage("movement.DoorLocked");

            if (!OpenableAttribute.IsOpen(exit))
                return MessageFactory.GetMessage("movement.DoorClosed");

            // create the messages
            IMessage departMessage = Messages.Departure.Copy();
            departMessage["player"] = actor.Title;
            departMessage["direction"] = dirName;

            IMessage arrivalMessage = Messages.Arrival.Copy();
            arrivalMessage["player"] = actor.Title;

            try
            {
                ContainerUtils.Transfer(actor, exit.TargetRoom);
                room.Write(actor, departMessage);
                exit.TargetRoom.Write(actor, arrivalMessage);

                IMessage confirmation = Messages.GoSelf.Copy();
                confirmation["direction"] = dirName;
                actor.Write(confirmation);
                if (actor is IPlayer)
                    Interpreter.ExecuteCommand(actor, "look");
                return null;
            }
            catch (ContainerAddException)
            {
                return MessageFactory.GetMessage("movement.CantGoExit");
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
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = actor.Room;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("movement.NoExit");

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(IOpenable)))
                return MessageFactory.GetMessage("movement.NotADoor");

            IOpenable openObj = (IOpenable)exit.GetAttribute(typeof(IOpenable));
            if (open)
                openObj.Open();
            else
                openObj.Close();

            string action = open ? "Open" : "Close";
            IMessage mActionSelf = MessageFactory.GetMessage("movement.Player" + action + "DoorSelf");
            IMessage mActionOthers = MessageFactory.GetMessage("movement.Player" + action + "DoorOthers");

            mActionOthers["player"] = actor.Title;
            mActionSelf["direction"] = mActionOthers["direction"] = direction.ToString();

            IMessage mActionAnonymous = MessageFactory.GetMessage("movement.Anonymous" + action + "Door");
            room.Write(actor, mActionOthers);

            exit.TargetRoom.Write(mActionAnonymous);

            return mActionSelf;
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
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = actor.Room;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("movement.NoExit");

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(ILockable)))
                return MessageFactory.GetMessage("movement.DoorNotLockable");


            ILockable lockObj = (ILockable)exit.GetAttribute(typeof(ILockable));
            // we'll do the lock/unlock without a key for now
            if (unlock)
                lockObj.Unlock();
            else
                lockObj.Lock();

            string action = unlock ? "Unlock" : "Lock";
            IMessage mActionSelf = MessageFactory.GetMessage("movement.Player" + action + "DoorSelf");
            IMessage mActionOthers = MessageFactory.GetMessage("movement.Player" + action + "DoorOthers");

            mActionOthers["player"] = actor.Title;
            mActionSelf["direction"] = mActionOthers["direction"] = direction.ToString();

            foreach (Living liv in room.LivingThings)
            {
                if (liv != actor)
                    liv.Write(mActionOthers);
            }

            return mActionSelf;
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
                context.ErrorMessage = MessageFactory.GetMessage("movement.InvalidDirection");
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
                    arg.Handler = new ConvertArgumentHandler(DirectionConverter);
                }
            }
        }
    }
}
