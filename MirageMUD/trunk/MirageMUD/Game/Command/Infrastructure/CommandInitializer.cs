using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Mirage.Core.Collections;
using Mirage.Game.Command.Infrastructure.ArgumentConversion;
using System.Collections.Generic;

namespace Mirage.Game.Command.Infrastructure
{    
    public class CommandInitializer : IInitializer
    {
        public string Name
        {
            get { return this.GetType().Name; }
        }

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
                    MethodInvoker.RegisterTypesMethods(t, CommandGroupFactory);
                }
            }
        }
    }
}
