using System;
using System.Windows.Forms;
using System.IO;
using HuaweiUnlocker;
using System.Diagnostics;
namespace HuaweiUnlocker
{
    public partial class Huawei : Form
    {
        public Huawei()
        {
            InitializeComponent();
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            MISC.LOGGBOX = LOGGER;
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            MISC.ReadLngFile();
            DBB.Text = MISC.L("DebugLbl");
            MISC.Fl = new FRAME();
            MISC.Fl.Show();
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (!File.Exists("log.txt")) return;
            File.Copy("log.txt", "LOGS\\" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "=LOG.txt", true);
            File.Delete("log.txt");
        }

        private void debugs(object sender, EventArgs e)
        {
            MISC.debug = DBB.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(MISC.GETPORT("qdloader 9008").ComName == "NaN") MISC.loadedhose = false;
        }
    }
}
