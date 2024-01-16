using HuaweiUnlocker.DIAGNOS;
using HuaweiUnlocker.TOOLS;
using HuaweiUnlocker.UI;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HuaweiUnlocker.FlashTool.FlashToolQClegacy;
using static HuaweiUnlocker.LangProc;

namespace HuaweiUnlocker
{

    public partial class Window : Form
    {
        private static string device;
        private static string loader;
        public static string Path;
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
            if (!File.Exists("Languages\\Russian.ini")) SaveResources("Russian.ini", "Languages");
            if (!File.Exists("Languages\\English.ini")) SaveResources("English.ini", "Languages");

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
                            HISIbootloaders.Items.Add(device[0]);
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
                    HISIbootloaders.Items.Add(folderDEV);
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
                WritePA2.Text = WritePA.Text = Language.Get("WritePA");
                ErasePA2.Text = ErasePA.Text = Language.Get("ErasePA");
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
            if (PARTLIST.Rows.Count > 0 || KirinFiles.Rows.Count > 0)
            {
                KirinFiles.Enabled = PARTLIST.Enabled = false;
                string partition = "NaN";
                if (Tab.SelectedIndex == 2 & PARTLIST.Rows.Count > 0)
                {
                    partition = PARTLIST.CurrentRow.Cells[0].Value.ToString();
                    WHAT.Visible = WHAT.Enabled = true;
                }
                if (Tab.SelectedIndex != 2 & KirinFiles.Rows.Count > 0)
                {
                    partition = KirinFiles.CurrentRow.Cells[0].Value.ToString();
                    WHAT2.Visible = WHAT2.Enabled = true;
                }
                WHAT2.Text = WHAT.Text = Language.Get("Action") + partition;
                Temp = partition;
                LOG(0, "PartSled", partition);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Tab.SelectedIndex == 2)
                WHAT.Visible = WHAT.Enabled = false;
            else
                WHAT2.Visible = WHAT2.Enabled = false;
            KirinFiles.Enabled = PARTLIST.Enabled = true;
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
            var a = ZipFile.Read(zipFile);
            a.ExtractAll(folderPath, ExtractExistingFileAction.OverwriteSilently);
            a.Dispose();
            File.Delete(zipFile);
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
        private void Downloaded2(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LOG(2, "FailCon", newline + Language.Get("Error") + e.Error);
                return;
            }
            LOG(0, "Downloaded", Temp + ".zip");
            UnZip(Temp + ".zip", "UnlockFiles\\" + Temp);
            ConnectKirin();
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


