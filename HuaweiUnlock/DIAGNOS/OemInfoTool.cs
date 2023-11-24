using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.DIAGNOS
{
    public class OemInfoTool
    {
        public static List<string> data = new List<string>();
        public static List<int> Offsets = new List<int>();
        private static byte[] FileAll;
        public static void Decompile(string path, int counttoread, string Header = "4F454D5F494E464F06")
        {
            //UNCORRECT
            Offsets.Clear();
            data.Clear();
            if (Directory.Exists("UnlockFiles/OemInfoData")) Directory.Delete("UnlockFiles/OemInfoData", true);
            Directory.CreateDirectory("UnlockFiles/OemInfoData");
            LOG(0, "Reading File...");
            FileAll = File.ReadAllBytes(path);
            LOG(0, "Length: " + FileAll.Length);

            LOG(0, "Reading offsets");
            Offsets = GetOffsets(FileAll, Header);
            for (var i = 0; i < Offsets.Count; i++)
                data.Add("OEM_INFO_" + Offsets[i] + ".header");
            File.WriteAllBytes("UnlockFiles/OemInfoData/OEM_INFO_" + 0 + ".header", FileAll.Take(Offsets[0]).ToArray());
            for (int i = 0; i < Offsets.Count; i++) {
                int curof = Offsets[i];
                LOG(0, "Writting: OEM_INFO_" + curof + ".header");
                if (Offsets.Count > i + 1)
                    File.WriteAllBytes("UnlockFiles/OemInfoData/OEM_INFO_" + curof + ".header", FileAll.Skip(curof).Take(Offsets[i + 1] - curof).ToArray());
                else
                    File.WriteAllBytes("UnlockFiles/OemInfoData/OEM_INFO_" + curof + ".header", FileAll.Skip(curof).Take(FileAll.Length - curof).ToArray());
            }
        }
        public static List<int> GetOffsets(byte[] file, string pattr)
        {
            List<int> mdata = new List<int>();
            var piza = CRC.HexStringToBytes(pattr);
            var len = piza.Length;
            var limit = file.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                int k = 0;
                for (; k < len; k++)
                    if (piza[k] != file[i + k]) break;
                if (k == len)
                {
                    LOG(0, "Reading: OEM_INFO_" + i + ".header");
                    mdata.Add(i);
                }
            }
            return mdata;
        }
        public static void Compile(string intpath, string outpath)
        {
            //NOT WORK 
            FileStream outfile = new FileStream(outpath, FileMode.OpenOrCreate);
            outfile.Seek(0, SeekOrigin.Begin);
            byte[] filebts = File.ReadAllBytes(intpath + "/OEM_INFO_0.header");
            outfile.Write(filebts, 0, filebts.Length);
            foreach (var aoffset in Offsets)
            {
                LOG(0, "Adding: OEM_INFO_" + aoffset + ".header");
                filebts = File.ReadAllBytes(intpath + "/OEM_INFO_" + aoffset + ".header");
                outfile.Write(filebts, 0, filebts.Length);
            }
            outfile.Close();
            outfile.Dispose();
        }
    }
}
