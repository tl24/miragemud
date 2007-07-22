using System;
using System.Collections.Generic;
using System.Text;
using Mirage.IO;
using Mirage.Data;

namespace Mirage
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            Mirage.Command.MethodInvoker.RegisterType(typeof(Mirage.Data.Player));
            Mirage.Command.MethodInvoker.RegisterType(typeof(Mirage.Command.Interpreter));
            Mirage.Command.MethodInvoker.RegisterType(typeof(Mirage.Command.Movement));
            Mirage.Command.MethodInvoker.RegisterType(typeof(Mirage.Command.AreaBuilder));
            Server listener = new Server(4500);
            AreaLoader loader = new AreaLoader();
            loader.LoadAll();
            listener.Run();
        }
    }
}
