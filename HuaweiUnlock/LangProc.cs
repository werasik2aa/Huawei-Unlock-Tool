using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Management;
using HuaweiUnlocker.UI;
using HuaweiUnlocker.FlashTool;
using System.Linq;
using System.Threading;
using HuaweiUnlocker.DIAGNOS;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Base62;

namespace HuaweiUnlocker
{
    public static class LangProc
    {
        public const string APP_VERSION = "15F";
        public static TextBox LOGGBOX;
        public static bool debug = false;
        public static string log, loge, newline = Environment.NewLine, PrevFolder = "c:\\";
        private static StreamWriter se = new StreamWriter("log.txt");
        public static NProgressBar PRG;
        public static TabControl Tab;
        private static Action action;
        public static Process CurProcess;
        public static IDentifyDev DeviceInfo = new IDentifyDev();
        public static Task CurTask;
        public static CancellationTokenSource ct = new CancellationTokenSource();
        public static CancellationToken token = ct.Token;
        public class Port_D
        {
            public string ComName;
            public string FullName;
        }
        public struct Partition
        {
            public string BlockStart;
            public string BlockEnd;
            public string BlockLength;
            public string BlockNumSectors;
            public string BlockBytes;
        }
        public class IDentifyDev
        {
            public string BSN = "NaN";
            public string BUILD = "NaN";
            public string VERSION = "NaN";
            public string SerialNum = "NaN";
            public string HWID = "NaN";
            public string SWID = "NaN";
            public string PK_HASH = "NaN";
            public string SBLV = "NaN";
            public string Name = "Unknown";
            public string CPUName = "Unknown";
            public Dictionary<string, Partition> Partitions = new Dictionary<string, Partition>();
            public bool loadedhose = false;
            public Port_D Port = new Port_D();
        }
        public static bool SyncRUN(string command, string subcommand)
        {
            action = () => Tab.Enabled = false;
            if (Tab.InvokeRequired)
                Tab.Invoke(action);
            else
                action();
            log = "SUCCESS";
            if (debug)
                LOG(0, command + newline + subcommand);
            Process p = CurProcess = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = subcommand;
            p.Start();
            string outtext = "";
            while ((outtext = p.StandardOutput.ReadLine()) != null)
            {
                if (outtext.ToLower().Contains("partition name:"))
                {
                    string[] partitionDATA = outtext.Split(' ');
                    Partition ps = new Partition()
                    {
                        BlockStart = partitionDATA[6],
                        BlockEnd = "" + int.Parse(partitionDATA[6]) + int.Parse(partitionDATA[10]),
                        BlockBytes = "512",
                        BlockNumSectors = partitionDATA[10],
                        BlockLength = (int.Parse(partitionDATA[10]) / 2).ToString()
                    };
                    if (debug) LOG(0, "Partition:", partitionDATA[3]);
                    DeviceInfo.Partitions.Add(partitionDATA[3], ps);
                }
                if (outtext.Contains("%") || outtext.ToLower().Contains("remaining"))
                {
                    int percent = 1;
                    if (outtext.Contains("%"))
                        percent = int.Parse(outtext.Split(' ').Last().Replace("%}", "").Split('.')[0]);
                    else if (outtext.Contains("remaining"))
                    {
                        int sR = percent = int.Parse(outtext.Split(' ')[2]);
                        int dS = FlashToolQClegacy.CurPartLenght - int.Parse(outtext.Split(' ')[2]);
                        if (FlashToolQClegacy.CurPartLenght != 0)
                            percent = (int)Math.Round((double)(100 * dS / FlashToolQClegacy.CurPartLenght));
                        if (percent <= 0) percent = 1;
                    }
                    action = () => { LOGGBOX.Text = LOGGBOX.Text.Substring(0, LOGGBOX.Text.LastIndexOf(newline)); LOG(0, Language.Get("Percent"), percent + "%"); };
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();
                    Progress(percent);
                }
                if (outtext.StartsWith("SerialNumber"))
                    DeviceInfo.SerialNum = outtext.Split(' ')[1];
                if (outtext.StartsWith("MSM_HW_ID"))
                    DeviceInfo.CPUName = IdentifyCPUbyID(DeviceInfo.HWID = outtext.Split(' ')[1]);
                if (outtext.StartsWith("OEM_PK_HASH"))
                    DeviceInfo.PK_HASH = outtext.Split(' ')[1];
                if (outtext.Contains("SBL SW Version"))
                    DeviceInfo.SBLV = outtext.Split(' ')[3];
                log = log + newline + outtext;
                if (!outtext.Contains("remaining"))
                    if (debug) LOG(0, newline + outtext);
                Thread.Sleep(5);
            }
            p.WaitForExit();
            p.Close();
            p.Dispose();
            LOG(0, "Done", DateTime.Now);
            action = () => Tab.Enabled = true;
            if (Tab.InvokeRequired)
                Tab.Invoke(action);
            else
                action();
            return !isError(log);
        }
        public static bool isError(string i)
        {
            i = i.ToLower();
            if (i.Contains("the operation completed successfully") || i.Contains("success")) return false;
            if (i.Contains("failed") || i.Contains("error") || i.Contains("error setting com port timeouts") || i.Contains("fail") || i.Contains("status: 2") || i.Contains("failed to write hello response back to device") || i.Contains("failed to open com port")) return true;
            return false;
        }
        public static bool LOG(int o, string i, object j = null, string sepa = " ")
        {
            string state = "";
            j = j == null ? "" : j;
            switch (o)
            {
                default:
                    state = "";
                    break;
                case 0:
                    state = Language.Get("Info");
                    break;
                case 1:
                    state = Language.Get("Warning");
                    break;
                case 2:
                    state = Language.Get("Error");
                    break;
            }
            i = Language.isExist(i) ? Language.Get(i) : i;
            i = i.Contains("/n") ? i.Replace("/n", newline) : i;
            Action action;
            j = Language.isExist(j.ToString()) ? Language.Get(j.ToString()) : j;
            j = j.ToString().Contains("/n") ? i.ToString().Replace("/n", newline) : j;
            if (Program.ISWINDOW)
            {
                action = () => LOGGBOX.AppendText(newline + state + i + sepa + j.ToString());
                if (LOGGBOX.InvokeRequired)
                    LOGGBOX.Invoke(action);
                else
                    action();
            }
            else Console.WriteLine(newline + state + i + sepa + j.ToString());
            se.WriteLine(LOGGBOX.Text);
            return true;
        }
        public static Port_D GETPORT(string name, string devicename = "")
        {
            Port_D req = new Port_D();
            req.ComName = "NaN";
            req.FullName = "NaN";
            if (String.IsNullOrEmpty(devicename) || devicename == "Auto")
                try
                {
                    foreach (ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"").Get())
                        if (queryObj["Name"].ToString().ToLower().Contains(name))
                        {
                            string DEVICEname = queryObj["Name"].ToString();
                            req.ComName = DEVICEname.Split(' ').Last().Replace("(", "").Replace(")", "");
                            req.FullName = DEVICEname;
                            break;
                        }
                }
                catch (Exception)
                {
                    LOG(2, "NoRights");
                }
            else
                if (!String.IsNullOrEmpty(devicename))
            {
                req.ComName = devicename.Split(' ').Last().Replace("(", "").Replace(")", "");
                req.FullName = devicename;
            }
            return req;
        }
        public static List<Port_D> GETPORTLIST()
        {
            List<Port_D> req = new List<Port_D>();
            try
            {
                foreach (ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"").Get())
                {
                    string DEVICEname = queryObj["Name"].ToString();
                    Port_D reqq = new Port_D();
                    reqq.ComName = DEVICEname.Split(' ').Last().Replace("(", "").Replace(")", "");
                    reqq.FullName = DEVICEname;
                    req.Add(reqq);
                }
            }
            catch (Exception)
            {
                LOG(2, "NoRights");
            }
            return req;
        }
        public static string PickLoader(string dev)
        {
            DeviceInfo.Name = dev;
            string pth = Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev;
            if (!Directory.Exists(pth)) return "NaN";
            foreach (var a in Directory.GetFiles(pth))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        public static string CertExtr(string SFDump)
        {
            int rootcert = 0;
            string pattern = "3082.{4}3082";
            MatchCollection matchs = Regex.Matches(SFDump, pattern);
            List<string> certs = new List<string>();
            StringBuilder SHAstr = new StringBuilder(string.Empty);
            SHA256 mysha256 = SHA256.Create();
            SHA384 rsaPSS = SHA384.Create();
            byte[] hashbytes = null;
            if (matchs.Count >= 2)
            {
                string certl = SFDump.Substring(matchs[0].Index + 4, 4);
                int certlen = int.Parse(certl, NumberStyles.HexNumber);
                if ((matchs[0].Index + certlen * 2 + 8) == matchs[1].Index)
                {
                    rootcert = 2;
                    if (matchs.Count >= 3) rootcert = 3;
                }
            }
            if (rootcert > 0)
            {
                for (int i = 0; i < rootcert; i++)
                {
                    string certl = SFDump.Substring(matchs[i].Index + 4, 4);
                    int certlen = Int32.Parse(certl, NumberStyles.HexNumber);
                    certs.Insert(i, matchs[i].Value + SFDump.Substring(matchs[i].Index + 12, certlen * 2 - 4));
                }
                Guide guide = new Guide();
                foreach (KeyValuePair<string, int> correct_SHA in guide.SHA_magic_numbers)
                {
                    if (certs[rootcert - 1].Contains(correct_SHA.Key))
                    {
                        switch (correct_SHA.Value)
                        {
                            case 0://SHA384 - старые серты
                                hashbytes = rsaPSS.ComputeHash(CRC.HexStringToBytes(certs[rootcert - 1]));
                                break;
                            case 1://SHA256 - старые серты
                                hashbytes = mysha256.ComputeHash(CRC.HexStringToBytes(certs[rootcert - 1]));
                                break;
                            case 2://SHA256 - нормальные серты
                                hashbytes = mysha256.ComputeHash(CRC.HexStringToBytes(certs[rootcert - 1]));
                                break;
                            case 3://SHA384 - новые серты
                                hashbytes = rsaPSS.ComputeHash(CRC.HexStringToBytes(certs[rootcert - 1]));
                                break;
                            case 4://SHA384 - паченый старый программер
                                hashbytes = rsaPSS.ComputeHash(CRC.HexStringToBytes(certs[rootcert - 1]));
                                break;
                            default:
                                //hashbytes = mysha256.ComputeHash(StringToByteArray(certs[rootcert - 1]));
                                hashbytes = null;
                                break;
                        }
                    }
                }
                if (hashbytes != null)
                {
                    SHAstr.Append(BitConverter.ToString(hashbytes));
                    SHAstr.Replace("-", string.Empty);
                    while (SHAstr.Length < 64) SHAstr.Insert(0, '0');
                }
            }
            return SHAstr.ToString();
        }
        public static string[] IDs(string dumpfile)
        {
            string[] certarray = new string[6] { "-", "-", "-", "-", "-", "-" };
            int HWIDstrInd = dumpfile.IndexOf("2048575F4944"); // HW_ID
            int SWIDstrInd = dumpfile.IndexOf("2053575F4944"); // SW_ID
            string HWID = "3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F"; // Для неопределённого HWID ставим ??
            string SWID = "3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F3F"; // Для неопределённого SWID ставим ??
            //Выбираем новый или старый способ поиска идентификаторов
            if (dumpfile.Length > 8600 && dumpfile.Substring(8200, 2).Equals("06")) //Новый шланг
            {
                StringBuilder hw_res = new StringBuilder(string.Empty);
                for (int i = 0; i < 4; i++)
                {
                    hw_res.Insert(0, dumpfile.Substring(8312 + i * 2, 2)); //103C, 4byte, HW_ID -идентификатор процессора
                }
                certarray[0] = hw_res.ToString();
                certarray[1] = dumpfile.Substring(8322, 2) + dumpfile.Substring(8320, 2); //1040, 2byte, OEM_ID -идентификатор OEM
                certarray[2] = dumpfile.Substring(8330, 2) + dumpfile.Substring(8328, 2); //1043 (1044 корректно), 2byte, MODEL_ID -идентификатор модели
                if (string.IsNullOrEmpty(CertExtr(dumpfile))) certarray[3] = "?"; else certarray[3] = CertExtr(dumpfile);  //хеш
                certarray[4] = dumpfile.Substring(8304, 2).TrimStart('0'); //1038, 1byte, SW_ID -идентификатор образа
                if (dumpfile.Substring(8520, 2) == "00") certarray[5] = string.Empty;
                else certarray[5] = "(" + dumpfile.Substring(8520, 2).TrimStart('0') + ")"; //10A4, 1byte, SW_VER -версия образа
            }
            else //Старый шланг 5 или 3 или ещё что-то
            {
                if (HWIDstrInd >= 32 && SWIDstrInd >= 32)
                {
                    HWID = dumpfile.Substring(HWIDstrInd - 32, 32);
                    SWID = dumpfile.Substring(SWIDstrInd - 32, 32);
                }
                for (int i = 0; i < certarray.Length; i++)
                {
                    switch (i)
                    {
                        case 0: // Вытягиваем процессор
                            string[] HStr = new string[8];
                            int counth = 0;
                            for (int j = 0; j < 16; j += 2)
                            {
                                HStr[counth] = Convert.ToString((char)int.Parse(HWID.Substring(j, 2), NumberStyles.HexNumber));
                                counth++;
                            }
                            certarray[i] = string.Join(string.Empty, HStr);
                            break;
                        case 1: // Вытягиваем производителя
                            string[] OStr = new string[4];
                            int counto = 0;
                            for (int j = 16; j < 24; j += 2)
                            {
                                OStr[counto] = Convert.ToString((char)int.Parse(HWID.Substring(j, 2), NumberStyles.HexNumber));
                                counto++;
                            }
                            certarray[i] = string.Join(string.Empty, OStr);
                            break;
                        case 2: // Вытягиваем номер модели
                            string[] MStr = new string[4];
                            int countm = 0;
                            for (int j = 24; j < 32; j += 2)
                            {
                                MStr[countm] = Convert.ToString((char)int.Parse(HWID.Substring(j, 2), NumberStyles.HexNumber));
                                countm++;
                            }
                            certarray[i] = string.Join(string.Empty, MStr);
                            break;
                        case 3: // Расчитываем хеш
                            if (string.IsNullOrEmpty(CertExtr(dumpfile))) certarray[i] = "?"; else certarray[i] = CertExtr(dumpfile);
                            break;
                        case 4: // Формируем тип софтвера
                            string[] SNStr = new string[8];
                            int countn = 0;
                            for (int j = 16; j < 32; j += 2)
                            {
                                SNStr[countn] = Convert.ToString((char)int.Parse(SWID.Substring(j, 2), NumberStyles.HexNumber));
                                countn++;
                            }
                            string nstr = string.Join(string.Empty, SNStr);
                            string nend;
                            switch (nstr)
                            {
                                case "????????":
                                    nend = "?";
                                    break;
                                case "00000000":
                                    nend = "0";
                                    break;
                                default:
                                    nend = nstr.TrimStart('0');
                                    break;
                            }
                            certarray[i] = nend;
                            break;
                        case 5: //  Формируем версию софтвера
                            string[] SWStr = new string[8];
                            int countv = 0;
                            for (int j = 0; j < 16; j += 2)
                            {
                                SWStr[countv] = Convert.ToString((char)int.Parse(SWID.Substring(j, 2), NumberStyles.HexNumber));
                                countv++;
                            }
                            string verstr = string.Join(string.Empty, SWStr);
                            string verend;
                            switch (verstr)
                            {
                                case "????????":
                                    verend = string.Empty;
                                    break;
                                case "00000000":
                                    verend = string.Empty;
                                    break;
                                default:
                                    verend = "(" + verstr.TrimStart('0') + ")";
                                    break;
                            }
                            certarray[i] = verend;
                            break;
                        default:
                            break;
                    }
                }
            }
            return certarray;
        }
        public static Dictionary<string, Partition> GET_GPT_FROM_FILE(string GPT_File, int block_size)
        {
            Dictionary<string, Partition> GPT = new Dictionary<string, Partition>();
            DataS.GPT_Struct magic_number = new DataS.GPT_Struct(0x00, 8, string.Empty);
            DataS.GPT_Struct gpt_startadress = new DataS.GPT_Struct(0x48, 8, string.Empty);
            DataS.GPT_Struct max_gpt_blocks = new DataS.GPT_Struct(0x50, 4, string.Empty);
            DataS.GPT_Struct record_length = new DataS.GPT_Struct(0x54, 4, string.Empty);
            string Full_GPT = BitConverter.ToString(File.ReadAllBytes(GPT_File)).Replace("-", "");
            string GPT_Header = Full_GPT.Remove(0, block_size * 2);
            string gpt_header = GPT_Header.Remove(block_size * 2);
            magic_number.ValueString = gpt_header.Substring(magic_number.StartAdress * 2, magic_number.Length * 2);
            if (!magic_number.ValueString.Equals("4546492050415254"))
            {
                LOG(2, "THIS FILE IS NOT a gpt_####0.bin");
                return GPT;
            }
            gpt_startadress.ValueString = gpt_header.Substring(gpt_startadress.StartAdress * 2, gpt_startadress.Length * 2);
            string gsa = gpt_startadress.ValueString;
            while (gsa.EndsWith("00"))
                gsa = gsa.Remove(gsa.Length - 2, 2);
            gsa = gsa.TrimStart('0');
            max_gpt_blocks.ValueString = gpt_header.Substring(max_gpt_blocks.StartAdress * 2, max_gpt_blocks.Length * 2);
            string mgb = string.Empty;
            for (int i = 0; i < max_gpt_blocks.Length; i++)
                mgb = mgb.Insert(0, max_gpt_blocks.ValueString.Substring(i * 2, 2));
            mgb = mgb.TrimStart('0');
            record_length.ValueString = gpt_header.Substring(record_length.StartAdress * 2, record_length.Length * 2);
            string rl = record_length.ValueString;
            while (rl.EndsWith("00"))
                rl = rl.Remove(rl.Length - 2, 2);
            rl = rl.TrimStart('0');
            int rlint = Convert.ToInt32(rl, 16);
            string GPT_Values = Full_GPT.Remove(0, block_size * 2 * Convert.ToInt32(gsa, 16));
            DataS.GPT_Struct block_startadress = new DataS.GPT_Struct(0x20, 8, string.Empty);
            DataS.GPT_Struct block_endadress = new DataS.GPT_Struct(0x28, 8, string.Empty);
            DataS.GPT_Struct block_name = new DataS.GPT_Struct(0x38, 72, string.Empty);
            string[] blocks_array = new string[Convert.ToInt32(mgb, 16)];
            for (int i = 0; i < blocks_array.Length; i++)
                blocks_array[i] = GPT_Values.Substring(i * rlint * 2, rlint * 2);
            foreach (string block_string in blocks_array)
            {
                block_startadress.ValueString = block_string.Substring(block_startadress.StartAdress * 2, block_startadress.Length * 2);
                string bsa = string.Empty;
                for (int k = 0; k < block_startadress.Length; k++)
                    bsa = bsa.Insert(0, block_startadress.ValueString.Substring(k * 2, 2));
                block_endadress.ValueString = block_string.Substring(block_endadress.StartAdress * 2, block_endadress.Length * 2);
                string bea = string.Empty;
                for (int m = 0; m < block_endadress.Length; m++)
                    bea = bea.Insert(0, block_endadress.ValueString.Substring(m * 2, 2));
                block_name.ValueString = block_string.Substring(block_name.StartAdress * 2, block_name.Length * 2);
                StringBuilder bn = new StringBuilder();
                for (int p = 0; p < block_name.Length; p += 4)
                {
                    string unichar = (block_name.ValueString.Substring(p + 2, 2) + block_name.ValueString.Substring(p, 2)).TrimStart('0');
                    if (!string.IsNullOrEmpty(unichar)) bn.Append(Convert.ToChar(Convert.ToInt32(unichar, 16)));
                }
                Partition GPT_Items = new Partition()
                {
                    BlockStart = Convert.ToInt32(bsa.ToString(), 16).ToString(),
                    BlockEnd = Convert.ToInt32(bea.ToString(), 16).ToString()
                };
                if (!string.IsNullOrEmpty(GPT_Items.BlockStart) && !string.IsNullOrEmpty(GPT_Items.BlockEnd))
                {
                    uint blocks_count = Convert.ToUInt32(GPT_Items.BlockEnd, 16) - Convert.ToUInt32(GPT_Items.BlockStart, 16) + 1;
                    GPT_Items.BlockBytes = (blocks_count * block_size).ToString();
                    GPT_Items.BlockNumSectors = blocks_count.ToString();
                    GPT_Items.BlockLength = (blocks_count / 2).ToString();
                    GPT.Add(bn.ToString(), GPT_Items);
                }
            }
            return GPT;
        }
        public static void ClearLog()
        {
            LOGGBOX.Text = "";
            LOGGBOX.Copy();
        }
        public static bool CheckDevice(string path, string DeviceName = "")
        {
            LOGGBOX.Text = "";
            LOG(0, "CheckCon");
            DeviceInfo.Port = GETPORT("qdloader 9008", DeviceName);
            if (DeviceInfo.Port.ComName == "NaN" || DeviceInfo.Port.FullName == "NaN")
            {
                LOG(2, "DeviceNotCon");
                DeviceInfo.loadedhose = false;
            }
            else if (!path.Contains(":\\") && !DeviceInfo.loadedhose)
                LOG(2, "ErrLdr");
            else if (!File.Exists(path) && !DeviceInfo.loadedhose)
                LOG(2, "ErrLdr");
            else
                return true;
            return false;
        }
        public static string IdentifyCPUbyID(string id)
        {
            switch (id)
            {
                case "0x009690e1":
                    return "MSM8992";
                case "0x009600e1":
                    return "MSM8909";
                case "0x000460e1":
                    return "MSM8953";
                case "0x0091b0e1":
                    return "MSM8929";
                case "0x006220e1":
                    return "MSM7227A";
                case "0x009470e1":
                    return "MSM8996";
                case "0x009900e1":
                    return "MSM8976";
                case "0x009b00e1":
                    return "MSM8976";
                case "0X008A30E1":
                    return "MSM8930";
                case "0x0004f0e1":
                    return "MSM8937";
                case "0x0090b0e1":
                    return "MSM8936";
                case "0x009180e1":
                    return "MSM8928";
                case "0x008140e1":
                    return "MSM8x10";
                case "0x008050e2":
                    return "MSM8926";
                case "0x0005f0e1":
                    return "MSM8996";
                case "0x007B80E1":
                    return "MSM8974";
                case "0x009400e1":
                    return "MSM8994";
                case "0x008150e1":
                    return "MSM8x10";
                case "0x008050e1":
                    return "MSM8926";
                case "0x000560e1":
                    return "MSM8917";
                case "0x007050e1":
                    return "MSM8916";
                case "0x008110e1":
                    return "MSM8210";
                default:
                    return "Unknown";
            }
        }
        public static bool WriteGPT_TO_XML(string papthto, Dictionary<string, Partition> partbI)
        {
            StreamWriter writer = new StreamWriter(papthto);
            writer.WriteLine("<?xml version=\"1.0\" ?>");
            writer.WriteLine("<data>");
            foreach (var i in partbI)
            {
                if (string.IsNullOrEmpty(i.Key)) continue;
                string line = "<program SECTOR_SIZE_IN_BYTES=\"512\" file_sector_offset=\"0\" filename=" + '"' + i.Key + ".img" + '"' + " label=" + '"' + i.Key + '"' + " num_partition_sectors=" + '"' + i.Value.BlockNumSectors + '"' + " size_in_KB=" + '"' + i.Value.BlockLength + '"' + " sparse=\"false\" start_sector=" + '"' + i.Value.BlockStart + '"' + "/>";
                writer.WriteLine(line);
                if (debug) LOG(0, line);
            }
            writer.WriteLine("</data>");
            LOG(0, "-==>" + DeviceInfo.Name + "<==-");
            LOG(0, "RrGPTXMLS", papthto);
            writer.Close();
            writer.Dispose();
            return partbI.Count > 0;
        }
        public static void Progress(int v)
        {
            action = () => PRG.Value = v;
            if (PRG.InvokeRequired)
                PRG.Invoke(action);
            else
                action();
        }
    }
}