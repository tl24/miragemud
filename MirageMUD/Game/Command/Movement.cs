using System;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Containers;

namespace Mirage.Game.Command
{
    public class Movement : CommandGroupBase
    {
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
            if (!(actor.Container is Room))
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("movement.CantGoExit");

            RoomExit exit = room.Exits[direction];
            if (LockableAttribute.IsLocked(exit))
                return MessageFactory.GetMessage("movement.DoorLocked");

            if (!OpenableAttribute.IsOpen(exit))
                return MessageFactory.GetMessage("movement.DoorClosed");

            // create the messages
            IMessage departMessage = MessageFactory.GetMessage("movement.PlayerLeavesDirection");
            departMessage["player"] = actor.Title;
            departMessage["direction"] = dirName;

            IMessage arrivalMessage = MessageFactory.GetMessage("movement.PlayerArrival");
            arrivalMessage["player"] = actor.Title;

            try
            {
                ContainerUtils.Transfer(actor, exit.TargetRoom);
                room.Write(actor, departMessage);
                exit.TargetRoom.Write(actor, arrivalMessage);

                IMessage confirmation = MessageFactory.GetMessage("movement.YouLeaveDirection");
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
            if (!(actor.Container is Room))
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
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
            if (!(actor.Container is Room))
                return MessageFactory.GetMessage("movement.NotInRoom");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
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
