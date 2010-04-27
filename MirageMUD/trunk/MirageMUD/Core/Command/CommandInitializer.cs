using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using log4net;
using System.Collections.Specialized;
using Castle.Core;

namespace Mirage.Core.Command
{
    public class CommandInitializer : IInitializer, IStartable
    {
        public string Name
        {
            get { return this.GetType().Name; }
        }

        public void Execute()
        {
            LoadCommands();
        }

        private static void LoadCommands()
        {
            ILog logger = LogManager.GetLogger(typeof(CommandInitializer));
            NameValueCollection locations = (NameValueCollection)ConfigurationManager.GetSection("MirageMUD/CommandLocations");
            if (locations != null)
            {
                foreach (string key in locations)
                {
                    logger.Info("Searching " + locations[key] + " for commands");
                    MethodInvoker.RegisterType(Type.GetType(locations[key]));
                }
            }
        }

        #region IStartable Members

        public void Start()
        {
            LoadCommands();
        }

        public void Stop()
        {
        }

        #endregion
    }
}
