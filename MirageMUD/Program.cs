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
            Mirage.Command.MethodInvoker.registerType(typeof(Mirage.Data.Player));
            Mirage.Command.MethodInvoker.registerType(typeof(Mirage.Command.Interpreter));
            Mirage.Command.MethodInvoker.registerType(typeof(Mirage.Command.Movement));
            Server listener = new Server(4500);
            AreaLoader loader = new AreaLoader();
            loader.LoadAll();
            listener.Run();
        }
    }
}
