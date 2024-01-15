using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace HuaweiUnlocker
{
    static class Program
    {
        public static bool ISWINDOW = true;
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length != 1)
            {
                MessageBox.Show("This tool already runned! You can run only one app window", "[ERROR]", MessageBoxButtons.OK);
                Application.Exit();
            }
            else
            {
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) process.Kill();
                foreach (var process in Process.GetProcessesByName("fh_loader.exe")) process.Kill();
                Application.Run(new Window());
            }
        }
    }
}
