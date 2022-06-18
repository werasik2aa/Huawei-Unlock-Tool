using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO.Compression;
using Ionic.Zip;
using System.IO;
namespace HuaweiUnlocker
{
    public partial class Unlock : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        public Unlock()
        {
            InitializeComponent();
            tool.LOGGE.Text = "Version 2.0 beta";
            tool.LOG("INFO: Huawei Unlock Tool v1");
            tool.LOG("INFO: Author: moongamer");
            tool.LOG("INFO: This tool uses Board bootloader!");
            tool.LOG("INFO: Connect device via EDL (9008 mode)");
            tool.error = true;

            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://igriastranomier.ucoz.ru/hwlock/devices.txt");
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null) DEVICER.Items.Add(line);
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tool.progr.Value = 0;
            tool.error = false;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            tool.all();
            tool.progr.Value = 1;
            if (!Directory.Exists(Path))
            {
                tool.LOG("INFO: Downloading Unlock Files for: " + device);
                WebClient client = new WebClient();
                client.DownloadFile("https://igriastranomier.ucoz.ru/hwlock/" + device + ".zip", device + ".zip");
                if (!File.Exists(device + ".zip"))
                {
                    tool.error = true;
                    tool.LOG("tool.error: UNLOCK FILES DOESN'T DOWNLOADED");
                }
                else UnZip(device + ".zip", "UnlockFiles\\" + device);
                button1.Text = "Unlock Device";
            }
            tool.LOG("INFO: Checking connection...");
            if (ISA.Checked) loader = tool.PickLoader(device.Split('-')[0]);
            else loader = tool.PickLoader(Loaders.Text);
            Loaders.Text = loader;
            if (!tool.port.StartsWith("COM"))
            {
                tool.error = true;
                tool.LOG("tool.error: DEVICE NOT CONNECTED");
            }
            if (!tool.error)
            {
                tool.LOG("INFO: Sending command...");
                if (!tool.Unlock(device, Loaders.Text, Path))
                {
                    tool.error = true;
                    tool.LOG("ERROR: FAILED TO UNLOCK BOOTLOADER");
                }
                else tool.LOG("INFO: SUCCESS");
            }
            else
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            tool.progr.Value = 100;
            tool.port = "NaN";
            tool.all();
        }

        private void SS(object sender, EventArgs e)
        {
            tool.PORTFIND();
            if (!Directory.Exists("UnlockFiles\\" + DEVICER.Text.ToUpper())) button1.Text = "Download Package And Unlock";
            else button1.Text = "Unlock Device";
        }
        private void UnZip(string zipFile, string folderPath)
        {
            ZipFile zip = ZipFile.Read(zipFile);
            zip.ExtractAll(folderPath, ExtractExistingFileAction.OverwriteSilently);
        }

        private void ISAS(object sender, EventArgs e)
        {
            if (!ISA.Checked)
            {
                foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                {
                    String[] es = a.Split('\\');
                    Loaders.Items.Add(es[es.Length - 1]);
                }
            }
            else
                Loaders.Items.Clear();
        }

        private void UnlockFrp_Click(object sender, EventArgs e)
        {
            tool.progr.Value = 0;
            tool.error = false;
            tool.all();
            device = DEVICER.Text.ToUpper();
            tool.LOG("INFO: Checking connection...");
            if (ISA.Checked) loader = tool.PickLoader(device.Split('-')[0]);
            else loader = tool.PickLoader(Loaders.Text);
            Loaders.Text = loader;
            if (!tool.port.StartsWith("COM"))
            {
                tool.error = true;
                tool.LOG("tool.error: DEVICE NOT CONNECTED");
            }
            if (!tool.error)
            {
                tool.LOG("INFO: Sending command...");
                if (!tool.UnlockFrp(Loaders.Text))
                {
                    tool.error = true;
                    tool.LOG("ERROR: FAILED TO UNLOCK FRP");
                }
                else tool.LOG("INFO: SUCCESS");
            }
            else
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            tool.all();
            tool.progr.Value = 100;
            tool.getgpt = false;
        }

        private void Erasda_Click(object sender, EventArgs e)
        {
            tool.progr.Value = 0;
            tool.error = false;
            tool.all();
            device = DEVICER.Text.ToUpper();
            tool.LOG("INFO: Checking connection...");
            if (ISA.Checked) loader = tool.PickLoader(device.Split('-')[0]);
            else loader = tool.PickLoader(Loaders.Text);
            Loaders.Text = loader;
            if (!tool.port.StartsWith("COM"))
            {
                tool.error = true;
                tool.LOG("tool.error: DEVICE NOT CONNECTED");
            }
            if (!tool.error)
            {
                tool.LOG("INFO: Sending command...");
                if (!tool.Erase("userdata", Loaders.Text))
                {
                    tool.error = true;
                    tool.LOG("ERROR: FAILED TO ERASE USERDATA");
                }
                else tool.LOG("INFO: SUCCESS");
            }
            else
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            tool.all();
            tool.progr.Value = 100;
        }
    }
}
