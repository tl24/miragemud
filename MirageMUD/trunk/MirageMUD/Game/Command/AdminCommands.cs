﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.Command;
using Mirage.Game.World;
using Mirage.Game.Communication;
using Mirage.Core;
using Castle.Core.Logging;

namespace Mirage.Game.Command
{
    [CommandDefaults(Roles="Admin")]
    public class AdminCommands
    {
        private ILogger logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        [Command]
        public void Shutdown([Actor]IActor actor)
        {
            Logger.Info("Shutdown initiated by " + actor);
            foreach (IPlayer player in MudFactory.GetObject<IPlayerRepository>())
                player.Write(MudFactory.GetObject<IMessageFactory>().GetMessage("shutdown", "The mud is shutting down"));
            MudFactory.GetObject<MirageServer>().Shutdown = true;
        }
    }
}