using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace ActivityLogger.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            KillOtherInstances();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        static void KillOtherInstances()
        {
            var currentProcess = Process.GetCurrentProcess();
            var instances = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
            var warn = false;
            foreach (var instance in instances)
            {
                if (instance.Id != currentProcess.Id)
                {
                    try
                    {
                        instance.Kill();
                    }
                    catch (Exception)
                    {
                        warn = true;
                    }
                }
            }

            if (warn)
            {
                MessageBox.Show("Other Activity Loggers are already running.");
            }
        }
    }
}
