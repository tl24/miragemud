using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.IO.Net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace SampleMud
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 4000;
            if (args.Length == 1)
            {
                int.TryParse(args[0], out port);
                if (port <= 0)
                    port = 4000;
            }

            SampleMudServer server = new SampleMudServer(
                new ConnectionManager(
                    new List<IConnectionListener> {
                        new TextConnectionListener(port)
                    }
                )
            );
            server.Initializers = new[] { new CommandsInitializer() };
            server.Run();            
        }
    }
}
