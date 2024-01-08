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
using QMSL_Library;
using Org.BouncyCastle.Utilities.Encoders;

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
            wndw = this;
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
                key.SetValue("DEBUG", false);
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
                StreamReader readerD = new StreamReader(client.OpenRead("https://werasik2aa.github.io/Huawei-Unlock-Tool/js/Data.js"));
                string line = readerD.ReadLine();
                while ((line = readerD.ReadLine()) != null & !line.Contains("];"))
                {
                    if (!line.StartsWith("[") && !String.IsNullOrEmpty(line) && !line.StartsWith("//") && !line.StartsWith("#"))
                    {
                        string[] device = line.Replace("\t", "").Replace("\"", "").Replace(",", "").Split('\'');
                        device[0] = device[0].Split(' ')[0];
                        if (!device[0].StartsWith("KIRIN"))
                            DEVICER.Items.Add(device[0]);
                        else
                        {
                            HISIbootloaders.Items.Add(device[0]);
                            HISIbootloaders2.Items.Add(device[0]);
                        }
                        source.Add(device[0], device[1]);
                    }
                }
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
                if (folderDEV.StartsWith("KIRIN") & !HISIbootloaders.Items.Contains(folderDEV))
                {
                    HISIbootloaders2.Items.Add(folderDEV);
                    HISIbootloaders.Items.Add(folderDEV);
                }
                else if (!DEVICER.Items.Contains(folderDEV))
                    DEVICER.Items.Add(folderDEV);
            }
            Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
            if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
            LoaderBox.Items.Add("");
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
                SelectKFirmw.Text = SelPth.Text = SelectLOADER.Text = Selecty2.Text = Selecty3.Text = Language.Get("SelBtn");
                EraseMeBtn.Text = Language.Get("ErasePM");
                AutoXml.Text = AutoLdr.Text = Language.Get("AutoLBL"); ;
                FlashUKIRINBtn.Text = Flash.Text = Language.Get("FlBtn");
                DUMPALL.Text = Language.Get("DuBtn");
                GPfir.Text = Language.Get("BackupRestoreTag");
                RdGPT.Text = Language.Get("RdGPTBtn");
                ReadPA.Text = Language.Get("ReadPA");
                WritePA.Text = Language.Get("WritePA");
                ErasePA.Text = Language.Get("ErasePA");
                EraseDA.Text = Language.Get("EraseDA");
                FrpHISIUnlock.Text = UnlockFrp.Text = Language.Get("UnlockBTN");
                HomeTag.Text = Language.Get("HomeTag");
                UnlockTag.Text = Language.Get("UnlockTag");
                GPTtag.Text = Language.Get("GPTtag");
                HISItag.Text = Language.Get("UnlockTagHISI");
                GLOADER.Text = Language.Get("LoaderHeader");
                PTOFIRM.Text = PTOFIRM.Text = Language.Get("PathToFirmLBL");
                RAW.Text = Language.Get("RWIMGlbl");
                SLDEV.Text = Language.Get("SELDEVlbl");
                CrtGPTBtn.Text = Language.Get("RrRRGPTXMLBtn");
                CrtGPTBtn2.Text = Language.Get("WrRRGPTXMLBtn");
                ClrFoldersBTN.Text = button12.Text = Language.Get("CancelBtn");
                UnpBTN.Text = Language.Get("UnpBtn");
                FlashUpdAppBTN.Text = Language.Get("FlashUnpBtn");

                Tab.TabPages[0].Text = Language.Get("HomeTag");
                Tab.TabPages[1].Text = Language.Get("UnlockSimpl");
                Tab.TabPages[2].Text = Language.Get("GPTtagSimpl");
                Tab.TabPages[3].Text = Language.Get("UnlockSimplHISI");
                Tab.TabPages[4].Text = Language.Get("HISIGPTtagSimpl");

                PARTLIST.Columns[0].HeaderText = Language.Get("NameTABLE0");
                PARTLIST.Columns[1].HeaderText = Language.Get("NameTABLE1");
                PARTLIST.Columns[2].HeaderText = Language.Get("NameTABLE2");

                KirinFiles.Columns[0].HeaderText = Language.Get("NameTABLE0");
                KirinFiles.Columns[1].HeaderText = Language.Get("NameTABLE2");

                groupBox2.Text = Language.Get("DeviceInfoTag");
                TUTR2.Text = Language.Get("Tutr2");
                groupBox16.Text = groupBox13.Text = groupBox3.Text = ACTBOX.Text = Language.Get("Action");
                //HISI TEXT
                CpuHISIBox2.Text = CpuHISIBox.Text = Language.Get("HISISelectCpu");
                RdHISIinfo.Text = Language.Get("HISIReadFB");
                HISI_board_FB.Text = Language.Get("HISIWriteKirinFB");
                ConnectKIRINBTN.Text = UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD");
                FBLstHISI.Text = Language.Get("HISIWriteKirinFBL");

                Path = "UnlockFiles\\" + DEVICER.Text.ToUpper();
                if (!Directory.Exists(Path)) BoardU.Text = Language.Get("DdBtn"); else BoardU.Text = Language.Get("DdBtnE");
                DBB.Text = Language.Get("DebugLbl");
                LOGGBOX.Text = "Version [" + APP_VERSION + "] BETA/n(C) MOONGAMER (QUALCOMM UNLOCKER)/n(C) MASHED-POTATOES (KIRIN UNLOCKER)".Replace("/n", Environment.NewLine);
                LOG(0, "Thanks 'Yogesh Joshi' for advertising on the website: https://www.softwarecrackguru.com");
                LOG(0, "SMAIN1");
                LOG(0, "SMAIN2");
                LOG(0, "SMAIN3");
                LOG(0, "MAIN1");
                LOG(0, "MAIN2");
                LOG(0, "MAIN3");
                LOG(0, "TutrQC");
                LOG(0, "TutrHI");
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
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
                FlashPartsXml(Xm.Text, PatXm.Text, AutoLdr.Checked ? GuessMbn() : LoaderBox.Text, pather.Text);
            }
            else
            {
                LOG(0, "EemmcWPS", pather.Text);
                FlashPartsRaw(AutoLdr.Checked ? GuessMbn() : LoaderBox.Text, pather.Text);
            }

            Progress(100);
        }

        private void PATHTOFIRMWARE_Clck(object sender, EventArgs e)
        {
            if (!RAW.Checked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "XML Files (*.xml)|*.xml",
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pather.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    if (AutoXml.Checked)
                    {
                        foreach (var a in Directory.GetFiles(pather.Text))
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            Progress(0);
            SaveFileDialog openFileDialog = new SaveFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.FileName;
                LOG(0, "DumpTr", pather.Text);
                Dump(AutoLdr.Checked ? GuessMbn() : LoaderBox.Text, pather.Text);
            }

            Progress(100);
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            SelectLOADER.Enabled = AutoLdr.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            LOG(0, "ReadGPT");
            DeviceInfo.Partitions = new Dictionary<string, Partition>();
            if (ReadGPT(AutoLdr.Checked ? GuessMbn() : LoaderBox.Text))
            {
                LOG(0, "SUCC_ReadGPT");
                foreach (var obj in DeviceInfo.Partitions)
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
                LOG(0, "PartSled", partition);
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = MessageBox.Show(Language.Get("AreY") + Temp, Language.Get("CZdmg"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                LOG(1, "ErPS", Temp);
                Erase(Temp, AutoLdr.Checked ? "" : LoaderBox.Text);
                Progress(100);
            }
        }

        private async void WRITEevent_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            loader = AutoLdr.Checked ? "" : LoaderBox.Text;
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
            loader = AutoLdr.Checked ? "" : LoaderBox.Text;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                CurTask = Task.Run(() =>
                {
                    int i = int.Parse(DeviceInfo.Partitions[Temp].BlockStart);
                    int j = int.Parse(DeviceInfo.Partitions[Temp].BlockNumSectors);
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) { Tab.Enabled = true; return; }
            loader = AutoLdr.Checked ? GuessMbn() : LoaderBox.Text;
            DeviceInfo.Name = device;
            LOG(0, "PrcsUnl");
            Unlock(loader, Path);
            Progress(100);
        }

        private async void UnlockFrp_Click(object sender, EventArgs e)
        {
            Progress(0);
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            device = DEVICER.Text.ToUpper();
            LOG(0, "CheckCon");
            loader = GuessMbn();
            CurTask = Task.Run(() =>
            {
                if (!UnlockFrp(loader))
                    LOG(2, "FailFrp");
                else
                    LOG(0, "SUCC_FrpUnlock");
            }, token);
            PGG.ValueMaximum = 100;
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
        private void DebugE_ch(object sender, EventArgs e)
        {
            debug = DBB.Checked;
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
            if (!DeviceInfo.loadedhose)
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Language.Get("AreY") + Temp, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                LOG(0, "ErPS", Temp);
                Erase("userdata", AutoLdr.Checked ? "" : LoaderBox.Text);
                Progress(100);
            }
        }

        private void RdHISIinfo_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            try
            {
                LOG(-1, "=============READ INFO (FASTBOOT)=============");
                if (HISI.ReadInfo())
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
            LOG(-1, "=============> VALUE: " + (EnDisFBLOCK.Checked ? 0 : 1) + " <=============");
            try
            {
                if (HISI.ReadInfo())
                {
                    BuildIdTxt.Text = HISI.AVER;
                    ModelIdTxt.Text = HISI.MODEL;
                    VersionIdTxt.Text = HISI.BNUM;
                    FblockStateTxt.Text = HISI.FBLOCKSTATE;
                    BLKEYTXT.Text = HISI.BLKEY;
                    VersionIdTxt.Text = HISI.AVER;
                    HISI.SetNVMEProp("FBLOCK", new[] { (byte)(EnDisFBLOCK.Checked ? 0 : 1) });
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
                    if (HISI.ReadInfo())
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
        private async void UNLOCKHISI_Click(object sender, EventArgs e)
        {
            LOG(-1, "-->HISI UNL LOGS<--");
            try
            {
                Tab.Enabled = false;
                if (BLkeyHI.Text.Length == 16)
                {
                    if (isVCOM.Checked)
                    {
                        Tab.Enabled = false;
                        LOG(-1, "=============UNLOCKER BL/FRP (KIRIN TESTPOINT)=============");
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
                            return;
                        }
                        Path = "UnlockFiles\\" + device + "\\manifest.xml";
                        DeviceInfo = new IDentifyDev();
                        LOG(0, "CheckCon");
                        DeviceInfo.Port = GETPORT("huawei usb com", PORTBOX.Text);
                        DeviceInfo.CPUName = UNLOCKHISI.Text;
                        DeviceInfo.loadedhose = false;
                        if (DeviceInfo.Port.ComName != "NaN")
                        {
                            CurTask = Task.Run(() =>
                            {
                                HISI.StartUnlockPRCS(FRPchk.Checked, BLkeyHI.Text, Bootloader.ParseBootloader(Path), DeviceInfo.Port.ComName);
                            });
                            await CurTask;
                            if (HISI.AVER != HISI.BSN) DeviceInfo.loadedhose = true;
                            BuildIdTxt.Text = HISI.AVER;
                            ModelIdTxt.Text = HISI.MODEL;
                            VersionIdTxt.Text = HISI.BNUM;
                            FblockStateTxt.Text = HISI.FBLOCKSTATE;
                            BLKEYTXT.Text = HISI.BLKEY;
                        }
                        else { Tab.Enabled = true; LOG(2, "DeviceNotCon"); }
                    }
                    else
                    {
                        if (!FRPchk.Checked)
                        {
                            LOG(-1, "=============REWRITE KEY (FASTBOOT)=============");
                            LOG(-1, "=============> KEY: " + BLkeyHI.Text + " <=============");
                            LOG(-1, "=============> LENGHT: " + BLkeyHI.Text + " <=============");
                        }
                        else
                            LOG(-1, "=============UNLOCK FRP (FASTBOOT)=============");
                        LOG(-1, "=============Only for unlocked bootloader=============");

                        if (HISI.ReadInfo())
                        {
                            BuildIdTxt.Text = HISI.AVER;
                            ModelIdTxt.Text = HISI.MODEL;
                            VersionIdTxt.Text = HISI.BNUM;
                            FblockStateTxt.Text = HISI.FBLOCKSTATE;
                            BLKEYTXT.Text = BLkeyHI.Text;
                            if (!FRPchk.Checked)
                                HISI.WriteBOOTLOADERKEY(BLkeyHI.Text);
                            else
                                HISI.UnlockFRP();
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            DialogResult dialogResult = MessageBox.Show(Language.Get("ERmINFO"), Language.Get("CZdmg"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                if (EraseMemory(AutoLdr.Checked ? GuessMbn() : LoaderBox.Text))
                    LOG(0, "EraseMS");
                else
                    LOG(2, "EEraseMS");
            Progress(100);
        }

        private void ClearS_Click(object sender, EventArgs e)
        {
            DeviceInfo.Partitions.Clear();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible;
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
            DeviceInfo = new IDentifyDev();
            PARTLIST.Rows.Clear();
            PARTLIST.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT.Enabled = WHAT.Visible;
            Tab.Enabled = true;
            UpdateApp.unpacked = false;
            HISI.BSN = HISI.AVER = HISI.BNUM = HISI.MODEL = "NaN";
            LOG(1, "Canceled");
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
            if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                UpdateApp.Unpack(openFileDialog.FileName, 2);
        }

        private void Searching(object sender, EventArgs e)
        {
            var a = PORTBOX.Text;
            PORTBOX.Items.Clear();
            PORTBOX.Items.Add("");
            PORTBOX.Items.Add("Auto");
            foreach (var i in GETPORTLIST())
                PORTBOX.Items.Add(i.FullName);
            if (DeviceInfo.loadedhose && !PORTBOX.Items.Contains(DeviceInfo.Port.FullName))
            {
                LOG(0, "DPort", DeviceInfo.Port.FullName);
                PORTBOX.Items.Remove(DeviceInfo.Port.FullName);
                DeviceInfo = new IDentifyDev();
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
            PORTBOX.SelectedText = a;
            PORTBOX.SelectedItem = a;
        }

        private void CrtGptBTN2_Click(object sender, EventArgs e)
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
                        WriteGPT_TO_XML(saveFileDialog.FileName, gpttable, false);
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
                    if (!CheckDevice(AutoLdr.Checked ? "" : LoaderBox.Text, PORTBOX.Text)) return;
                    LOG(0, "ReadGPT");
                    Progress(20);
                    if (ReadGPT(AutoLdr.Checked ? GuessMbn() : LoaderBox.Text))
                        if (DeviceInfo.Partitions.Count > 0)
                        {
                            Progress(50);
                            LOG(0, "SUCC_ReadGPT");
                            LOG(0, "RrGPTXMLSPR", papthto);
                            WriteGPT_TO_XML(papthto, DeviceInfo.Partitions, false);
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

        private void BypAuBTN_Click(object sender, EventArgs e)
        {

        }

        private void IdentifyBTN_Click(object sender, EventArgs e)
        {
            LOG(0, "CheckCon", " [HISI]");
            var portQC = DeviceInfo.Port = GETPORT("huawei usb com", PORTBOX.Text);
            LOG(0, "CheckCon", " [QCOM]");
            var portHISI = DeviceInfo.Port = GETPORT("qdloader 9008", PORTBOX.Text);
            if (!portHISI.ComName.Equals("NaN"))
                LOG(0, "CPort", "[HISI] " + DeviceInfo.Port.FullName);
            if (!portQC.ComName.Equals("NaN"))
            {
                LOG(0, "CPort", "[QCOM] " + DeviceInfo.Port.FullName);
                GetIdentifier();
                LOG(0, "LoaderSearch");
                GuessMbnTest();
            }
            HISI.ReadInfo(5);
            if (portQC.ComName == portHISI.ComName & !HISI.IsDeviceConnected())
                LOG(1, "NoDEVICEAnsw");
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

        private void ISAS2(object sender, EventArgs e)
        {
            LoaderBox.Text = "";
            LoaderBox.Items.Clear();
            if (AutoLdr.Checked)
            {
                LoaderBox.Items.Add("");
                foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
                    LoaderBox.Items.Add(a.Split('\\').Last());
                LoaderBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else
                LoaderBox.DropDownStyle = ComboBoxStyle.DropDown;
            SelectLOADER.Enabled = !AutoLdr.Checked;
        }

        private void FrpHISIUnlock_Click(object sender, EventArgs e)
        {
            FRPchk.Checked = true;
            UNLOCKHISI_Click(sender, e);
        }

        private void RebootFB_Click(object sender, EventArgs e)
        {
            HISI.Reboot();
        }

        private void HomeeBTN_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(0);
        }

        private void QcomUnlBTN_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(1);
        }

        private void QcomPartsBTN_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(2);
        }

        private void KirinPartsBTN_Click(object sender, EventArgs e)
        {
            Tab.SelectTab(3);
        }

        private async void ConnectKIRINBTN_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            LOG(-1, "=============BOOTLOADER (KIRIN TESTPOINT->FASTBOOT)=============");
            if (String.IsNullOrEmpty(HISIbootloaders2.Text))
            {
                LOG(1, "HISISelectCpu");
                Tab.Enabled = true;
                return;
            }
            device = HISIbootloaders2.Text.ToUpper();
            if (!Directory.Exists("UnlockFiles\\" + device))
            {
                Progress(1);
                LOG(0, "DownloadFor", device);
                LOG(0, "URL: " + source[device]);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressBar);
                Temp = HISIbootloaders2.Text.ToUpper();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded);
                client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD");
                return;
            }
            Path = "UnlockFiles\\" + device + "\\manifest.xml";
            DeviceInfo = new IDentifyDev();
            LOG(0, "CheckCon");
            DeviceInfo.Port = GETPORT("huawei usb com", PORTBOX.Text);
            if (DeviceInfo.Port.ComName != "NaN")
            {
                CurTask = Task.Run(() =>
                {
                    HISI.WriteKirinBootloader(Bootloader.ParseBootloader(Path), DeviceInfo.Port.ComName);
                });
                await CurTask;
                if (DeviceInfo.loadedhose = HISI.FBLOCK)
                    LOG(0, "Ready to flash firmware");
                else
                    LOG(0, "Not ready for flash");
            }
            else
            {
                LOG(0, "[Fastboot] ", "CheckCon");
                if (HISI.ReadInfo())
                    if (DeviceInfo.loadedhose = HISI.FBLOCK)
                        LOG(0, "Ready to flash firmware");
                    else
                        LOG(0, "Not ready for flash");
                else
                    Tab.Enabled = LOG(2, "DeviceNotCon");
            }
        }

        private void FlashUKIRINBtn_Click(object sender, EventArgs e)
        {
            ConnectKIRINBTN_Click(sender, e);
            if (!HISI.FBLOCK & DeviceInfo.loadedhose)
            {
                LOG(2, "HISIInfoS");
                return;
            }
            if (DeviceInfo.Partitions.Count <= 1)
            {
                LOG(2, "ErrBin");
                return;
            }
            Fastboot fb = new Fastboot();
            try
            {
                if (fb.Connect())
                {
                    foreach (var a in DeviceInfo.Partitions)
                    {
                        LOG(0, "Writer", a);
                        var res = fb.Command("flash:" + a.Key.Split('.')[0]);
                        LOG(0, "Response:", res.Payload);
                        fb.UploadData("UnlockFiles/UpdateAPP/" + a.Key);
                    }
                }
                fb.Disconnect();
            }
            catch(Exception ea)
            {
                LOG(2, "Unknown", ea.Message);
            }
        }

        private void SelectKFirmw_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Firmware Files (*.img)|*.img|(*.app)|*.app",
            };
            Tab.Enabled = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                KirinFirmPath.Text = openFileDialog.FileName;
                if (!string.IsNullOrEmpty(KirinFirmPath.Text) & KirinFirmPath.Text.ToLower().EndsWith(".app") & File.Exists(KirinFirmPath.Text))
                    UpdateApp.Unpack(KirinFirmPath.Text, 3);
                else if (!string.IsNullOrEmpty(KirinFirmPath.Text) & KirinFirmPath.Text.ToLower().EndsWith(".img") & File.Exists(KirinFirmPath.Text))
                {
                    UpdateApp.ReadFilesInDirAsPartitions();
                    Tab.Enabled = true;
                }

                while (Tab.Enabled != true)
                    Application.DoEvents();
                foreach (var p in DeviceInfo.Partitions)
                    KirinFiles.Rows.Add(p.Key, p.Value.BlockLength);
                KirinFiles.AutoResizeRows();
                LOG(0, "Done");
            }

        }

        private void HISIbootloaders2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + HISIbootloaders2.Text.ToUpper();
            if (!Directory.Exists(Path)) ConnectKIRINBTN.Text = Language.Get("HISIWriteKirinBLD"); else ConnectKIRINBTN.Text = Language.Get("HISIInitFB");
        }
    }
}
