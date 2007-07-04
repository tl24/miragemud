using System;
using System.Collections.Generic;
using System.Text;
using Shoop.IO;
using Shoop.Data;

namespace Shoop
{
    class Program
    {
        static void Main(string[] args)
        {
            Shoop.Command.MethodInvoker.registerType(typeof(Shoop.Data.Player));
            Shoop.Command.MethodInvoker.registerType(typeof(Shoop.Command.Interpreter));
            Shoop.Command.MethodInvoker.registerType(typeof(Shoop.Command.Movement));
            Server listener = new Server(4500);
            AreaLoader loader = new AreaLoader();
            loader.LoadAll();
            listener.Run();
        }
    }
}
