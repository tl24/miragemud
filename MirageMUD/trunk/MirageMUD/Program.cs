using System;
using log4net;
using Mirage.Game;
using Mirage.Game.World;

namespace Mirage
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            log4net.Config.XmlConfigurator.Configure();            
            //MirageServer listener = new MirageServer(4500);
            //listener.Run();
            try
            {
                MirageServer listener = MudFactory.GetObject<MirageServer>();
                listener.Run();
            }
            catch (Exception e)
            {
                ILog logger = LogManager.GetLogger("");
                logger.Error(e.Message, e);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ILog logger = LogManager.GetLogger("");
                logger.Error("Unhandled exception occurred", (Exception)e.ExceptionObject);
            }
            catch
            {
                Console.WriteLine("Unhandled exception occurred: " + e.ExceptionObject);
            }
        }
    }
}
