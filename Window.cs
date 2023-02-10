using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using static HuaweiUnlocker.FlashTool.FlashToolQClegacy;
using static HuaweiUnlocker.LangProc;
using System.ComponentModel;
using System.Net;
using Ionic.Zip;
using HuaweiUnlocker.DIAGNOS;
using System.Linq;
using HuaweiUnlocker.TOOLS;
using System.Security.Cryptography;

namespace HuaweiUnlocker
{

    public partial class Window : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        public static DIAG diag = new DIAG();
        private Dictionary<string, string> source = new Dictionary<string, string>();
        private Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\4PDA_HUAWEI_UNLOCK", true);
        private static string tempsel;
        public Window()
        {
            InitializeComponent();

            foreach (string i in Directory.GetFiles("Languages", "*.ini"))
            {
                var j = i.Split('\\').Last().Replace(".ini", "");
                if (!LBOX.Items.Contains(j)) LBOX.Items.Add(j);
            }

            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\4PDA_HUAWEI_UNLOCK");
                key.SetValue("LANGUAGE", "English");
                key.SetValue("DEBUG", true);
                LBOX.SelectedItem = "English";
            }

            LBOX.SelectedItem = CURRENTlanguage = key.GetValue("LANGUAGE").ToString();
            DBB.Checked = debug = bool.Parse(key.GetValue("DEBUG").ToString());

            LOGGBOX = LOGGER;
            progr = PGG;
            Lang();

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");

