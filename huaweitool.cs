using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.IO.Compression;
using Ionic.Zip;
namespace HuaweiUnlocker
{
    public partial class Huawei : Form
    {
        private List<string> notdevice = new List<string>();
        private List<string> current = new List<string>();
        private static string port = "NaN";
        private static string device;
        private static string loader;
        public static string Path;
        private static bool first = true;
        public static bool error = false;
        public static bool debug = false;
        public static bool loadedhose = false;
        private static TextBox LOGGE;
        public Huawei()
        {
            InitializeComponent();
            LOGGE = LOGGER;
            LOGGE.Text = "Version 1.0 beta";
            LOG("INFO: Huawei Unlock Tool v1");
            LOG("INFO: Author: moongamer");
            LOG("INFO: This tool uses Board bootloader!");
            REPEAT.Enabled = true;
            error = true;
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!File.Exists("log.txt")) File.Create("log.txt");

            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://igriastranomier.ucoz.ru/hwlock/devices.txt");
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null) DEVICER.Items.Add(line);
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) button1.Text = "Download Package And Unlock";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            error = false;
            debug = DEBUGER.Checked;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            button1.Enabled = false;
            REPEAT.Enabled = false;
            if (!Directory.Exists(Path))
            {
                LOG("Downloading Unlock Files for: " + device);
                WebClient client = new WebClient();
                client.DownloadFile("https://igriastranomier.ucoz.ru/hwlock/" + device + ".zip", device + ".zip");
                if (!File.Exists(device + ".zip"))
                {
                    error = true;
                    LOG("ERROR: UNLOCK FILES DOESN'T DOWNLOADED");
                }
                else UnZip(device + ".zip", "UnlockFiles\\" + device);
                button1.Text = "Unlock Device";
            }
            LOG("Checking connection...");
            if (!ISA.Checked)
                loader = Loader_text.Text;
            else
                loader = "loader.mbn";
            Loaders.Text = loader;
            if (!port.StartsWith("COM"))
            {
                error = true;
                LOG("ERROR: DEVICE NOT CONNECTED");
            }
            if (!error) LOG("Sending command...");
            if (!error)
                if (!dload.Unlock(port, device, loader, Path))
                {
                    error = true;
                    LOG("ERROR: Device failed to load loader or port occupied");
                }
                else LOG("INFO: SUCCESS");
            else
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }

            port = "NaN";
            button1.Enabled = true;
            PORTER.Text = "Connect Your Device";
            REPEAT.Enabled = true;
        }

        private void Search_PORT(object sender, EventArgs e)
        {
            current.Clear();
            foreach (var a in SerialPort.GetPortNames())
            {
                current.Add(a);
                if (first) notdevice.Add(a);
                if (!notdevice.Contains(a) && !first && port != a) { PORTER.Text = "Device port: " + a; port = a; LOG("Connected PORT: " + a); break; }
            }
            if (first) first = false;
            if (!current.Contains(port)) { if (port != "NaN") LOG("Disconnected PORT: " + port); port = "NaN"; PORTER.Text = "Connect Your Device"; loadedhose = false; }
            if (!Directory.Exists("UnlockFiles\\" + DEVICER.Text.ToUpper())) button1.Text = "Download Package And Unlock";
            else button1.Text = "Unlock Device";
        }
        internal static void LOG(string i)
        {
            LOGGE.Text = (LOGGE.Text + "/n" + i).Replace("/n", Environment.NewLine);

            StreamWriter se = File.AppendText("log.txt");
            se.WriteLine(i.Replace("/n", Environment.NewLine));
            se.Close();
        }
        private void UnZip(string zipFile, string folderPath)
        {
            ZipFile zip = ZipFile.Read(zipFile);
            zip.ExtractAll(folderPath, ExtractExistingFileAction.OverwriteSilently);
            File.Delete(zipFile);
        }
    }
}
