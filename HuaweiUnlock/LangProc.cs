using HuaweiUnlocker.DIAGNOS;
using HuaweiUnlocker.FlashTool;
using HuaweiUnlocker.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuaweiUnlocker
{
    public static class LangProc
    {
        public const string APP_VERSION = "29F";
        public static TextBox LOGGBOX;
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
        public static Window wndw;
        public static bool debug = false;
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
                    DeviceInfo.CPUName = DataS.IdentifyCPUbyID(DeviceInfo.HWID = outtext.Split(' ')[1]);
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
        public static bool LOG(int o, object i, object j = null, string sepa = " ")
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
            //GET STRING LANGS
            i = Language.isExist(i.ToString()) ? Language.Get(i.ToString()) : i;
            i = i.ToString().Contains("/n") ? i.ToString() : i;

            j = Language.isExist(j.ToString()) ? Language.Get(j.ToString()) : j;
            j = j.ToString().Contains("/n") ? i.ToString() : j;

            try
            {
                i = string.Join("", Regex.Split((string)i, @"(?:\r\n|\n|\r)"));
                j = string.Join("", Regex.Split((string)j, @"(?:\r\n|\n|\r)"));
            }
            catch
            {

            }
            action = () => LOGGBOX.AppendText((newline + state + i + sepa + j.ToString()).Replace("/n", newline).Replace("\n", newline));
            if (LOGGBOX.InvokeRequired)
                LOGGBOX.Invoke(action);
            else
                action();
            se.WriteLine(newline + state + i + sepa + j.ToString());
            return true;
        }
        //GET PORTS
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
        //ALL LIST OF COM PORTS
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

        //GetLoader by model name
        public static string PickLoader(string dev)
        {
            DeviceInfo.Name = dev;
            string pth = Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev;
            if (!Directory.Exists(pth)) return "NaN";
            foreach (var a in Directory.GetFiles(pth))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }

        //GPT READER
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
            if (!magic_number.ValueString.Equals("4546492050415254")) //HEADER OF GPT FILE
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
                if (!string.IsNullOrEmpty(bsa) && !string.IsNullOrEmpty(bea))
                {
                    uint blocks_count = Convert.ToUInt32(bea, 16) - Convert.ToUInt32(bsa, 16) + 1;
                    if (!GPT.ContainsKey(bn.ToString()) & !bn.ToString().Contains("userdata") & !string.IsNullOrEmpty(bn.ToString()))
                        GPT.Add(bn.ToString().Replace(".img", ""), new Partition()
                        {
                            BlockStart = Convert.ToInt32(bsa, 16).ToString(),
                            BlockEnd = Convert.ToInt32(bea, 16).ToString(),
                            BlockBytes = (blocks_count * block_size).ToString(),
                            BlockNumSectors = blocks_count.ToString(),
                            BlockLength = (blocks_count / 2).ToString(),
                        });
                }
            }
            return GPT;
        }
        public static void ClearLog()
        {
            LOGGBOX.Text = "";
            LOGGBOX.Copy();
        }
        //IT'S CAN BE SIMPLE. BUT WHO I AM FOR DO THE SIMPLE?
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
            else
            {
                if (DeviceInfo.loadedhose) return true;
                if (wndw.AutoLdr.Checked & DeviceInfo.HWID.Contains("NaN"))
                {
                    DeviceInfo.Name = "NaN";
                    DeviceInfo.Port = GETPORT("qdloader 9008", "Auto");
                    FlashToolQClegacy.GetIdentifier();
                    LOG(0, "LoaderSearch");
                    var ambn = GuessMbn();
                    if (!string.IsNullOrEmpty(ambn))
                    {
                        if (ambn == "True")
                            return false;
                        DeviceInfo.Name = ambn.Split('\\')[1];
                        return true;
                    }
                    else return !LOG(0, "NoDEVICEAnsw");

                }
                if (!File.Exists(path) && !DeviceInfo.loadedhose & !wndw.AutoLdr.Checked)
                    LOG(2, "ErrLdr", path);
                else
                    return true;
            }
            return false;
        }
        //MAYBE NEED STARTBYTE in HEX ...
        public static bool WriteGPT_TO_XML(string papthto, Dictionary<string, Partition> partbI, bool verify)
        {
            StreamWriter writer = new StreamWriter(papthto);
            writer.WriteLine("<?xml version=\"1.0\" ?>");
            writer.WriteLine("<data>");
            writer.WriteLine("  <!--NOTE: This is an ** Autogenerated file **-->");
            writer.WriteLine("  <!--NOTE: HUT_HUAWEI UNLOCK TOOL **-->");

            foreach (var i in partbI)
            {
                if (string.IsNullOrEmpty(i.Key)) continue;
                if (verify && !File.Exists("UnlockFiles/UpdateAPP/" + i.Key + ".img")) continue;
                string line = "  <program SECTOR_SIZE_IN_BYTES=\"512\" file_sector_offset=\"0\" filename=\"" + i.Key + ".img\"" + " label=\"" + i.Key + "\" num_partition_sectors=\"" + i.Value.BlockNumSectors + "\" physical_partition_number=\"0\" size_in_KB=\"" + i.Value.BlockLength + "\" sparse=\"false\" start_byte_hex=\"" + string.Format("0x{0:x16}", i.Value.BlockStart) + "\" start_sector=\"" + i.Value.BlockStart + "\" />";
                if (i.Key.ToLower() == "userdata")
                    line = "  <program SECTOR_SIZE_IN_BYTES=\"512\" file_sector_offset=\"0\" filename=\"" + i.Key + ".img\"" + " label=\"" + i.Key + "\" num_partition_sectors=\"" + 1 + "\" physical_partition_number=\"0\" size_in_KB=\"" + i.Value.BlockLength + "\" sparse=\"false\" start_byte_hex=\"" + string.Format("0x{0:x16}", i.Value.BlockStart) + "\" start_sector=\"" + i.Value.BlockStart + "\" />";
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
        public static void Progress(int v, int max = 100)
        {
            action = () => { PRG.Value = v; PRG.ValueMaximum = max; };
            if (PRG.InvokeRequired)
                PRG.Invoke(action);
            else
                action();
            Application.DoEvents();
        }
        public static string GuessMbn()
        {
            if (!string.IsNullOrEmpty(wndw.LoaderBox.Text))
                return PickLoader(wndw.LoaderBox.Text);
            if (wndw.AutoLdr.Checked & DeviceInfo.HWID.Contains("NaN"))
                return LOG(0, "NoDEVICEAnsw").ToString();
            if (debug) LOG(0, "LoaderSearch");
            string[] subdirectoryEntries = Directory.GetDirectories("qc_boot");
            foreach (string subdirectory in subdirectoryEntries)
            {
                var a = Directory.GetFiles(subdirectory).First();
                var b = File.ReadAllBytes(a);
                if (Encoding.ASCII.GetString(b).ToLower().Contains(DeviceInfo.HWID.Replace("0x", "")))
                {
                    if (debug) LOG(0, "LoaderFound", a);
                    return a;
                }
            }
            return "";
        }
        public static string GuessMbnTest()
        {
            if (!string.IsNullOrEmpty(wndw.LoaderBox.Text))
                return PickLoader(wndw.LoaderBox.Text);
            if (wndw.AutoLdr.Checked & DeviceInfo.HWID.Contains("NaN"))
                return LOG(0, "NoDEVICEAnsw").ToString();
            if (debug) LOG(0, "LoaderSearch");
            string any = "";
            string[] subdirectoryEntries = Directory.GetDirectories("qc_boot");
            foreach (string subdirectory in subdirectoryEntries)
            {
                var a = Directory.GetFiles(subdirectory).First();
                var b = File.ReadAllBytes(a);
                if (Encoding.ASCII.GetString(b).ToLower().Contains(DeviceInfo.HWID.Replace("0x", "")))
                    LOG(0, "LoaderFound", any = a);
            }
            return any;
        }
        public static byte[] GetResource(string ResourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
        public static void SaveResources(string ResourceName, string SavePath, string SaveName = "")
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
            string myspace = typeof(LangProc).Namespace;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(myspace + "." + ResourceName);
            FileStream filewritter = new FileStream(SavePath + "\\" + (string.IsNullOrEmpty(SaveName) ? ResourceName : SaveName), FileMode.CreateNew);
            for (int i = 0; i < stream.Length; i++) filewritter.WriteByte((byte)stream.ReadByte());
            filewritter.Close();
            filewritter.Dispose();
        }
    }
}