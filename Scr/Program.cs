using System;
using System.Windows.Forms;
using Common;

namespace Scr
{
    static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // parse args
            var startupArgs = StartupArgsHandler.Parse(args,
                new StartupArgs[] { new RunArgs(), new ConfigureArgs(), new PreviewArgs() },
                new RunArgs());

            // run form
            Application.Run(new MainForm(startupArgs));
        }
    }
}
