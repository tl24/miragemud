using System;
using System.Collections.Generic;
using System.Text;
using Shoop.IO;

namespace Shoop
{
    class Program
    {
        static void Main(string[] args)
        {
            Shoop.Command.MethodInvoker.registerType(typeof(Shoop.Data.Player));
            Shoop.Command.MethodInvoker.registerType(typeof(Shoop.Command.Interpreter));
            Server listener = new Server(4500);
            listener.run();
        }
    }
}
