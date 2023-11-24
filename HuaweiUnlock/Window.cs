using System;
using System.Windows.Forms;
using System.IO;
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
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace HuaweiUnlocker
{

    public partial class Window : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
        public static HISI HISI = new HISI();
        private Dictionary<string, string> source = new Dictionary<string, string>();
        private Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\4PDA_HUAWEI_UNLOCK", true);
        private static string Temp;
        public Window()
        {
            InitializeComponent();
            foreach (var process in Process.GetProcesses())
                if (process.ProcessName.Contains("emmcdl.exe"))
                    process.Kill();
            foreach (var process in Process.GetProcesses())
                if (process.ProcessName.Contains("fh_loader.exe"))
                    process.Kill();

            //Create Folders
            if (!Directory.Exists("UnlockFiles")) Directory.CreateDirectory("UnlockFiles");
            if (!Directory.Exists("LOGS")) Directory.CreateDirectory("LOGS");
            if (!Directory.Exists("Languages")) Directory.CreateDirectory("Languages");
            if (!Directory.Exists("Tools")) Directory.CreateDirectory("Tools");

            //Extract languages if not exist
            if (!File.Exists("Languages\\Russian.ini")) ResourcesMNG.SaveResources("Russian.ini", "Languages");
            if (!File.Exists("Languages\\English.ini")) ResourcesMNG.SaveResources("English.ini", "Languages");

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
            PRG = PGG;

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
                    else if (!HISIbootloaders.Items.Contains(folderDEV))
                        HISIbootloaders.Items.Add(folderDEV);
            }
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
            foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                LoaderBox.Items.Add(a.Split('\\').Last());
            LangProc.Tab = Tab;
            PORTBOX.Items.Add("");
            PORTBOX.Items.Add("Auto");
            foreach (var i in GETPORTLIST())
                PORTBOX.Items.Add(i.FullName);
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
        }
        public void Lang()
        {
            if (Language.ReadLngFile())
            {
                //QUALCOMM AND BASIC
                SelPth.Text = SelectLOADER.Text = Selecty2.Text = Selecty3.Text = Language.Get("SelBtn");
                EraseMeBtn.Text = Language.Get("ErasePM");
                AutoXml.Text = AutoLdr.Text = Language.Get("AutoLBL"); ;
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
                CrtGPTBtn.Text = Language.Get("RrRRGPTXMLBtn");
                CrtGPTBtn2.Text = Language.Get("WrRRGPTXMLBtn");
                ClrFoldersBTN.Text = button12.Text = Language.Get("CancelBtn");
                UnpBTN.Text = Language.Get("UnpBtn");
                FlashUpdAppBTN.Text = Language.Get("FlashUnpBtn");

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
                LOGGBOX.Text = "Version [" + APP_VERSION + "] BETA/n(C) MOONGAMER (QUALCOMM UNLOCKER)/n(C) MASHED-POTATOES (KIRIN UNLOCKER)".Replace("/n", Environment.NewLine);
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
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Programmer files (*.mbn;*.elf;*.hex)|*.mbn;*.elf;*.hex|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = LoaderBox.Text = openFileDialog.FileName;
        }
        private void XML_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Sectors data files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) PrevFolder = Xm.Text = openFileDialog.FileName;
        }

        private void Flash_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(pather.Text) || !Directory.Exists(pather.Text) && !File.Exists(pather.Text))
            {
                LOG(2, "NoFirmPath");
                return;
            }
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            Progress(0);
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
                LOG(0, "EemmcXML_WPS", pather.Text);
                FlashPartsXml(Xm.Text, PatXm.Text, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text);
            }
            else
            {
                LOG(0, "EemmcWPS", pather.Text);
                FlashPartsRaw(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text);
            }

            Progress(100);
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
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            Progress(0);
            SaveFileDialog openFileDialog = new SaveFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.FileName;
                LOG(0, "DumpTr", pather.Text);
                Dump(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, pather.Text);
            }

            Progress(100);
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            SelectLOADER.Enabled = AutoLdr.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            LOG(0, "ReadGPT");
            LangProc.DeviceInfo.Partitions = new Dictionary<string, Partition>();
            if (ReadGPT(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
            {
                LOG(0, "SUCC_ReadGPT");
                foreach (var obj in LangProc.DeviceInfo.Partitions)
                    PARTLIST.Rows.Add(obj.Key, obj.Value.BlockStart, obj.Value.BlockLength);
                PARTLIST.AutoResizeRows();
                RdGPT.Enabled = RdGPT.Visible = false;
                Progress(100);
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
                Temp = partition;
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
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Language.Get("AreY") + Temp, Language.Get("CZdmg"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                LOG(1, "ErPS", Temp);
                Erase(Temp, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text);
                Progress(100);
            }
        }

        private async void WRITEevent_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            loader = AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text;
            if (file.ShowDialog() == DialogResult.OK)
            {
                CurTask = Task.Run(() =>
                {
                    LOG(0, "EwPS", Temp + newline);
                    Write(Temp, loader, file.FileName);
                    Progress(100);
                }, token);
                await CurTask;
            }
        }

        private async void READevent_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog folder = new SaveFileDialog();
            loader = AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                CurTask = Task.Run(() =>
                {
                    int i = int.Parse(LangProc.DeviceInfo.Partitions[Temp].BlockStart);
                    int j = int.Parse(LangProc.DeviceInfo.Partitions[Temp].BlockNumSectors);
                    LOG(0, "EdPS", Temp + newline);
                    Dump(i, j, Temp, loader, folder.FileName);
                    Progress(100);
                }, token);
                await CurTask;
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
            LOG(0, "Downloaded", Temp + ".zip");
            UnZip(Temp + ".zip", "UnlockFiles\\" + Temp);
            UNLBTN_Click(sender, e);
        }
        private void ProgressBar(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress(e.ProgressPercentage);
        }
        private void UNLBTN_Click(object sender, EventArgs e)
        {
            Progress(0);
            device = DEVICER.Text.ToUpper();
            Path = "UnlockFiles\\" + device;
            if (!DEVICER.Text.Contains("-")) { LOG(0, "SelDev"); return; }
            if (!Directory.Exists(Path))
            {
                Progress(1);
                LOG(0, "DownloadFor" + device);
                LOG(0, "URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressBar);
                Temp = DEVICER.Text.ToUpper();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);
                client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                BoardU.Text = Language.Get("DdBtnE");
                Tab.Enabled = false;
                return;
            }
            loader = AutoLdr.Checked ? PickLoader(DEVICER.Text.Split('-').First()) : LoaderBox.Text;
            if (!CheckDevice(loader, PORTBOX.Text)) { Tab.Enabled = true; return; }
            LangProc.DeviceInfo.Name = device;
            LOG(0, "PrcsUnl", newline);
            Unlock(loader, Path);
            Progress(100);
        }

        private async void UnlockFrp_Click(object sender, EventArgs e)
        {
            Progress(0);
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            device = DEVICER.Text.ToUpper();
            LOG(0, "CheckCon");
            loader = AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text;
            CurTask = Task.Run(() =>
            {
                if (!UnlockFrp(loader))
                    LOG(2, "FailFrp");
                else
                    LOG(0, "SUCC_FrpUnlock");
            }, token);
            Progress(100);
            await CurTask;
        }
        private bool Find()
        {
            Port_D data = GETPORT("android adapter pcui");
            if (DIAG.PCUI != data.ComName)
            {
                DIAG.PCUI = data.ComName;
                LOG(0, data.ComName != "NaN" ? Language.Get("Info") + "PCUI PORT: " + data.FullName : Language.Get("Error") + "PCUI PORT not found");
            }
            else LOG(1, "Not Found PCUI");

            data = GETPORT("dbadapter reserved interface");
            if (DIAG.DBDA != data.ComName)
            {
                DIAG.DBDA = data.ComName;
                LOG(0, data.ComName != "NaN" ? Language.Get("Info") + "DBADAPTER PORT: " + data.FullName : Language.Get("Error") + "DBADAPTER PORT not found");
            }
            else LOG(1, "Not Found DBADAPTER");

            return DIAG.DBDA != "NaN" & DIAG.PCUI != "NaN";
        }
        private void ReadINFODIAG_Click(object sender, EventArgs e)
        {
            if (!Find()) return;

        }

        private void RB_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            DIAG.REBOOT();
        }

        private void RecoveryBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            DIAG.To_Three_Recovery();
        }

        private void TryAUTH_CLCK(object sender, EventArgs e)
        {
            if (!Find()) return;
            LOG(-1, "TRYING TO AUTH PHONE FUCK THE HW: !!!BETA TEST NOT WORKING!!!");

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
            PatXm.Enabled = SelPth.Enabled = Xm.Enabled = AutoXml.Enabled = Selecty2.Enabled = !RAW.Checked;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (CurProcess != null)
            {
                CurProcess.Close();
                CurProcess.Dispose();
            }
            if (token != null && ct != null && CurTask != null)
            {
                ct.Cancel();
                ct.Dispose();
                if (CurTask.IsCompleted)
                    CurTask.Dispose();
                ct = new CancellationTokenSource();
                token = ct.Token;
            }
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
            key.SetValue("LANGUAGE", LBOX.Text);
            key.SetValue("DEBUG", DBB.Checked);
            Language.CURRENTlanguage = LBOX.Text;
            Lang();
        }

        private void Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!LangProc.DeviceInfo.loadedhose)
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
            DIAG.FACTORY_RESET_MNF(1);
        }

        private void EraseDA_Click(object sender, EventArgs e)
        {
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Language.Get("AreY") + Temp, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                LOG(0, "ErPS", Temp);
                Erase("userdata", AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text);
                Progress(100);
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
                    LOG(-1, "=============> LENGHT: 16 <=============");
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
                            Progress(1);
                            LOG(0, "DownloadFor", device);
                            LOG(0, "URL: " + source[device]);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            WebClient client = new WebClient();
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressBar);
                            Temp = HISIbootloaders.Text.ToUpper();
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);
                            client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                            UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD");
                            Tab.Enabled = false;
                            return;
                        }
                        Path = "UnlockFiles\\" + device + "\\manifest.xml";
                        LangProc.DeviceInfo = new IDentifyDev();
                        LangProc.DeviceInfo.Port = GETPORT("huawei usb com", PORTBOX.Text);
                        if (LangProc.DeviceInfo.Port.ComName != "NaN")
                        {
                            FlashToolHisi.FlashBootloader(Bootloader.ParseBootloader(Path));
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
            //CMDbox.Text = DIAG.TestHack();
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
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = MessageBox.Show(Language.Get("ERmINFO"), Language.Get("CZdmg"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                if (EraseMemory(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                    LOG(0, "EraseMS");
                else
                    LOG(2, "EEraseMS");
            Progress(100);
        }

        private void ClearS_Click(object sender, EventArgs e)
        {
            LangProc.DeviceInfo.Partitions.Clear();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(6);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //RESET CANCEL BTN
            if (CurProcess != null)
            {
                CurProcess.Close();
                CurProcess.Dispose();
            }
            if (token != null && ct != null && CurTask != null)
            {
                ct.Cancel();
                ct.Dispose();
                if (CurTask.IsCompleted)
                    CurTask.Dispose();
                ct = new CancellationTokenSource();
                token = ct.Token;
            }
            LangProc.DeviceInfo = new IDentifyDev();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible;
            Tab.Enabled = true;
            UpdateApp.unpacked = false;
            LOG(1, "Canceled");
            CMDbox.Text = "";
        }

        private void FlashUpdAppBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Update.app (*.app;*.APP)|*.app;*.APP|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = "Update.APP"
            };
            if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                UpdateApp.Unpack(openFileDialog.FileName, 2, AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text);
        }

        private void Searching(object sender, EventArgs e)
        {
            PORTBOX.Items.Clear();
            PORTBOX.Items.Add("");
            PORTBOX.Items.Add("Auto");
            foreach (var i in GETPORTLIST())
                PORTBOX.Items.Add(i.FullName);
            if (LangProc.DeviceInfo.loadedhose && !PORTBOX.Items.Contains(LangProc.DeviceInfo.Port.FullName))
            {
                LOG(0, "DPort", LangProc.DeviceInfo.Port.FullName);
                PORTBOX.Items.Remove(LangProc.DeviceInfo.Port.FullName);
                LangProc.DeviceInfo = new IDentifyDev();
                if (CurProcess != null)
                {
                    CurProcess.Close();
                    CurProcess.Dispose();
                }
                PARTLIST.Rows.Clear();
                PARTLIST.Update();
                RdGPT.Visible = RdGPT.Enabled = true;
                WHAT.Enabled = WHAT.Visible;
                Tab.Enabled = true;
            }
        }

        private void CrtGptBTN2_Click(object sender, EventArgs e)
        {
            if (debug) LOG(-1, "===================.CREATE GPT CrtGptBTN_Click().===================");
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Rawprogram0.xml (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = Language.Get("SelPathToGPTXML")
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string papthto = saveFileDialog.FileName;
                try
                {
                    if (!CheckDevice(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text, PORTBOX.Text)) return;
                    LOG(0, "ReadGPT");
                    Progress(20);
                    if (ReadGPT(AutoLdr.Checked ? PickLoader(LoaderBox.Text) : LoaderBox.Text))
                        if (LangProc.DeviceInfo.Partitions.Count > 0)
                        {
                            Progress(50);
                            LOG(0, "SUCC_ReadGPT");
                            LOG(0, "RrGPTXMLSPR", papthto);
                            WriteGPT_TO_XML(papthto, LangProc.DeviceInfo.Partitions);
                            Progress(100);
                        }
                        else
                            LOG(2, "RrGPTXMLE", "ERR_ReadGPT");
                }
                catch (Exception esa)
                {
                    LOG(2, "RrGPTXMLE", esa.Message);
                }
                Progress(0);
            }
        }

        private void UnpBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Update.app (*.app;*.APP)|*.app;*.APP|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = "Update.APP"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                UpdateApp.Unpack(openFileDialog.FileName, 1);
        }

        private void CrtGPTBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    InitialDirectory = PrevFolder,
                    Filter = "Rawprogram0.xml (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    Title = Language.Get("SelPathToGPTXML")
                };
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = PrevFolder,
                    Filter = "gpt_####0.bin (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    Title = Language.Get("SelPathToGPT")
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var gpttable = GET_GPT_FROM_FILE(openFileDialog.FileName, 512);
                    if (gpttable.Count > 0)
                    {
                        Progress(50);
                        LOG(0, "RrGPTXMLSPR", saveFileDialog.FileName);
                        LOG(0, "File", openFileDialog.FileName + "<==-");
                        WriteGPT_TO_XML(saveFileDialog.FileName, gpttable);
                        Progress(100);
                    }
                    else
                        LOG(2, "RrGPTXMLE", "ERR_ReadGPTFile");
                }
                else
                    LOG(2, "RrGPTXMLE", "ERR_ReadGPTFile2");
            }
            catch (Exception ee) { LOG(2, ee.Message); }
        }

        private void BypAuBTN_Click(object sender, EventArgs e)
        {
            
        }

        private void IdentifyBTN_Click(object sender, EventArgs e)
        {
            LOG(0, "CheckCon");
            LangProc.DeviceInfo.Port = GETPORT("qdloader 9008", PORTBOX.Text);
            GetIdentifier();
        }
        private async void ReadOemBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = PrevFolder,
                Filter = "Update.app (*.app;*.APP)|*.app;*.APP|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = "Update.APP"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                IndexiesOEMdata.Items.Clear();
                CurTask = Task.Run(() =>
                {
                    OemInfoTool.Decompile(openFileDialog.FileName);
                }, token);
                await CurTask;
                foreach (var item in OemInfoTool.data)
                    IndexiesOEMdata.Items.Add(item);
                CompileOemInfBTN.Enabled = true;
                LOG(0, "Done");
            }
        }
        private void TabPageOeminfSwitch(object sender, EventArgs e)
        {
            string o = "NoData";
            if (!string.IsNullOrEmpty(IndexiesOEMdata.Text) && IndexiesOEMdata.Items.Count > 0 && IndexiesOEMdata.Items.Contains(IndexiesOEMdata.Text))
            {
                o = CRC.HexDump(File.ReadAllBytes("UnlockFiles/OemInfoData/" + IndexiesOEMdata.Text));
                o = TrimOrNCHK.Checked ? o.Trim('F').Replace("FFFF", "") : o;
            }
            ContantOemText.Text = o;
        }

        private void ClrFoldersBTN_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("UnlockFiles")) Directory.Delete("UnlockFiles", true);
            if (Directory.Exists("LOGS")) Directory.Delete("LOGS", true);
        }

        private async void CompileOemInfBTN_Click(object sender, EventArgs e)
        {
            LOG(0, "Compiling oeminfo.img");
            CurTask = Task.Run(() =>
            {
                OemInfoTool.Compile("UnlockFiles/OemInfoData", "UnlockFiles/oeminfo-unsigned-unhashed.img");
            }, token);
            await CurTask;
            LOG(0, "Done");
        }

        private void SendCMDtestBTN_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            var ss = DIAG.READ_SECRET_KEY();
            var scr = LibCrypt.Decrypt7Cisco(ss);
            CMDbox.Text = "[KEY_READ]" + newline + CRC.HexDump(ss) + newline + "[CISCO7_DECRYPT]" + newline + CRC.HexDump(scr) + newline + "[AUTH_RESPONSE]" + newline + CRC.HexDump(DIAG.AUTH_PHONE(scr));
        }

        private void RDinf_Click(object sender, EventArgs e)
        {
            if (!Find()) return;
            string data = "";
            byte[] SecretKey_BIN = DIAG.READ_SECRET_KEY();
            string SecretKey_HEX = CRC.BytesToHexString(SecretKey_BIN);
            string SecretKey_BASE64 = Convert.ToBase64String(SecretKey_BIN);
            string SecretKey_BASE62_DEC = CRC.BytesToHexString(LibCrypt.BS62.Decode(SecretKey_BIN));
            string SecretKey_CISCO7_DECR = CRC.BytesToHexString(LibCrypt.Decrypt7Cisco(SecretKey_BIN));
            string AUTH_RESP = CRC.BytesToHexString(DIAG.AUTH_PHONE(SecretKey_CISCO7_DECR));
            string TimeIMSI = DIAG.READ_TIME_IMSI();
            string IMEI = DIAG.READ_IMEI_1();
            string IMEI2 = DIAG.READ_IMEI_2();
            string BT_MAC = DIAG.READ_BLTU_MAC();
            string WIFI_MAC = DIAG.READ_WIFI_MAC();
            string Country = Encoding.ASCII.GetString(DIAG.READ_COUNTRY_CODE());
            string[] BSNBID = DIAG.READ_BSN_BUILD_ID();
            string[] FIRMINFdat = DIAG.READ_FIRMWARE_INFO();
            data += "   TimeIMSI: " + TimeIMSI + newline;
            data += "   IMEI: " + IMEI + newline;
            data += "   IMEI2: " + IMEI2 + newline;
            data += "   BT_MAC: " + BT_MAC + newline;
            data += "   WIFI_MAC: " + WIFI_MAC + newline;
            data += "   Country: " + Country + newline;
            data += "   BSN: " + BSNBID[0] + newline;
            data += "   BUILD_ID: " + BSNBID[1] + newline;
            data += "   SECRET_KEY_CRYPTED_HEX: " + SecretKey_HEX + newline;
            data += "   SECRET_KEY_CRYPTED_BASE64: " + SecretKey_BASE64 + newline;
            data += "   SECRET_KEY_CRYPTED_BASE62_DEC: " + SecretKey_BASE62_DEC + newline;
            data += "   SECRET_KEY_CISCO_TYPE_7: " + SecretKey_CISCO7_DECR + newline;
            data += "   SECRET_KEY_AUTH_RESPONSE: " + AUTH_RESP + newline;
            foreach (var i in FIRMINFdat)
                data += "FIRM_INFO: " + i + newline;
            data += "FIRM_INFO: " + newline;
            DeviceInfo.Text = data;
        }
    }
}
