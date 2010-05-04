using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Castle.Core;
using Castle.Core.Logging;
using System.Reflection;
using System.Linq;
using Mirage.Core.Util;

namespace Mirage.Core.Command
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

        public void Execute()
        {
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
                    MethodInvoker.RegisterType(t);
                }
            }
        }
    }
}
