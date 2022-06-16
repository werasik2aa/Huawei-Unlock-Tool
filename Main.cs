using System;
using System.Windows.Forms;
using System.IO;
using HuaweiUnlocker;
namespace HuaweiUnlocker
{
    public partial class Huawei : Form
    {
        public Huawei()
        {
            InitializeComponent();
            if (!File.Exists("log.txt")) File.Create("log.txt");
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");
        }
        private void Huawei_Deactivate(object sender, EventArgs e)
        {
            File.Copy("log.txt", "LOGS\\" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "=LOG.txt", true);
        }

        private void gs_Click(object sender, EventArgs e)
        {
            FlashTool fl = new FlashTool();
            fl.Show();
        }

        private void Checker_Tick(object sender, EventArgs e)
        {
            tool.PORTFIND();
        }

        private void UnlockTool_Click(object sender, EventArgs e)
        {
            Unlock u = new Unlock();
            u.Show();
        }
    }
}
