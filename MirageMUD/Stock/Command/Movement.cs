using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using Mirage.Core.Data.Attribute;
using Mirage.Stock.Data;
using Mirage.Core.Command;
using Mirage.Core.Data.Containers;

namespace Mirage.Stock.Command
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
                ContainerUtils.Transfer(actor, exit.TargetRoom);
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
                actor.Write(new StringMessage(MessageType.Confirmation, "Movement." + dirName, "You go " + dirName + ".\r\n"));
                if (actor is IPlayer)
                    Interpreter.ExecuteCommand(actor, "look");
                return null;
            }
            catch (ContainerAddException)
            {
                return MessageFactory.GetMessage("msg:/movement/cant.go.exit");
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
                return MessageFactory.GetMessage("msg:/movement/not.in.room");

            string dirName = direction.ToString().ToLower();
            Room room = (Room)actor.Container;
            if (!room.Exits.ContainsKey(direction))
                return MessageFactory.GetMessage("msg:/movement/no.exit");

            RoomExit exit = room.Exits[direction];
            if (!exit.HasAttribute(typeof(ILockable)))
                return MessageFactory.GetMessage("msg:/movement/door.not.lockable");


            ILockable lockObj = (ILockable)exit.GetAttribute(typeof(ILockable));
            // we'll do the lock/unlock without a key for now
            if (unlock)
                lockObj.Unlock();
            else
                lockObj.Lock();

            string action = unlock ? "unlock" : "lock";
            ResourceMessage mActionSelf = (ResourceMessage)MessageFactory.GetMessage("msg:/movement/player." + action + ".door.self");
            ResourceMessage mActionOthers = (ResourceMessage)MessageFactory.GetMessage("msg:/movement/player." + action + ".door.others");

            mActionOthers["player"] = actor.Title;
            mActionSelf["direction"] = mActionOthers["direction"] = direction.ToString();

            foreach (Living liv in room.Animates)
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
                context.ErrorMessage = MessageFactory.GetMessage("msg:/movement/invalid.direction");
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
