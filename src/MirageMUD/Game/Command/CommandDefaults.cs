using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.World;
using Castle.Core.Logging;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Base class for commands
    /// </summary>
    public abstract class CommandDefaults
    {
        public MudWorld World { get; set; }

        private ILogger logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
    }
}
