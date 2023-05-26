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
        private static bool unpacked = false;
        public static async void Unpack(string path, int state, string loader = "")
        {
            CurTask = Task.Run(() =>
            {
                var i = 0;
                if (state > 0)
                {
                    if (!Directory.Exists("/UnlockFiles/UpdateAPP/"))
                        Directory.CreateDirectory("UnlockFiles/UpdateAPP");
                    StreamWriter writer = new StreamWriter("UnlockFiles/log.txt");
                    UpdateFile f = UpdateFile.Open(path, false);
                    foreach (var a in f)
                    {
                        if (!unpacked)
                        {
                            LOG(0, "Extracting", a.FileType);
                            writer.WriteLine("Extracting: " + a.FileType + " " + a.BlockSize.ToString() + " " + a.FileSequence + " " + a.DataOffset);
                            f.Extract(i, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                            if (a.FileType.ToLower().Contains("gpt"))
                            {
                                writer.WriteLine("Creating rawprogram0");
                                CreateRWProgram0xml();
                            }
                            i++;
                        }
                        if (state == 2)
                        {
                            FlashToolQClegacy.CurPartLenght = (int) a.FileSize;
                            writer.WriteLine("Flsashing: " + a.FileType + " " + a.BlockSize.ToString() + " " + a.FileSequence + " " + a.DataOffset);
                            FlashToolQClegacy.Write(a.FileType.ToLower(), loader, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                        }
                    }
                    writer.Close();
                    writer.Dispose();
                    unpacked = true;
                    return true;
                }
                LOG(0, "Searching GPT.bin in file:", path);
                UpdateFile ff = UpdateFile.Open(path, false);
                foreach (var a in ff)
                {
                    if (a.FileType.ToLower().Contains("gpt"))
                    {
                        LOG(0, "Extracting", a.FileType);
                        LOG(0, a.BlockSize.ToString(), " " + a.FileSequence + " " + a.DataOffset);
                        ff.Extract(i, "UnlockFiles/UpdateAPP/" + a.FileType.ToLower() + ".img");
                        CreateRWProgram0xml();
                        break;
                    }
                    i++;
                }
                return true;
            });
            await CurTask;
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
                var gpttable = GET_GPT_FROM_FILE(filesInDir[0].FullName, 512);
                if (gpttable.Count > 0)
                    WriteGPT_TO_XML(pathxml, gpttable);
            }
        }
    }
