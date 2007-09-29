using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using Mirage.Core.Data;

namespace Mirage
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();            
            Server listener = new Server(4500);
            listener.Run();
        }
    }
}
