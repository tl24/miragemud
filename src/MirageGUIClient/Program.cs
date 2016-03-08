using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MirageGUI.Forms;

namespace MirageGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            log4net.Config.XmlConfigurator.Configure();
            Application.Run(new BuilderPane());
        }
    }
}