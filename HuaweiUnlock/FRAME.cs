using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Collections.Concurrent;
using static HuaweiUnlocker.Utils.FlashTool;
using static HuaweiUnlocker.MISC;
using System.ComponentModel;
using System.Net;
using Ionic.Zip;
using static HuaweiUnlocker.CMD;
using HuaweiUnlocker.Utils;

namespace HuaweiUnlocker
{
    public partial class FRAME : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        private Dictionary<string, string> source = new Dictionary<string, string>();
        public FRAME()
        {
            InitializeComponent();
            LOGGBOX.Text = "Version 6.0 (C) MOONGAMER";
            LOG(I("SMAIN1"));
            LOG(I("SMAIN2"));
            LOG(I("SMAIN3"));
            LOG(I("Tutr"));

            LOG(I("MAIN1"));
            LOG(I("MAIN2"));
            LOG(I("MAIN3"));
            LOG(I("Tutr"));
            progr = PGG;
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
            if (!Directory.Exists(Path)) BoardU.Text = L("DdBtn"); else BoardU.Text = L("DdBtnE");
            Lang();
        }
        public void Lang()
        {
            ReadLngFile();
            button1.Text = button2.Text = button3.Text = L("SelBtn");
            AutoXml.Text = L("AutoLBL");
            AutoLdr.Text = L("AutoLBL");
            Flash.Text = L("FlBtn");
            DUMPALL.Text = L("DuBtn");
            GPfir.Text = L("SelPathToFGB");
            RdGPT.Text = L("RdGPTBtn");
            ReadPA.Text = L("ReadPA");
            WritePA.Text = L("WritePA");
            ErasePA.Text = L("ErasePA");
            EraseDA.Text = L("EraseDA");
            BoardU.Text = L("UnlockBTN");
            UnlockFrp.Text = L("UnlockBTN");
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = L("DdBtn"); else BoardU.Text = L("DdBtnE");
        }
        private void LOADER_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Ld.Text;
            if (Ld.Text == "") openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Programmer files (*.mbn;*.elf;*.hex)|*.mbn;*.elf;*.hex|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) Ld.Text = openFileDialog.FileName;
        }
        private void XML_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Xm.Text;
            if (Xm.Text == "") openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Sectors data files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) Xm.Text = openFileDialog.FileName;
        }

        private void Flash_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            progr.Value = 0;
            if (Xm.Text.Length < 5 && !RAW.Checked)
            {
                LOG(E("ErrXML"));
                return;
            }
            if (pather.Text.Length < 5 && RAW.Checked)
            {
                LOG(E("ErrBin"));
                return;
            }
            Tab.Enabled = !Tab.Enabled;
            if (!RAW.Checked)
            {
                if (!FlashPartsXml(Xm.Text, AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text, pather.Text))
                    LOG(E("ErrXML2"));
                else LOG(I("Flashing") + pather.Text + L("Done"));
            }
            else
            {
                if (!FlashPartsRaw(AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text, pather.Text))
                    LOG(E("ErrBin2"));
                else LOG(I("Flashing2") + pather.Text + L("Done"));
            }
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            Tab.Enabled = !Tab.Enabled;
            progr.Value = 100;
        }

        private void Xml(object sender, EventArgs e)
        {
            button2.Enabled = !AutoXml.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!RAW.Checked)
            {
                FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pather.Text = openFileDialog.SelectedPath;
                    if (AutoXml.Checked)
                    {
                        foreach (var a in Directory.GetFiles(openFileDialog.SelectedPath))
                        {
                            if (AutoXml.Checked && a.EndsWith(".xml")) Xm.Text = a;
                            if (AutoLdr.Checked && a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) Ld.Text = a;
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = Xm.Text;
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Sector DUMP files (*.img;*.bin)|*.img;*.bin;*.emmc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK) pather.Text = openFileDialog.FileName;
            }
        }

        private void RAW_CheckedChanged(object sender, EventArgs e)
        {
            Xm.Visible = !RAW.Checked;
            DETECTED.Visible = !RAW.Checked;
            AutoXml.Visible = !RAW.Checked;
            button2.Visible = !RAW.Checked;
            AutoXml.Enabled = !RAW.Checked;
            button2.Enabled = !RAW.Checked;
            DETECTED.Enabled = !RAW.Checked;
            Xm.Enabled = !RAW.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            progr.Value = 0;
            Tab.Enabled = !Tab.Enabled;
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.SelectedPath + "\\DUMP.APP";
                if (!Dump(AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text, pather.Text))
                {
                    LOG("ERROR: Failed Dump All!");
                    foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
                }
                else LOG(I("Dumping") + pather.Text + L("Done"));
            }
            Tab.Enabled = !Tab.Enabled;
            progr.Value = 100;
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = AutoLdr.Checked;
        }

        private void AutoXml_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = AutoXml.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            RdGPT.Visible = false;
            RdGPT.Enabled = false;
            LOG(I("ReadGPT"));
            GPTTABLE = new Dictionary<string, int[]>();
            bool gpt = ReadGPT(AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text);
            if (gpt)
            {
                foreach (var obj in GPTTABLE) PARTLIST.Rows.Add(obj.Key, obj.Value[0], obj.Value[1]);
                PARTLIST.AutoResizeRows();
                LOG(I("SUCC_ReadGPT"));
            }
            else { LOG(I("ERR_ReadGPT")); }
            RdGPT.Visible = !gpt;
            RdGPT.Enabled = !gpt;
            progr.Value = 100;
        }

        private void PARTLIST_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (PARTLIST.Rows.Count > 0)
            {
                PARTLIST.Enabled = false;
                WHAT.Enabled = true;
                WHAT.Visible = true;
                string partition = PARTLIST.CurrentRow.Cells[0].Value.ToString();
                LNG.Text = L("Action") + partition;
                WHAT.Text = partition;
                LOG(I("sl") + partition);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WHAT.Enabled = false;
            PARTLIST.Enabled = true;
            WHAT.Visible = false;
        }

        private void ERASEevent_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(L("AreY")+WHAT.Text, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Erase(WHAT.Text, AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text))
                    LOG(I("ErPS") + WHAT.Text);
                else
                    LOG(E("ErPE") + WHAT.Text);
                progr.Value = 100;
            }
        }

        private void WRITEevent_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (Write(WHAT.Text, AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text, file.FileName))
                    LOG(I("EwPS") + WHAT.Text);
                else
                    LOG(E("EwPE") + WHAT.Text);
                progr.Value = 100;
            }
        }

        private void READevent_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                int i = GPTTABLE[WHAT.Text][0];
                int j = GPTTABLE[WHAT.Text][1];
                if (Dump(i, j, WHAT.Text, AutoLdr.Checked? PickLoader(Ld.Text) : Ld.Text, folder.SelectedPath))
                    LOG(I("EdPS") + WHAT.Text);
                else
                    LOG(E("EdPE") + WHAT.Text);
                progr.Value = 100;
            }
        }

        private void DEVICER_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = L("DdBtn"); else BoardU.Text = L("DdBtnE");
        }

        private void ISAS2(object sender, EventArgs e)
        {
            if (AutoLdr.Checked)
            {
                foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                {
                    String[] es = a.Split('\\');
                    Ld.Items.Add(es[es.Length - 1]);
                }
            }
            else
            {
                Ld.Text = "";
                Ld.Items.Clear();
            }
        }
        private void UnZip(string zipFile, string folderPath)
        {
            ZipFile.Read(zipFile).ExtractAll(folderPath, ExtractExistingFileAction.OverwriteSilently);
        }
        private void client_finish(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LOG(E("FailCon") + Environment.NewLine + L("Error") + e.Error);
                return;
            }
            LOG(I("Downloaded") + DEVICER.Text.ToUpper() + ".zip");
            UnZip(DEVICER.Text.ToUpper() + ".zip", "UnlockFiles\\" + device);
            button1_Click(sender, e);
        }
        private void Erasda_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            progr.Value = 0;
            Tab.Enabled = !Tab.Enabled;
            LOG(I("CheckCon"));
            loader = PickLoader(DEVICER.Text.ToUpper().Split('-')[0]);
            LOG(I("EraserD"));
            if (!Erase("userdata", loader))
                LOG(E("FailUsrData"));

            else LOG(I("Success"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            Tab.Enabled = !Tab.Enabled;
            progr.Value = 100;
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = e.BytesReceived;
            double totalBytes = e.TotalBytesToReceive;
            double percentage = bytesIn / totalBytes * 100;
            progr.Value = (int)percentage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            if (!DEVICER.Text.Contains("-")) { LOG("INFO: " + I("SelDev")); return; }
            progr.Value = 1;
            if (!Directory.Exists(Path))
            {
                LOG(I("DownloadFor") + device);
                LOG("URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_finish);
                    client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                    button1.Text = L("UnlockBTN");
                    return;
                }
            }
            loader = PickLoader(device.Split('-')[0]);
            LOG(I("CheckCon"));
            Tab.Enabled = !Tab.Enabled;
            LOG(I("SendingCmd"));

            if (!Unlock(device, loader, Path))
                LOG(E("FailUnl"));
            else
                LOG(I("Success"));

            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            progr.Value = 100;
            Tab.Enabled = !Tab.Enabled;
        }

        private void UnlockFrp_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            progr.Value = 0;
            if (!DEVICER.Text.Contains("-")) { LOG(E("SelDev")); return; }
            device = DEVICER.Text.ToUpper();
            LOG(I("CheckCon"));
            loader = PickLoader(device.Split('-')[0]);
            Tab.Enabled = !Tab.Enabled;
            LOG(I("SendingCmd"));
            if (!UnlockFrp(loader))
                LOG(E("FailFrp"));

            else LOG(I("Success"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            Tab.Enabled = !Tab.Enabled;
            progr.Value = 100;
        }
        private bool Find()
        {
            Port_D data = GETPORT("android adapter pcui");
            LOG(I("Info") + "PCUI PORT: " + data.DeviceName);
            DIAG.PCUI = data.ComName;

            data = GETPORT("dbadapter reserved interface");
            DIAG.DBDA = data.ComName;
            LOG(I("Info") + "DBADAPTER PORT: " + data.DeviceName);

            return DIAG.DBDA != "NaN" && DIAG.PCUI != "NaN";
        }
        private void Read_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(I("TrDaI"));
            if (DIAG.DBDA != "") {
                IMEIbox.Text = DIAG.GET_IMEI1();

                string[] DATA = DIAG.GET_FIRMWAREINFO();
                SNbox.Text = DATA[0];
                BIDbox.Text = DATA[1];

                DATA = DIAG.GET_BOARDINFO();
                CHIPbox.Text = DATA[0];
                VERbox.Text = DATA[1];

                RSAka.Text = DIAG.GET_SECRET_KEY_CRYPTED();
            }
        }

        private void RB_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(I("TrRb") + "system");
            DIAG.REBOOT();
        }

        private void RecoveryBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(I("TrRb") + "UpgradeMode (Recovery 3 point)");
            DIAG.To_Three_Recovery();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG("TRYING TO AUTH PHONE FUCK THE HW: !!!BETA TEST NOT WORKING!!!");
            DIAG.AUTH();
        }

        private void BRTFRC_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG("TRYING TO AUTH PHONE FUCK THE HW");
            DIAG.BROOTFORCE_HW_CMD();
        }
    }
}
