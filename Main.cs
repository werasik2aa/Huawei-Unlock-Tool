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
            tool.readlng();
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            tool.progr = Pg;
            tool.PORTER = PORTER;
            tool.LOGGE = LOGGER;
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (!File.Exists("log.txt")) return;
            File.Copy("log.txt", "LOGS\\" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "=LOG.txt", true);
            File.Delete("log.txt");
        }

        private void gs_Click(object sender, EventArgs e)
        {
            tool.Fl = new FlashTool();
            tool.Fl.Show();
        }

        private void Checker_Tick(object sender, EventArgs e)
        {
            tool.PORTFIND();
        }

        private void UnlockTool_Click(object sender, EventArgs e)
        {
            tool.Un = new Unlock();
            tool.Un.Show();
        }

        private void debugs(object sender, EventArgs e)
        {
            tool.debug = checkBox1.Checked;
        }

        private void msmdm_Click(object sender, EventArgs e)
        {
            tool.QXD = new QXDterminal();
            tool.QXD.Show();
        }
    }
}
