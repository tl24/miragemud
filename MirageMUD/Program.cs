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
            Mirage.Core.Command.MethodInvoker.RegisterType(typeof(Mirage.Core.Data.Player));
            Mirage.Core.Command.MethodInvoker.RegisterType(typeof(Mirage.Core.Command.Interpreter));
            Mirage.Core.Command.MethodInvoker.RegisterType(typeof(Mirage.Core.Command.Movement));
            Mirage.Core.Command.MethodInvoker.RegisterType(typeof(Mirage.Core.Command.AreaBuilder));
            Server listener = new Server(4500);
            AreaLoader loader = new AreaLoader();
            loader.LoadAll();
            listener.Run();
        }
    }
}
