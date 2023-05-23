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
using HuaweiUnlocker.FlashTool;
using HuaweiUnlocker.TOOLS;
using HuaweiUnlocker.UI;
namespace HuaweiUnlocker
{

    public partial class Window : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        public static DIAG diag = new DIAG();
        public static HISI HISI = new HISI();
        private Dictionary<string, string> source = new Dictionary<string, string>();
        private Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\4PDA_HUAWEI_UNLOCK", true);
        private static string tempsel;
        private static string TempDownload;
        public Window()
        {
            InitializeComponent();
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");

            foreach (string i in Directory.GetFiles("Languages", "*.ini"))
                LBOX.Items.Add(i.Split('\\').Last().Replace(".ini", ""));

            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\4PDA_HUAWEI_UNLOCK");
                key.SetValue("LANGUAGE", "English");
                key.SetValue("DEBUG", true);
                LBOX.SelectedItem = "English";
            }

            LBOX.SelectedItem = Language.CURRENTlanguage = key.GetValue("LANGUAGE").ToString();
            DBB.Checked = debug = bool.Parse(key.GetValue("DEBUG").ToString());

            LOGGBOX = LOGGER;
            progr = PGG;

            Lang();
            //DEVICE LIST FROM WEB
            try
            {
                WebClient client = new WebClient();
                StreamReader readerD = new StreamReader(client.OpenRead("http://igriastranomier.ucoz.ru/hwlock/devices.txt"));
                string line = readerD.ReadLine();
                while ((line = readerD.ReadLine()) != null)
                {
                    if (!line.StartsWith("[") && !String.IsNullOrEmpty(line) && !line.StartsWith("//") && !line.StartsWith("#"))
                    {
                        string[] a = line.Split(' ');
                        if (!a[0].StartsWith("KIRIN"))
                            DEVICER.Items.Add(a[0]);
                        else
                            HISIbootloaders.Items.Add(a[0]);
                        source.Add(a[0], a[1]);
                    }
                }
                client.Dispose();
            }
            catch
            {
                if (Directory.Exists("Languages"))
                    LOG(2, "WebCon");
                else
                    throw new Exception("NO LANGUAGE FILE!!!");
            }
            foreach (var a in Directory.GetDirectories("UnlockFiles"))
            {
                string folderDEV = a.Split('\\').Last();
                if (!folderDEV.StartsWith("KIRIN"))
                    if (!DEVICER.Items.Contains(folderDEV))
                        DEVICER.Items.Add(folderDEV);
                    else
                    if (!HISIbootloaders.Items.Contains(folderDEV))
                        HISIbootloaders.Items.Add(folderDEV);
            }
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
            foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                LoaderBox.Items.Add(a.Split('\\').Last());
            LangProc.Tab = Tab;
        }
        public void Lang()
        {
            if (Language.ReadLngFile())
            {
                //QUALCOMM AND BASIC
                SelPth.Text = SelectLOADER.Text = Selecty2.Text = Selecty3.Text = Language.Get("SelBtn");
                EraseMeBtn.Text = Language.Get("ErasePM");
                AutoCom.Text = AutoXml.Text = AutoLdr.Text = Language.Get("AutoLBL"); ;
                Flash.Text = Language.Get("FlBtn");
                DUMPALL.Text = Language.Get("DuBtn");
                GPfir.Text = Language.Get("SelPathToFGB");
                RdGPT.Text = Language.Get("RdGPTBtn");
                ReadPA.Text = Language.Get("ReadPA");
                WritePA.Text = Language.Get("WritePA");
                ErasePA.Text = Language.Get("ErasePA");
                EraseDA.Text = Language.Get("EraseDA");
                UnlockFrp.Text = Language.Get("UnlockBTN");
                HomeTag.Text = Language.Get("HomeTag");
                BackupRestoreTag.Text = Language.Get("BackupRestoreTag");
                UnlockTag.Text = Language.Get("UnlockTag");
                GPTtag.Text = Language.Get("GPTtag");
                HISItag.Text = Language.Get("UnlockTagHISI");
                GLOADER.Text = Language.Get("LoaderHeader");
                PTOFIRM.Text = Language.Get("PathToFirmLBL");
                RAW.Text = Language.Get("RWIMGlbl");
                SLDEV.Text = Language.Get("SELDEVlbl");
                UpdatePortList.Text = Language.Get("UpdPortListBtn");

                Tab.TabPages[0].Text = Language.Get("HomeTag");
                Tab.TabPages[1].Text = Language.Get("BackupRestoreTagSimpl");
                Tab.TabPages[2].Text = Language.Get("UnlockSimpl");
                Tab.TabPages[3].Text = Language.Get("GPTtagSimpl");
                Tab.TabPages[4].Text = Language.Get("DiagTagSimpl");
                Tab.TabPages[5].Text = Language.Get("UnlockSimplHISI");

                PARTLIST.Columns[0].HeaderText = Language.Get("NameTABLE0");
                PARTLIST.Columns[1].HeaderText = Language.Get("NameTABLE1");
                PARTLIST.Columns[2].HeaderText = Language.Get("NameTABLE2");

                groupBox2.Text = DevInfoQCBox.Text = Language.Get("DeviceInfoTag");
                TUTR2.Text = Language.Get("Tutr2");
                ACTBOX.Text = Language.Get("Action");
                RDinf.Text = Language.Get("DiagTagRead");
                UpgradMDbtn.Text = Language.Get("DiagTagUpgradeMode");
                ReBbtn.Text = Language.Get("DiagTagReboot");
                FrBTN.Text = Language.Get("DiagTagFactoryReset");
                //HISI TEXT
                CpuHISIBox.Text = Language.Get("HISISelectCpu");
                RdHISIinfo.Text = Language.Get("HISIReadFB");
                HISI_board_FB.Text = Language.Get("HISIWriteKirinFB");
                UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD");
                FBLstHISI.Text = Language.Get("HISIWriteKirinFBL");

                Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
                if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
                DBB.Text = Language.Get("DebugLbl");
                LOGGBOX.Text = "Version 12.1F BETA/n(C) MOONGAMER (QUALCOMM UNLOCKER)/n(C) MASHED-POTATOES (KIRIN UNLOCKER)".Replace("/n", Environment.NewLine);
                LOG(0, "SMAIN1");
                LOG(0, "SMAIN2");
                LOG(0, "SMAIN3");
                LOG(0, "MAIN1");
                LOG(0, "MAIN2");
                LOG(0, "MAIN3");
                LOG(0, "Tutr");
            }
        }
        private void LOADER_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = PrevFolder;
            openFileDialog.Filter = "Programmer files (*.mbn;*.elf;*.hex)|*.mbn;*.elf;*.hex|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = LoaderBox.Text = openFileDialog.FileName;
        }
        private void XML_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = PrevFolder;
            openFileDialog.Filter = "Sectors data files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = Xm.Text = openFileDialog.FileName;
        }

        private void Flash_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (String.IsNullOrEmpty(pather.Text) || !Directory.Exists(pather.Text) && !File.Exists(pather.Text))
            {
                LOG(2, "NoFirmPath");
                return;
            }
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;
            if (Xm.Text.Length < 5 && !RAW.Checked)
            {
                LOG(2, "ErrXML");
                return;
            }
            if (pather.Text.Length < 5 && RAW.Checked)
            {
                LOG(2, "ErrBin");
                return;
            }
            if (!RAW.Checked)
            {
                if (!FlashPartsXml(Xm.Text, PatXm.Text, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG(2, "EemmcXML_WPE");
                else
                    LOG(0, "EemmcXML_WPS", pather.Text);
            }
            else
            {
                if (!FlashPartsRaw(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG(2, "ErrBin2");
                else
                    LOG(0, "EemmcWPS", pather.Text);
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
                            if (AutoXml.Checked && a.EndsWith(".xml"))
                            {
                                if (a.Contains("rawprogram"))
                                    Xm.Text = a;
                                if (a.Contains("patch"))
                                    PatXm.Text = a;
                            }
                            if (AutoLdr.Checked && a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) LoaderBox.Text = a;
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = PrevFolder;
                openFileDialog.Filter = "Sector DUMP files (*.img;*.bin)|*.img;*.bin;*.emmc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = pather.Text = openFileDialog.FileName;
            }
        }

        private void DumpALL_CLK(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;

            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.SelectedPath + "\\DUMP.APP";
                if (!Dump(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text))
                    LOG(2, "Failed Dump All!");
                else
                    LOG(0, "Dumping", pather.Text);
            }

            progr.Value = 100;
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            SelectLOADER.Enabled = AutoLdr.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            LOG(0, "ReadGPT");
            GPTTABLE = new Dictionary<string, int[]>();
            bool gpt = ReadGPT(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text);
            if (gpt)
            {
                foreach (var obj in GPTTABLE) PARTLIST.Rows.Add(obj.Key, obj.Value[0], obj.Value[1]);
                PARTLIST.AutoResizeRows();
                LOG(0, "SUCC_ReadGPT");
                RdGPT.Enabled = RdGPT.Visible = false;
                progr.Value = 100;
                return;
            }
            else
                LOG(2, "ERR_ReadGPT");
            RdGPT.Enabled = RdGPT.Visible = true;
        }

        private void PARTLIST_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (PARTLIST.Rows.Count > 0)
            {
                PARTLIST.Enabled = false;
                WHAT.Enabled = true;
                WHAT.Visible = true;
                string partition = PARTLIST.CurrentRow.Cells[0].Value.ToString();
                WHAT.Text = Language.Get("Action") + partition;
                tempsel = partition;
                LOG(0, "sl", partition);
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
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Language.Get("AreY") + tempsel, Language.Get("CZdmg2"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Erase(tempsel, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(1, "ErPS", tempsel);
                progr.Value = 100;
            }
        }

        private void WRITEevent_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (Write(tempsel, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, file.FileName))
                    LOG(0, "EwPS", tempsel + newline);
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
                    LOG(0, "EdPS", tempsel + newline);
                progr.Value = 100;
            }
        }

        private void DEVICER_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
        }

        private void ISAS2(object sender, EventArgs e)
        {
            if (AutoLdr.Checked)
            {
                foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                    LoaderBox.Items.Add(a.Split('\\').Last());
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
        private void Downloaded(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LOG(2, "FailCon", newline + Language.Get("Error") + e.Error);
                return;
            }
            LOG(0, "Downloaded", TempDownload + ".zip");
            UnZip(TempDownload + ".zip", "UnlockFiles\\" + TempDownload);
            UNLBTN_Click(sender, e);
        }
        private void ProgressBar(object sender, DownloadProgressChangedEventArgs e)
        {
            double percentage = e.BytesReceived / e.TotalBytesToReceive * 100;
            progr.Value = (int)percentage;
        }
        private void UNLBTN_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            if (!DEVICER.Text.Contains("-")) { LOG(0, "SelDev"); return; }
            if (!Directory.Exists(Path))
            {
                progr.Value = 1;
                LOG(0, "DownloadFor" + device);
                LOG(0, "URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressBar);
                TempDownload = DEVICER.Text.ToUpper();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);
                client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                BoardU.Text = Language.Get("DdBtnE");
                Tab.Enabled = false;
                return;
            }
            loader = AutoLdr.Checked ? PickLoader(DEVICER.Text.Split('-').First()) : LoaderBox.Text;
            if (!CheckDevice(loader)) { Tab.Enabled = true; return; }
            if (!Unlock(device, loader, Path))
                LOG(2, "FailUnl");
            else
                LOG(0, "PrcsUnl", newline);

            progr.Value = 100;
        }

        private void UnlockFrp_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            progr.Value = 0;
            device = DEVICER.Text.ToUpper();
            LOG(0, "CheckCon");
            loader = PickLoader(device.Split('-')[0]);

            LOG(0, "SendingCmd");
            if (!UnlockFrp(loader))
                LOG(2, "FailFrp");
            else
                LOG(0, "Success");

            progr.Value = 100;
        }
        private bool Find()
        {
            Port_D data = GETPORT("android adapter pcui", null, true);
            if (diag.PCUI != data.ComName)
            {
                diag.PCUI = data.ComName;
                LOG(-1, data.ComName != "NaN" ? Language.Get("Info") + "PCUI PORT: " + data.DeviceName : Language.Get("Error") + "PCUI PORT not found");
            }

            data = GETPORT("dbadapter reserved interface", null, true);
            if (diag.DBDA != data.ComName)
            {
                diag.DBDA = data.ComName;
                LOG(-1, data.ComName != "NaN" ? Language.Get("Info") + "DBADAPTER PORT: " + data.DeviceName : Language.Get("Error") + "DBADAPTER PORT not found");
            }

            return diag.DBDA != "NaN" && diag.PCUI != "NaN";
        }
        private void ReadINFOdiag_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Find()) return;
                diag.HACKdbPort();
                LOG(-1, "TrDaI");
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
            LOG(0, "TrRb", "system");
            diag.REBOOT();
        }

        private void RecoveryBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(0, "TrRb", "UpgradeMode (Recovery 3 point)");
            OpenFileDialog f = new OpenFileDialog();

            if (f.ShowDialog() == DialogResult.OK)
                diag.To_Three_Recovery(f.FileName, CPUbox.Text);
            else
                diag.To_Three_Recovery("", "");
        }

        private void TryAUTH_CLCK(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(-1, "TRYING TO AUTH PHONE FUCK THE HW: !!!BETA TEST NOT WORKING!!!");

        }
        private void FlashF_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            byte[] msg = diag.DIAG_SEND(CMDbox.Text.Replace(" ", ""), "", 0, true, true);
            CMDbox.Text = CRC.HexDump(msg) + Environment.NewLine + CMDS.GetStatusStr(msg);
        }
        private void BURGBTN_Click(object sender, EventArgs e)
        {
            if (BURG.MaximumSize.Width > BURG.Size.Width)
                while (BURG.MaximumSize.Width > BURG.Size.Width)
                    BURGBTN.Width = BURG.Width += 1;
            else if (BURG.MinimumSize.Width < BURG.Size.Width)
                while (BURG.MinimumSize.Width < BURG.Size.Width)
                    BURGBTN.Width = BURG.Width -= 3;
        }

        private void RAW_CheckedChanged(object sender, EventArgs e)
        {
            PatXm.Enabled = SelPth.Enabled = SelPth.Visible = Xm.Enabled = DETECTED.Enabled = AutoXml.Enabled = Selecty2.Enabled = Selecty2.Visible = !RAW.Checked;
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
            CMDbox.Text = "";
        }

        private void SelLanguage_Click(object sender, EventArgs e)
        {
            Language.CURRENTlanguage = LBOX.Text;
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

        private void DBB_CheckedChanged(object sender, EventArgs e)
        {
            debug = DBB.Checked;
        }

        private void FrBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            diag.HACKdbPort();
            diag.FACTORY_RESET();
        }

        private void EraseDA_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Language.Get("AreY") + tempsel, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (Erase("userdata", AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(0, "ErPS", tempsel);
                else
                    LOG(2, "ErPE", tempsel);
                progr.Value = 100;
            }
        }

        private void RdHISIinfo_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            try
            {
                LOG(-1, "=============READ INFO (FASTBOOT)=============");
                if (HISI.ReadInfo(false))
                {
                    BuildIdTxt.Text = HISI.AVER;
                    ModelIdTxt.Text = HISI.MODEL;
                    VersionIdTxt.Text = HISI.BNUM;
                    FblockStateTxt.Text = HISI.FBLOCKSTATE;
                    BLKEYTXT.Text = HISI.BLKEY;
                    VersionIdTxt.Text = HISI.AVER;
                }
                else LOG(2, "DeviceNotCon");
            }
            catch (Exception esd)
            {
                LOG(2, esd.Message);
            }
            Tab.Enabled = true;
        }
        private void FBLstHISI_Click(object sender, EventArgs e)
        {
            LOG(-1, "=============WRITE FBLOCK (FASTBOOT)=============");
            LOG(-1, "=============> VALUE: " + (EnDisFBLOCK.Checked ? 1 : 0) + " <=============");
            try
            {
                if (HISI.ReadInfo(false))
                {
                    BuildIdTxt.Text = HISI.AVER;
                    ModelIdTxt.Text = HISI.MODEL;
                    VersionIdTxt.Text = HISI.BNUM;
                    FblockStateTxt.Text = HISI.FBLOCKSTATE;
                    BLKEYTXT.Text = HISI.BLKEY;
                    VersionIdTxt.Text = HISI.AVER;
                    HISI.SetFBLOCK(EnDisFBLOCK.Checked ? 1 : 0);
                }
            }
            catch (Exception se)
            {
                if (debug)
                    LOG(-1, "ERR: " + se);
            }
        }

        private void HISI_board_FB_Click(object sender, EventArgs e)
        {
            try
            {
                if (BLkeyHI.Text.Length == 16)
                {
                    LOG(-1, "=============REWRITE KEY (FASTBOOT)=============");
                    LOG(-1, "=============> KEY: " + BLkeyHI.Text + " <=============");
                    LOG(-1, "=============> LENGHT: " + BLkeyHI.Text + " <=============");
                    if (HISI.ReadInfo(false))
                    {
                        BuildIdTxt.Text = HISI.AVER;
                        ModelIdTxt.Text = HISI.MODEL;
                        VersionIdTxt.Text = HISI.BNUM;
                        FblockStateTxt.Text = HISI.FBLOCKSTATE;
                        BLKEYTXT.Text = HISI.BLKEY;
                        HISI.WriteBOOTLOADERKEY(BLkeyHI.Text);
                    }
                    else LOG(2, "DeviceNotCon");
                }
                else
                    LOG(2, "KeyLenghtERR");
            }
            catch (Exception se)
            {
                if (debug)
                    LOG(2, se.Message);
            }
        }

        private void UNLOCKHISI_Click(object sender, EventArgs e)
        {
            LOG(-1, "-->HISI UNL LOGS<--");
            try
            {
                Tab.Enabled = false;
                if (BLkeyHI.Text.Length == 16)
                {
                    if (isVCOM.Checked)
                    {
                        LOG(-1, "=============REWRITE KEY (KIRIN TESTPOINT)=============");
                        if (String.IsNullOrEmpty(HISIbootloaders.Text))
                        {
                            LOG(1, "HISISelectCpu");
                            Tab.Enabled = true;
                            return;
                        }
                        device = HISIbootloaders.Text.ToUpper();
                        if (!Directory.Exists("UnlockFiles\\" + device))
                        {
                            progr.Value = 1;
                            LOG(0, "DownloadFor", device);
                            LOG(0, "URL: " + source[device]);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            WebClient client = new WebClient();
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressBar);
                            TempDownload = HISIbootloaders.Text.ToUpper();
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);
                            client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                            UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD");
                            Tab.Enabled = false;
                            return;
                        }
                        Path = "UnlockFiles\\" + device + "\\manifest.xml";
                        Port_D data = GETPORT("huawei usb com", PORTBOX, AutoCom.Checked);
                        if (data.ComName != "NaN")
                        {
                            FlashToolHisi.FlashBootloader(Bootloader.ParseBootloader(Path), data.ComName);
                            LOG(0, "[FastBoot] Waiting for any device...");
                            if (HISI.ReadInfo(true))
                            {
                                BuildIdTxt.Text = HISI.AVER;
                                ModelIdTxt.Text = HISI.MODEL;
                                VersionIdTxt.Text = HISI.BNUM;
                                FblockStateTxt.Text = HISI.FBLOCKSTATE;
                                BLKEYTXT.Text = HISI.BLKEY;
                                HISI.WriteBOOTLOADERKEY(BLkeyHI.Text);
                            }
                            else LOG(2, "DeviceNotCon", "FASTBOOT TIMED OUT");
                        }
                        else { Tab.Enabled = true; LOG(2, "DeviceNotCon"); }
                    }
                    else
                    {
                        LOG(-1, "=============REWRITE KEY (FASTBOOT)=============");
                        LOG(-1, "=============> KEY: " + BLkeyHI.Text + " <=============");
                        LOG(-1, "=============> LENGHT: " + BLkeyHI.Text + " <=============");
                        if (HISI.ReadInfo(false))
                        {
                            HISI.WriteBOOTLOADERKEY(BLkeyHI.Text);
                            BuildIdTxt.Text = HISI.AVER;
                            ModelIdTxt.Text = HISI.MODEL;
                            VersionIdTxt.Text = HISI.BNUM;
                            FblockStateTxt.Text = HISI.FBLOCKSTATE;
                            BLKEYTXT.Text = HISI.BLKEY;
                        }
                        else LOG(-1, "DeviceNotCon");
                    }
                }
                else
                    LOG(2, "KeyLenghtERR");
            }
            catch (Exception se)
            {
                if (debug)
                    LOG(2, se.Message);
            }
            Tab.Enabled = true;
        }

        private void HISIbootloaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + HISIbootloaders.Text.ToUpper();
            if (!Directory.Exists(Path)) UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD"); else UNLOCKHISI.Text = Language.Get("HISIWriteKirinBL");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            CMDbox.Text = diag.TestHack();
        }

        private void nButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = PrevFolder;
            openFileDialog.Filter = "Patch0 Repartition data files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = PatXm.Text = openFileDialog.FileName;
        }

        private void EraseMeBtn_Click(object sender, EventArgs e)
        {
            TxSide = GETPORT("qdloader 9008", PORTBOX, AutoCom.Checked);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text)) return;
            DialogResult dialogResult = MessageBox.Show(Language.Get("ERmINFO"), Language.Get("CZdmg"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                if (EraseMemory(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(0, "EraseMS");
                else
                    LOG(2, "EEraseMS");
            progr.Value = 100;
        }

        private void ClearS_Click(object sender, EventArgs e)
        {
            GPTTABLE.Clear();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible = loadedhose = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(6);
        }


        private void UpdatePortList_Click(object sender, EventArgs e)
        {
            foreach (var i in GETPORTLIST(""))
                PORTBOX.Items.Add(i.DeviceName);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //RESET CANCEL BTN
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) process.Kill();
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) process.Kill();
            GPTTABLE.Clear();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible = loadedhose = false;
            Tab.Enabled = true;
            LOG(1, "Canceled...");
        }

        private void FlashUpdAppBTN_Click(object sender, EventArgs e)
        {
            UpdateApp.UnpackAPP("1.app");
        }
    }
}