        private void RdHISIinfo_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            ConnectKirin();
            Tab.Enabled = true;
        }
        private void FBLstHISI_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            LOG(-1, "=============WRITE FBLOCK (FASTBOOT)=============");
            LOG(-1, "=============> VALUE: " + (EnDisFBLOCK.Checked ? 0 : 1) + " <=============");
            try
            {
                ConnectKirin();
                if (HISI.IsDeviceConnected())
                {
                    HISI.SetNVMEProp("FBLOCK", new[] { (byte)(EnDisFBLOCK.Checked ? 0 : 1) });
                    HISI.Disconnect();
                }
                else LOG(2, "Unknown");
            }
            catch (Exception se)
            {
                if (debug)
                    LOG(-1, "ERR: " + se);
            }
            Tab.Enabled = true;
            LOG(0, "Done");
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
                    ConnectKirin();
                    if (HISI.IsDeviceConnected())
                    {
                        LOG(0, "HISINewKey", BLKEYTXT.Text = HISI.WriteKEY(BLkeyHI.Text));
                        HISI.Disconnect();
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
        }
        private void UNLOCKHISI_Click(object sender, EventArgs e)
        {
            LOG(-1, "-->HISI UNL LOGS<--");
            Tab.Enabled = false;
            try
            {
                if (BLkeyHI.Text.Length == 16)
                {
                    ConnectKirin();
                    LOG(-1, "=============UNLOCKER BL/FRP (KIRIN TESTPOINT)=============");
                    if(DeviceInfo.loadedhose || !isVCOM.Checked & HISI.IsDeviceConnected())
                    {
                        BLKEYTXT.Text = HISI.WriteKEY(BLkeyHI.Text.ToUpper());
                        if (RbCheck.Checked) HISI.Reboot();
                        HISI.Disconnect();
                    }
                    else LOG(2, isVCOM.Checked? "[HUAWEI COM 1.0] " : "[FASTBOOT] ", "DeviceNotCon");
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
            LOG(0, "Done");
        }

        private void HISIbootloaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            Path = "UnlockFiles\\" + HISIbootloaders.Text.ToUpper();
            if (!Directory.Exists(Path)) UNLOCKHISI.Text = Language.Get("HISIWriteKirinBLD"); else UNLOCKHISI.Text = Language.Get("HISIWriteKirinBL");
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
            if (Tab.SelectedIndex == 2)
            {
                PARTLIST.Rows.Clear();
                PARTLIST.Update();
                RdGPT.Visible = RdGPT.Enabled = true;
                WHAT.Enabled = WHAT.Visible = false;

            }
            else
            {
                WHAT.Enabled = WHAT.Visible;
                KirinFiles.Rows.Clear();
                KirinFiles.Update();
            }
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
            KirinFiles.Rows.Clear();
            KirinFiles.Update();
            RdGPT.Visible = RdGPT.Enabled = true;
            WHAT2.Enabled = WHAT2.Visible = WHAT.Enabled = WHAT.Visible = false;
            Tab.Enabled = true;
            UpdateApp.unpacked = false;
            HISI.BSN = HISI.AVER = HISI.BNUM = HISI.MODEL = "NaN";
            HISI.fb.Disconnect();
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
                KirinFiles.Rows.Clear();
                KirinFiles.Update();
                RdGPT.Visible = RdGPT.Enabled = true;
                WHAT2.Enabled = WHAT2.Visible = WHAT.Enabled = WHAT.Visible = false;
                HISI.BSN = HISI.AVER = HISI.BNUM = HISI.MODEL = "NaN";
                Tab.Enabled = true;
                if (!HISI.fb.device.IsOpen)
                    DeviceInfo.loadedhose = false;
            }
            PORTBOX.SelectedItem = PORTBOX.SelectedText = a;
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
            HISI.ReadInfo();
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
            Tab.Enabled = false;
            FRPchk.Checked = true;
            ConnectKirin();
            if (DeviceInfo.loadedhose || !isVCOM.Checked & HISI.IsDeviceConnected())
                HISI.UnlockFRP();
            else
            {
                LOG(2, isVCOM.Checked ? "[HUAWEI COM 1.0] " : "[FASTBOOT] ", "DeviceNotCon");
                LOG(2, "FailFrp");
            }
            Tab.Enabled = true;
            LOG(0, "Done");
        }

