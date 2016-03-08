using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Server;
using Mirage.Core.Command;

namespace SampleMud
{
    public class CommandsInitializer : IInitializer
    {
        public void Execute()
        {
            CommandInvoker.Instance.RegisterTypeMethods(typeof(SampleCommands), new ReflectedCommandFactory());
        }
    }
}
