using HuaweiUnlocker.Core;
using HuaweiUnlocker.FlashTool;
using HuaweiUnlocker.UI;
using System;
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
                            if (a.FileType.ToString().ToUpper().Equals("ERECOVERY_RAMDIS"))
                                a.FileType = "ERECOVERY_RAMDISK";
                            if (a.FileType.ToString().ToUpper().Equals("RECOVERY_RAMDIS"))
                                a.FileType = "RECOVERY_RAMDISK";
                            LOG(0, "Extracting", a.FileType);
                            UpdFile.Extract(i, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                            i++;
                        }
                        Progress(i / UpdFile.Count * 100, 100);
                    }
                    //Set 0
                    unpacked = true;
                    CreateRWProgram0xml();
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
                        return;
                    }

                    FlashToolQClegacy.FlashPartsXml(pathxmlE, "", GuessMbn(), "UnlockFiles/UpdateAPP");
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
        }
        private static void CreateRWProgram0xml()
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
            LOG(0, "RrGPTXMLSPR", " -> ~/" + pathxml);
            LOG(0, "RrGPTXMLSPR", " -> ~/" + pathxmlE);
            var gpttable = GET_GPT_FROM_FILE(filesInDir[0].FullName, 512);
            if (gpttable.Count > 0)
            {
                WriteGPT_TO_XML(pathxml, gpttable, false);
                WriteGPT_TO_XML(pathxmlE, gpttable, true);
            }
        }
    }
}