        private void RebootFB_Click(object sender, EventArgs e)
        {
            ConnectKirin();
            if (DeviceInfo.loadedhose || !isVCOM.Checked & HISI.IsDeviceConnected())
                HISI.Reboot();
            else
            {
                LOG(2, isVCOM.Checked ? "[HUAWEI COM 1.0] " : "[FASTBOOT] ", "DeviceNotCon");
                LOG(2, "FailFrp");
            }
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

        private async void ConnectKirin()
        {
            Tab.Enabled = false;
            if (String.IsNullOrEmpty(HISIbootloaders.Text.ToUpper()))
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
                Temp = device;
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloaded2);
                client.DownloadFileAsync(new Uri(source[device]), device + ".zip");
                return;
            }
            LOG(-1, "=============BOOTLOADER (KIRIN TESTPOINT->FASTBOOT)=============");
            Path = "UnlockFiles\\" + device + "\\manifest.xml";
            DeviceInfo = new IDentifyDev();
            LOG(0, "CheckCon");
            DeviceInfo.Port = GETPORT("huawei usb com", PORTBOX.Text);
            if (DeviceInfo.Port.ComName != "NaN")
            {
                CurTask = Task.Run(() =>
                {
                    HISI.FlashBootloader(Bootloader.ParseBootloader(Path), DeviceInfo.Port.ComName);
                });
                await CurTask;
                LOG(0, "[Fastboot] ", "CheckCon");
                if (HISI.IsDeviceConnected(100))
                {
                    HISI.ReadInfo();
                    DeviceInfo.loadedhose = true;
                    HISI.UnlockFBLOCK();
                    BuildIdTxt.Text = HISI.AVER;
                    ModelIdTxt.Text = HISI.MODEL;
                    VersionIdTxt.Text = HISI.BNUM;
                    FblockStateTxt.Text = HISI.FBLOCKSTATE;
                    BLKEYTXT.Text = HISI.BLKEY;
                }
                else { Tab.Enabled = true; LOG(2, "[Fastboot] ", "DeviceNotCon"); }
            }
            else if (HISI.IsDeviceConnected())
            {
                LOG(0, "[Fastboot] ", "CheckCon");
                HISI.ReadInfo();
                DeviceInfo.loadedhose = true;
                HISI.UnlockFBLOCK();
                BuildIdTxt.Text = HISI.AVER;
                ModelIdTxt.Text = HISI.MODEL;
                VersionIdTxt.Text = HISI.BNUM;
                FblockStateTxt.Text = HISI.FBLOCKSTATE;
                BLKEYTXT.Text = HISI.BLKEY;
            }
            else
                LOG(2, "[Fastboot|COM1.0] ", "DeviceNotCon");
            if (HISI.AVER != HISI.BSN) DeviceInfo.loadedhose = true;
        }

        private void FlashUKIRINBtn_Click(object sender, EventArgs e)
        {
            ConnectKirin();
            if (HISI.FBLOCK)
            {
                LOG(2, "HISIInfoS");
                LOG(0, "Trying to continue");
            }
            if (DeviceInfo.Partitions.Count <= 1)
            {
                LOG(2, "ErrBin");
                return;
            }
            try
            {
                if (HISI.fb.Connect(10))
                {
                    if (File.Exists("UnlockFiles/UpdateAPP/hisiufs_gpt.img"))
                    {
                        HISI.fb.UploadData("UnlockFiles/UpdateAPP/hisiufs_gpt.img", "partition");
                        HISI.fb.UploadData("UnlockFiles/UpdateAPP/hisiufs_gpt.img", "ptable");
                    }
                    foreach (var a in DeviceInfo.Partitions)
                    {
                        LOG(0, "Writer", a.Key);
                        HISI.fb.UploadData("UnlockFiles/UpdateAPP/" + a.Key + ".img", a.Key);
                    }
                    LOG(0, "Done");
                }
                HISI.fb.Disconnect();
            }
            catch (Exception ea)
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
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Tab.Enabled = false;
                    KirinFirmPath.Text = openFileDialog.FileName;
                    if (!string.IsNullOrEmpty(KirinFirmPath.Text) & KirinFirmPath.Text.ToLower().EndsWith(".app") & File.Exists(KirinFirmPath.Text))
                        UpdateApp.Unpack(KirinFirmPath.Text, 3);
                    else if (!string.IsNullOrEmpty(KirinFirmPath.Text) & KirinFirmPath.Text.ToLower().EndsWith(".img") & File.Exists(KirinFirmPath.Text))
                    {
                        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("UnlockFiles/UpdateAPP/");
                        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + "gpt" + "*.img");
                        if (filesInDir.Length == 0)
                        {
                            LOG(2, "RrGPTXMLE");
                            LOG(2, "NotFoundF", "GPT.img");
                            UpdateApp.ReadFilesInDirAsPartitions();
                        }
                        else
                            DeviceInfo.Partitions = GET_GPT_FROM_FILE(filesInDir[0].FullName, 512);
                        Tab.Enabled = true;
                    }

                    while (Tab.Enabled != true)
                        Application.DoEvents();
                    foreach (var p in DeviceInfo.Partitions)
                        KirinFiles.Rows.Add(p.Key, p.Value.BlockLength);
                    KirinFiles.AutoResizeRows();
                }
                catch
                {
                    LOG(2, "Selected file not an Update.APP");
                }
                LOG(0, "Done");
            }

        }
        private void WritePA2_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (!HISI.IsDeviceConnected()) HISI.fb.Connect(10);
                if (!HISI.IsDeviceConnected()) return;
                LOG(0, "Writer", file.FileName);
                HISI.fb.UploadData(file.FileName, file.FileName.Split('\\').Last().Split('.')[0]);
                LOG(0, "Done");
            }
        }

        private void ErasePA2_Click(object sender, EventArgs e)
        {
            LOG(0, "Eraser", Temp);
            HISI.fb.Command("erase:" + Temp);
            LOG(0, "Done");
        }

        private void TryUNLHisiFBBtn_Click(object sender, EventArgs e)
        {
            Tab.Enabled = false;
            ConnectKirin();
            if (DeviceInfo.loadedhose || !isVCOM.Checked & HISI.IsDeviceConnected())
                HISI.TryUnlock(HISI.ReadFactoryKey().Trim());
            else LOG(2, isVCOM.Checked ? "[HUAWEI COM 1.0] " : "[FASTBOOT] ", "DeviceNotCon");
            Tab.Enabled = true;
            LOG(0, "Done");
        }

        private void WriteFactoryBL_Click(object sender, EventArgs e)
        {
            ConnectKirin();
            if (DeviceInfo.loadedhose)
                LOG(0, "Done");
            else
            {
                Tab.Enabled = true;
                LOG(2, "[HUAWEI COM 1.0] ", "DeviceNotCon");
            }
        }
    }
}
