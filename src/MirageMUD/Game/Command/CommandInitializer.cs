using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Mirage.Core.Collections;
using Mirage.Game.Command.ArgumentConversion;
using System.Collections.Generic;
using Mirage.Core.Command.ArgumentConversion;
using Mirage.Core.Command;
using Mirage.Core.Server;

namespace Mirage.Game.Command
{    
    public class CommandInitializer : IInitializer
    {
        private ILogger logger;
        public ILogger Logger
        {
            get { return logger ?? NullLogger.Instance; }
            set { logger = value; }
        }

        public IList<CustomAttributeConverter> Converters { get; set; }

        public IReflectedCommandFactory CommandGroupFactory { get; set; }

        public void Execute()
        {
            foreach (var converter in Converters)
            {
                ReflectedCommand.Converters[converter.AttributeType] = converter.Convert;
            }
            foreach (Assembly assmbly in AssemblyList.Instance)
            {
                Logger.Info("Looking for commands in " + assmbly);
                var q = from t in assmbly.GetTypes()
                        where !t.IsAbstract && (t.IsClass || t.IsValueType)
                        && t.GetMethods().Any((mi)=>(mi.IsDefined(typeof(CommandAttribute), false)))
                        select t;
                foreach (Type t in q)
                {
                    Logger.Debug("Registering commands found in " + t);
                    CommandInvoker.Instance.RegisterTypeMethods(t, CommandGroupFactory);
                }
            }
        }
    }
}
