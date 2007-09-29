using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using log4net;
using System.Collections.Specialized;

namespace Mirage.Core.Command
{
    public class CommandInitializer : IInitializer
    {
        public void Execute()
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
    }
}
