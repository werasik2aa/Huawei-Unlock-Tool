using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static HuaweiUnlocker.DIAGNOS.DataS;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.DIAGNOS
{
    public class OemInfoTool
    {
        public static List<string> data = new List<string>();
        public static List<int> Offsets = new List<int>();
        private static byte[] FileAll;
        public static List<OemInfoHdr> DevStats = new List<OemInfoHdr>();
        public struct dd
        {
            Int64 Magic;
            IntPtr Version;
            IntPtr id;
            IntPtr Type;
            IntPtr DataLen;
            IntPtr Age;
        }
        public static void Decompile(string path, string Header = "4F454D5F494E464F")
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

            if (Offsets.Count == 0) return;

            for (var i = 0; i < Offsets.Count; i++)
                data.Add("OEM_INFO_" + Offsets[i] + ".header");
            File.WriteAllBytes("UnlockFiles/OemInfoData/OEM_INFO_" + 0 + ".header", FileAll.Take(Offsets[0]).ToArray());
            for (int i = 0; i < Offsets.Count; i++)
            {
                int curoffset = Offsets[i];
                var datalen = (Offsets.Count > i + 1 ? Offsets[i + 1] : FileAll.Length) - curoffset;
                var PIZA = FileAll.Skip(curoffset).Take(datalen).ToArray();
                LOG(0, "Writting: OEM_INFO_" + curoffset + ".header");
                File.WriteAllBytes("UnlockFiles/OemInfoData/OEM_INFO_" + curoffset + ".header", PIZA);
                var MAGIC = PIZA.Take(28).ToArray();
                var DataSP = PIZA.Skip(8).ToArray();
                DevStats.Add(new OemInfoHdr()
                {
                    Magic = MAGIC,
                    Offset = curoffset,
                    DataLenPage = datalen - 28,
                    Version = BitConverter.ToUInt32(DataSP.Take(4).ToArray(), 0),
                    ID = BitConverter.ToUInt32(DataSP.Skip(4).Take(4).ToArray(), 0),
                    Type = BitConverter.ToUInt32(DataSP.Skip(8).Take(4).ToArray(), 0),
                    DataLenOem = BitConverter.ToUInt32(DataSP.Skip(12).Take(4).ToArray(), 0),
                    Age = BitConverter.ToUInt32(DataSP.Skip(16).Take(4).ToArray(), 0),
                });
            }
            foreach (var a in DevStats)
            {
                var data = FileAll.Skip(a.Offset + 28 + 484).Take((int)a.DataLenOem).ToArray();
                if (a.DataLenOem <= 1024)
                {
                    LOG(0, a.ID.ToString());
                    LOG(0, "DATA:", Encoding.ASCII.GetString(data));
                }
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

            //ТУТ нужно было все файлы из папки в .img 
            //а я вот такую поеботу сделал 0_)
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("UnlockFiles/OemInfoData/");
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*.header");
            foreach (var dfile in filesInDir)
            {
                byte[] filebts = File.ReadAllBytes(dfile.FullName);
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
}
