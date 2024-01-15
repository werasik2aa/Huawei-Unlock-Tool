using HuaweiUnlocker.Core;
using HuaweiUnlocker.FlashTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static HuaweiUnlocker.LangProc;

namespace HuaweiUnlocker.TOOLS
{
    public class UpdateApp
    {
        private const string pathxml = "UnlockFiles/UpdateAPP/rawprogram0.xml";
        private const string pathxmlE = "UnlockFiles/UpdateAPP/rawprogram01.xml";
        public static bool unpacked = false;
        public static async void Unpack(string path, int state)
        {
            Tab.Enabled = false;
            var i = 0;
            if (!Directory.Exists("/UnlockFiles/UpdateAPP/")) Directory.CreateDirectory("UnlockFiles/UpdateAPP");
            UpdateFile UpdFile = UpdateFile.Open(path, false);
            if (state > 0)
            {
                CurTask = Task.Run(() =>
                {
                    Directory.Delete("UnlockFiles/UpdateAPP", true);
                    if (!Directory.Exists("/UnlockFiles/UpdateAPP/")) Directory.CreateDirectory("UnlockFiles/UpdateAPP");
                    foreach (var a in UpdFile) // STAGE EXTRACT
                    {
                        if (!unpacked)
                        {
                            if (a.FileType.ToString().ToUpper().Trim().Equals("ERECOVERY_RAMDIS"))
                                a.FileType = "ERECOVERY_RAMDISK";
                            if (a.FileType.ToString().ToUpper().Trim().Equals("RECOVERY_RAMDIS"))
                                a.FileType = "RECOVERY_RAMDISK";
                            LOG(0, "Extracting", a.FileType);
                            UpdFile.Extract(i, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                            i++;
                        }
                        Progress(i, UpdFile.Count);
                    }
                    //Set 0
                    unpacked = true;
                    if (state != 3)
                        CreateRWProgram0xml();
                    else
                        ReadFilesInDirAsPartitions();
                });
                await CurTask;
                if (state == 2)
                {
                    Progress(0);//0(STAGE FLASHING DLOAD 9008 MODE)
                    DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("UnlockFiles/UpdateAPP/");
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + "gpt" + "*.*");
                    if (filesInDir.Length == 0)
                    {
                        LOG(2, "RrGPTXMLE");
                        LOG(2, "NotFoundF", "GPT.img");
                        Tab.Enabled = true;
                        return;
                    }

                    var gpttable = GET_GPT_FROM_FILE(filesInDir[0].FullName, 512);
                    foreach (var a in UpdFile)
                    {
                        if (a.FileType.ToString().ToUpper().Trim().Equals("ERECOVERY_RAMDIS"))
                            a.FileType = "ERECOVERY_RAMDISK";
                        if (a.FileType.ToString().ToUpper().Trim().Equals("RECOVERY_RAMDIS"))
                            a.FileType = "RECOVERY_RAMDISK";
                        if (gpttable.ContainsKey(a.FileType.ToLower()))
                        {
                            FlashToolQClegacy.CurPartLenght = (int)a.FileSize;
                            string command = "Tools\\emmcdl.exe";
                            string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + GuessMbn() + '"' + " -b " + a.FileType.ToLower() + " " + '"' + "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img" + '"';
                            SyncRUN(command, subcommand);
                            i++;
                        }
                        Progress(i, gpttable.Count);
                    }
                }
            }
            else
            {
                LOG(0, "Searching GPT.bin in file:", path);
                foreach (var a in UpdFile)
                {
                    if (a.FileType.ToLower().Contains("gpt"))
                    {
                        LOG(0, "Extracting", a.FileType);
                        LOG(0, a.BlockSize.ToString(), " " + a.FileSequence + " " + a.DataOffset);
                        UpdFile.Extract(i, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                        break;
                    }
                    i++;
                }
                CreateRWProgram0xml();
            }
            Progress(100);
            LOG(0, "Done", DateTime.Now);
            Tab.Enabled = true;
        }
        public static void ReadFilesInDirAsPartitions()
        {
            Dictionary<string, Partition> gpttable = new Dictionary<string, Partition>();
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("UnlockFiles/UpdateAPP/");
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles();
            foreach (var a in filesInDir)
                gpttable.Add(a.FullName.Split('\\').Last(), new Partition()
                {
                    BlockLength = new FileInfo(a.FullName).Length.ToString(),
                });
            DeviceInfo.Partitions = gpttable;
        }
        public static void CreateRWProgram0xml()
        {
            if (!Directory.Exists("UnlockFiles/UpdateAPP"))
            {
                LOG(2, "RrGPTXMLE", "NoRights");
                return;
            }
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("UnlockFiles/UpdateAPP/");
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + "gpt" + "*.*");
            if (filesInDir.Length == 0)
            {
                LOG(2, "RrGPTXMLE");
                LOG(2, "NotFoundF", "GPT.img");
                return;
            }
            if (!filesInDir[0].FullName.StartsWith("hisi"))
            {
                LOG(0, "RrGPTXMLSPR", " -> ~/" + pathxml);
                LOG(0, "RrGPTXMLSPR", " -> ~/" + pathxmlE);
                Dictionary<string, Partition> gpttable = GET_GPT_FROM_FILE(filesInDir[0].FullName, 512);
                if (gpttable.Count > 0)
                {
                    WriteGPT_TO_XML(pathxml, gpttable, false);
                    WriteGPT_TO_XML(pathxmlE, gpttable, true);
                }
            }
        }
    }
}