            //DEVICE LIST FROM WEB
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://igriastranomier.ucoz.ru/hwlock/devices.txt");
                StreamReader readerD = new StreamReader(stream);
                string line = readerD.ReadLine();
                while ((line = readerD.ReadLine()) != null)
                {
                    string[] a = line.Split(' ');
                    DEVICER.Items.Add(a[0]);
                    if (!source.ContainsKey(a[0])) source.Add(a[0], a[1]);
                }
            }
            catch { LOG("Connect to WEB SERVER !ERROR!"); }

            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = L("DdBtn"); else BoardU.Text = L("DdBtnE");
            foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
            {
                String[] es = a.Split('\\');
                LoaderBox.Items.Add(es[es.Length - 1]);
            }
            LangProc.Tab = Tab;

        }
        public void Lang()
        {
            ReadLngFile();
            SelectLOADER.Text = Selecty2.Text = Selecty3.Text = L("SelBtn");
            AutoXml.Text = AutoLdr.Text = L("AutoLBL"); ;
            Flash.Text = L("FlBtn");
            DUMPALL.Text = L("DuBtn");
            GPfir.Text = L("SelPathToFGB");
            RdGPT.Text = L("RdGPTBtn");
            ReadPA.Text = L("ReadPA");
            WritePA.Text = L("WritePA");
            ErasePA.Text = L("ErasePA");
            EraseDA.Text = L("EraseDA");
            UnlockFrp.Text = L("UnlockBTN");
            HomeTag.Text = L("HomeTag");
            BackupRestoreTag.Text = L("BackupRestoreTag");
            UnlockTag.Text = L("UnlockTag");
            GPTtag.Text = L("GPTtag");
            HISItag.Text = L("HISItag");
            GLOADER.Text = L("LoaderHeader");
            PTOFIRM.Text = L("PathToFirmLBL");
            RAW.Text = L("RWIMGlbl");
            SLDEV.Text = L("SELDEVlbl");

            Tab.TabPages[0].Text = L("HomeTag");
            Tab.TabPages[1].Text = L("BackupRestoreTagSimpl");
            Tab.TabPages[2].Text = L("UnlockSimpl");
            Tab.TabPages[3].Text = L("GPTtagSimpl");

            PARTLIST.Columns[0].HeaderText = L("NameTABLE0");
            PARTLIST.Columns[1].HeaderText = L("NameTABLE1");
            PARTLIST.Columns[2].HeaderText = L("NameTABLE2");

            TUTR2.Text = L("Tutr2");
            ACTBOX.Text = L("Action");

            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = L("DdBtn"); else BoardU.Text = L("DdBtnE");
            DBB.Text = LangProc.L("DebugLbl");
            LOGGBOX.Text = "Version 10.0 (C) MOONGAMER - 4PDA";
            LOG(I("SMAIN1"));
            LOG(I("SMAIN2"));
            LOG(I("SMAIN3"));
            LOG(I("Tutr"));

            LOG(I("MAIN1"));
            LOG(I("MAIN2"));
            LOG(I("MAIN3"));
            LOG(I("Tutr"));
        }
        private void LOADER_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = LoaderBox.Text;
            if (LoaderBox.Text == "") openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Programmer files (*.mbn;*.elf;*.hex)|*.mbn;*.elf;*.hex|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) LoaderBox.Text = openFileDialog.FileName;
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
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
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
            if (!RAW.Checked)
            {
                if (!FlashPartsXml(Xm.Text, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG(E("ErrXML2"));
                else
                    LOG(I("Flashing") + pather.Text);
            }
            else
            {
                if (!FlashPartsRaw(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG(E("ErrBin2"));
                else
                    LOG(I("Flashing2") + pather.Text);
            }

            progr.Value = 100;
        }

        private void PATHTOFIRMWARE_Clck(object sender, EventArgs e)
        {
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
                            if (AutoLdr.Checked && a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) LoaderBox.Text = a;
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

        private void DumpALL_CLK(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;

            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.SelectedPath + "\\DUMP.APP";
                if (!Dump(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG("ERROR: Failed Dump All!");
                else
                    LOG(I("Dumping") + pather.Text);
            }

            progr.Value = 100;
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            SelectLOADER.Enabled = AutoLdr.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            LOG(I("ReadGPT"));
            GPTTABLE = new Dictionary<string, int[]>();
            bool gpt = ReadGPT(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text);
            if (gpt)
            {
                foreach (var obj in GPTTABLE) PARTLIST.Rows.Add(obj.Key, obj.Value[0], obj.Value[1]);
                PARTLIST.AutoResizeRows();
                LOG(I("SUCC_ReadGPT"));
                RdGPT.Visible = false;
                RdGPT.Enabled = false;
                progr.Value = 100;
                return;
            }
            else
                LOG(I("ERR_ReadGPT"));
            RdGPT.Visible = true;
            RdGPT.Enabled = true;
        }

        private void PARTLIST_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (PARTLIST.Rows.Count > 0)
            {
                PARTLIST.Enabled = false;
                WHAT.Enabled = true;
                WHAT.Visible = true;
                string partition = PARTLIST.CurrentRow.Cells[0].Value.ToString();
                WHAT.Text = L("Action") + partition;
                tempsel = partition;
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
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(L("AreY") + tempsel, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Erase(tempsel, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(I("ErPS") + tempsel);
                else
                    LOG(E("ErPE") + tempsel);
                progr.Value = 100;
            }
        }

        private void WRITEevent_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (Write(tempsel, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, file.FileName))
                    LOG(I("EwPS") + tempsel + newline);
                else
                    LOG(E("EwPE") + tempsel);
                progr.Value = 100;
            }
        }

        private void READevent_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                int i = GPTTABLE[tempsel][0];
                int j = GPTTABLE[tempsel][1];
                if (Dump(i, j, tempsel, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, folder.SelectedPath))
                    LOG(I("EdPS") + tempsel + newline);
                else
                    LOG(E("EdPE") + tempsel);
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
                    LoaderBox.Items.Add(es[es.Length - 1]);
                }
                LoaderBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else
            {
                LoaderBox.Text = "";
                LoaderBox.Items.Clear();
                LoaderBox.DropDownStyle = ComboBoxStyle.DropDown;
            }
            SelectLOADER.Enabled = !AutoLdr.Checked;
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
            UNLBTN_Click(sender, e);
        }
        private void Erasda_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;

            LOG(I("CheckCon"));
            loader = PickLoader(DEVICER.Text.ToUpper().Split('-')[0]);
            LOG(I("EraserD"));
            if (!Erase("userdata", loader))
                LOG(E("FailUsrData"));

            else LOG(I("Success"));

            progr.Value = 100;
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = e.BytesReceived;
            double totalBytes = e.TotalBytesToReceive;
            double percentage = bytesIn / totalBytes * 100;
            progr.Value = (int)percentage;
        }
        private void UNLBTN_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            LOG(I("CheckCon"));
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            if (!DEVICER.Text.Contains("-")) { LOG("INFO: " + I("SelDev")); return; }
            loader = AutoLdr.Checked ? PickLoader(DEVICER.Text.Split('-')[0]) : LoaderBox.Text;

            if (!Directory.Exists(Path))
            {
                progr.Value = 1;
                LOG(I("DownloadFor") + device);
                LOG("URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_finish);
                client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                BoardU.Text = L("DdBtnE");
                return;
            }

            if (!CheckDevice(loader)) return;

            LOG(I("SendingCmd"));
            if (!Unlock(device, loader, Path))
                LOG(E("FailUnl"));
            else
                LOG(I("PrcsUnl"));

            progr.Value = 100;

        }

        private void UnlockFrp_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            LOG(I("CheckCon"));
            loader = PickLoader(device.Split('-')[0]);

            LOG(I("SendingCmd"));
            if (!UnlockFrp(loader))
                LOG(E("FailFrp"));
            else
                LOG(I("Success"));

            progr.Value = 100;
        }
        private bool Find()
        {
            Port_D data = GETPORT("android adapter pcui");
            if (diag.PCUI != data.ComName)
            {
                diag.PCUI = data.ComName;
                LOG(data.ComName != "NaN" ? L("Info") + "PCUI PORT: " + data.DeviceName : L("Error") + "PCUI PORT not found");
            }

            data = GETPORT("dbadapter reserved interface");
            if (diag.DBDA != data.ComName)
            {
                diag.DBDA = data.ComName;
                LOG(data.ComName != "NaN" ? L("Info") + "DBADAPTER PORT: " + data.DeviceName : L("Error") + "DBADAPTER PORT not found");
            }

            return diag.DBDA != "NaN" && diag.PCUI != "NaN";
        }
        private void ReadINFOdiag_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Find()) return;
                LOG(I("TrDaI"));
                if (diag.DBDA != "")
                {
                    IMEIbox.Text = diag.GET_IMEI1();

                    string[] DATA = diag.GET_FIRMWAREINFO();
                    SNbox.Text = DATA[0];
                    BIDbox.Text = DATA[1];

                    DATA = diag.GET_BOARDINFO();
                    CHIPbox.Text = DATA[0];
                    VERbox.Text = DATA[1];
                }
            }
            catch { }
        }

        private void RB_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(I("TrRb") + "system");
            diag.REBOOT();
        }

        private void RecoveryBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(I("TrRb") + "UpgradeMode (Recovery 3 point)");
            OpenFileDialog f = new OpenFileDialog();

            if (f.ShowDialog() == DialogResult.OK)
                diag.To_Three_Recovery(f.FileName, CPUbox.Text);
            else
                diag.To_Three_Recovery("", "");
        }

        private void TryAUTH_CLCK(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG("TRYING TO AUTH PHONE FUCK THE HW: !!!BETA TEST NOT WORKING!!!");

        }
        private void FlashF_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            byte[] msg = diag.DIAG_SEND(CMD.Text, "", 0, true, true);
            CMD.Text = CRC.HexDump(msg) + Environment.NewLine + (CMDS.GetStatus(msg) ? "ACTION: SUCCESSFULL" : "ACTION: UNKNOWN CMD OR ACCESS DANIED");
        }
        private void BURGBTN_Click(object sender, EventArgs e)
        {
            if (BURG.MaximumSize.Width > BURG.Size.Width)
                while (BURG.MaximumSize.Width > BURG.Size.Width)
                {
                    BURG.Width += 1;
                    BURGBTN.Width += 1;
                }
            else if (BURG.MinimumSize.Width < BURG.Size.Width)
                while (BURG.MinimumSize.Width < BURG.Size.Width)
                {
                    BURG.Width -= 3;
                    BURGBTN.Width -= 3;
                }
        }

        private void RAW_CheckedChanged(object sender, EventArgs e)
        {
            Selecty2.Visible = !RAW.Checked;
            AutoXml.Enabled = !RAW.Checked;
            Selecty2.Enabled = !RAW.Checked;
            DETECTED.Enabled = !RAW.Checked;
            Xm.Enabled = !RAW.Checked;
        }

        private void AutoXml_CheckedChanged(object sender, EventArgs e)
        {
            Selecty2.Enabled = !AutoXml.Checked;
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (!File.Exists("log.txt")) return;
            File.Copy("log.txt", "LOGS\\" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "=LOG.txt", true);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(0);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(4);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(5);
        }

        private void DebugE_ch(object sender, EventArgs e)
        {
            debug = DBB.Checked;
        }

        private void CLEARDD_Click(object sender, EventArgs e)
        {
            CMD.Text = "";
        }

        private void SelLanguage_Click(object sender, EventArgs e)
        {
            CURRENTlanguage = LBOX.Text;
            Lang();
            key.SetValue("LANGUAGE", LBOX.Text);
            key.SetValue("DEBUG", DBB.Checked);
        }

        private void Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadedhose)
            {
                RdGPT.Visible = true;
                RdGPT.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void DBB_CheckedChanged(object sender, EventArgs e)
        {
            LangProc.debug = DBB.Checked;
        }

        private void FrBTN_Click(object sender, EventArgs e)
        {
            diag.FACTORY_RESET();
        }

        private void AUthBtn_Click(object sender, EventArgs e)
        {
            USBhack.TRY();
            
        }

        private void EraseDA_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008");
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(L("AreY") + tempsel, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Erase("userdata", AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(I("ErPS") + tempsel);
                else
                    LOG(E("ErPE") + tempsel);
                progr.Value = 100;
            }
        }
    }
}
