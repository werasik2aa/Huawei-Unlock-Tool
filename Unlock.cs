using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Ionic.Zip;
using System.IO;
using System.ComponentModel;
using static HuaweiUnlocker.tool;
namespace HuaweiUnlocker
{
    public partial class Unlock : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        private Dictionary<string, string> source = new Dictionary<string, string>();
        private Dictionary<string, string> lang = new Dictionary<string, string>();
        public Unlock()
        {
            InitializeComponent();
            //log
            LOGGBOX.Text = "Version 4.0 (C) MOONGAMER";
            LOG(I("MAIN1"));
            LOG(I("MAIN2"));
            LOG(I("MAIN3"));
            LOG(I("Tutr"));
            WebClient client = new WebClient();
            //DEVICE FROM WEB
            Stream stream = client.OpenRead("http://igriastranomier.ucoz.ru/hwlock/devices.txt");
            StreamReader readerD = new StreamReader(stream);
            string line = readerD.ReadLine();
            while ((line = readerD.ReadLine()) != null)
            {
                string[] a = line.Split(' ');
                DEVICER.Items.Add(a[0]);
                if (!source.ContainsKey(a[0])) source.Add(a[0], a[1]);
            }
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) button1.Text = "Download then Unlock"; else button1.Text = "Unlock";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            if (!DEVICER.Text.Contains("-")) { LOG("INFO: " + I("SelDev")); return; }
            progr.Value = 1;
            if (!Directory.Exists(Path))
            {
                LOG(I("DownloadFor") + device);
                LOG("URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_finish);
                    client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                    button1.Text = L("UnlockBTN");
                    return;
                }
            }
            if (ISA.Checked) loader = PickLoader(device.Split('-')[0]);
            else loader = PickLoader(Loaders.Text);
            Loaders.Text = loader;
            LOG(I("CheckCon"));
            if (!tool.CheckDevice(Loaders)) return;
            all();
            LOG(I("SendingCmd"));

            if (!Unlock(device, Loaders.Text, Path))
                LOG(E("FailUnl"));
            else 
                LOG(I("Success"));

            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            progr.Value = 100;
            port = "NaN";
            all();
        }

        private void SS(object sender, EventArgs e)
        {
            PORTFIND();
            if (!Directory.Exists("UnlockFiles\\" + DEVICER.Text.ToUpper())) button1.Text = L("DdBtn");
            else button1.Text = "Unlock Device";
        }

        private void UnZip(string zipFile, string folderPath)
        {
            ZipFile.Read(zipFile).ExtractAll(folderPath, ExtractExistingFileAction.OverwriteSilently);
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
            progr.Value = 0;
            if (!DEVICER.Text.Contains("-")) { LOG(E("SelDev")); return; }
            device = DEVICER.Text.ToUpper();
            LOG(I("CheckCon"));
            if (ISA.Checked) loader = PickLoader(device.Split('-')[0]);
            else loader = PickLoader(Loaders.Text);
            Loaders.Text = loader;
            if (!port.StartsWith("COM"))
            {
                LOG(E("DeviceNotCon"));
                return;
            }
            all();
            LOG(I("SendingCmd"));

            if (!UnlockFrp(Loaders.Text))
                LOG(E("FailFrp"));

            else LOG(I("Success"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            all();
            progr.Value = 100;
        }

        private void Erasda_Click(object sender, EventArgs e)
        {
            progr.Value = 0;
            all();
            device = DEVICER.Text.ToUpper();
            LOG(I("CheckCon"));
            if (ISA.Checked) loader = PickLoader(device.Split('-')[0]);
            else loader = PickLoader(Loaders.Text);
            Loaders.Text = loader;
            if (!port.StartsWith("COM"))
            {
                LOG(E("DeviceNotCon"));
                return;
            }
            LOG(I("SendingCmd"));

            if (!Erase("userdata", Loaders.Text))
                LOG(E("FailUsrData"));

            else LOG(I("Success"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            all();
            progr.Value = 100;
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = e.BytesReceived;
            double totalBytes = e.TotalBytesToReceive;
            double percentage = bytesIn / totalBytes * 100;
            progr.Value = (int)percentage;
        }
        void client_finish(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LOG(E("FailCon")+ Environment.NewLine + L("Error") + e.Error);
                return;
            }
            LOG(I("Downloaded") + DEVICER.Text.ToUpper() + ".zip");
            UnZip(DEVICER.Text.ToUpper() + ".zip", "UnlockFiles\\" + device);
            button1_Click(sender, e);
        }
        private void DEVICER_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) button1.Text = L("DdBtn"); else button1.Text = L("DdBtnE");
        }
    }
}
