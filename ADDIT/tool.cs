using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Management;
using System.Linq;

namespace HuaweiUnlocker
{
    public static class tool
    {
        public static TextBox LOGGBOX, PORTBOX;
        public static bool loadedhose = false;
        public static bool debug = false;
        public static string port = "NaN", log;
        private static StreamWriter se;
        private static string newline = Environment.NewLine;
        public static ProgressBar progr;
        public static Unlock Un;
        public static FlashTool Fl;
        public static QXDterminal QXD;
        private static int cur;
        public static Dictionary<string, int[]> GPTTABLE = new Dictionary<string, int[]>();
        private static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static bool LoadLoader(string device, string pathloader)
        {
                        string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + pathloader + '"';
            if (debug) { LOG("===FLASHLOADER===" + newline + newline + command + newline + subcommand); }
            try
            {
                LOG(I("FireHose") + device);
                var state = RUN(command, subcommand, false, false);
                loadedhose = state;
                progr.Value = 20;
                return state;
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool Unlock(string device, string loader, string path)
        {
                        string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + port + " --sendxml=" + '"' + path + "\\rawprogram0.xml" + '"' + " --search_path=" + path;
            if (debug) { LOG("===UNLOCKER===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 40;
                LOG(I("Unlocker") + device);
                return RUN(command, subcommand, false, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool Write(string partition, string loader, string path)
        {
                        progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -b " + partition + " "+'"'+ path + '"';
            if (debug) { LOG("===WRITE PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 50;
                LOG(I("Writer") + partition);
                return RUN(command, subcommand, false, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool Erase(string partition, string loader)
        {
                        progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -e " + partition;
            if (debug) { LOG("===ERASE PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 50;
                LOG(I("Eraser") + partition);
                return RUN(command, subcommand, false, true);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool ReadGPT(string loader)
        {
                        string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -gpt";
            try
            {
                if (GPTTABLE.Count == 0 || !loadedhose)
                {
                    GPTTABLE = new Dictionary<string, int[]>();
                    if (debug) { LOG("===UNLOCKER FRP===" + newline + newline + command + newline + subcommand); }
                    if (!loadedhose)
                        if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG(E("Fail")); return false; }
                        else loadedhose = true;
                    return RUN(command, subcommand, true, false);
                }
            }
            catch { }
            return true;
        }
        public static bool UnlockFrp(string loader)
        {
                        progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -e frp"; 
            if (debug) { LOG("===UNLOCK FRP===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG(E("Fail")); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 30;
                LOG(I("Unlocker") + "FRP: Universal" + newline + "GETGPT: INIT!");
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(E("FailFrpD")); return false; }
                progr.Value = 50;
                LOG(I("Eraser") + "FRP" + newline);
                return RUN(command, subcommand, false, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool FlashPartsXml(string xml, string loader, string path)
        {
                        progr.Value = 2;
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + port + " --sendxml=" + '"' + xml + '"' + " --search_path=" + '"' + path + '"';
            if (debug) { LOG("===Flash Partitions XML===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG("ERROR: Device failed to load loader or port occupied"); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 0;
                LOG(I("Flasher") + path);
                return RUN(command, subcommand, false, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool FlashPartsRaw(string loader, string file)
        {
                        progr.Value = 2;
            string command = "Tools\\fh_loader.exe";
            string subcommand = " --port=\\\\.\\" + port + " --sendimage=" + '"' + file + '"' + " --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=eMMC";
            if (debug) { LOG("===Flash Partitions RAW===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG("ERROR: Device failed to load loader or port occupied"); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 0;
                LOG("INFO: Flashing EMMC IMAGE: " + file);
                return RUN(command, subcommand, false, true);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool Dump(string loader, string savepath)
        {
                        progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -d 0 " + GPTTABLE["userdata"][0] + " -o " + '"'+ savepath + '"';
            if (debug) { LOG("===DUMPER===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG(E("Fail")); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 30;
                LOG(I("DumpTr") + newline);
                if (GPTTABLE.Count == 0) 
                    if(!ReadGPT(loader)) 
                    { LOG(E("Unknown")); return false; }
                                        LOG(I("Dump") + "0" + "<-TO->" + GPTTABLE["userdata"][0]);
                cur = GPTTABLE["userdata"][1];
                return RUN(command, subcommand, false, true);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
            }
            return false;
        }
        public static bool Dump(int i, int o, string partition,  string loader, string savepath)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -d " + i + " " + o + " -o " + '"' + savepath + '"' + "\\"+partition;
            if (debug) { LOG("===DUMPER PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG(E("Fail")); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 30;
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(E("Unknown")); return false; }
                                LOG(I("DumpTp") + partition + newline);
                cur = GPTTABLE[partition][1];
                return RUN(command, subcommand, false, true);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
            }
            return false;
        }
        private static bool isError(string i)
        {
            log = "";
            i = i.ToLower();
            if (i.Contains("the operation completed successfully") || i.Contains("success")) return false;
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2")) return true;
            return false;
        }
        public static void LOG(string i)
        {
            if (PORTBOX == null || LOGGBOX == null) return;
            LOGGBOX.AppendText((newline + i));

            se = new StreamWriter("log.txt");
            se.WriteLine(LOGGBOX.Text);
            se.Close();
        }
        public static void PORTFIND()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string DEVICEname = queryObj["Name"].ToString();
                    string[] DevideName = DEVICEname.Split(' ');
                    port = DevideName[DevideName.Length - 1].Replace("(", "").Replace(")", "");
                    if (DEVICEname.ToLower().Contains("qdloader 9008"))
                    {
                        if (port != PORTBOX.Text)
                        {
                            LOG("DEVICE: " + DEVICEname);
                            PORTBOX.Text = port;
                            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
                            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
                        }
                        return;
                    }
                }
                PORTBOX.Text = "Connect Your device";
                loadedhose = false;
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
                foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            }
            catch (ManagementException e)
            {
            }
        }
        public static string PickLoader(string dev)
        {
            foreach (var a in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        private static bool RUN(string command, string subcommand, bool getgpt, bool bysectors)
        {
            Process p = new Process();
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
                if (!PORTBOX.Text.Contains("COM")) return true;
                if (getgpt && outtext.ToLower().Contains("partition name:"))
                {
                    string[] partitionDATA = outtext.Split(' ');
                    int[] partitionlbl = new int[2];
                    partitionlbl[0] = int.Parse(partitionDATA[6]);
                    partitionlbl[1] = int.Parse(partitionDATA[10]);
                    GPTTABLE.Add(partitionDATA[3].ToLower(), partitionlbl);
                }
                if (bysectors)
                {
                    if (outtext.Contains("%"))
                    {
                        string a = outtext.Split(' ')[outtext.Split(' ').Length - 1].Replace("%}", "");
                        LOGGBOX.AppendText((newline + "INFO: Percent:-") + a + "%");
                        int percent = int.Parse(a.Split('.')[0]);
                        progr.Value = percent;
                    }
                    if (outtext.ToLower().Contains("remain"))
                    {
                        int sectors = int.Parse(outtext.Split(' ')[2]);
                        int percent = (int)Math.Round((double)(100 * (cur - sectors)) / cur);
                        if (percent < 0) percent = 1;
                        progr.Value = percent;
                    }
                }

                log = log + newline + outtext;
                if (debug) LOGGBOX.AppendText((newline + outtext));
            }
            return !isError(log);
        }

            public static void readlng()
        {
            Stream ss = File.OpenRead("lang.ini");
            StreamReader readerL = new StreamReader(ss);
            string line = readerL.ReadLine();
            while ((line = readerL.ReadLine()) != null)
                lang.Add(line.Split(',')[0], line.Split(',')[1]);
            readerL.Close();
        }
        public static void all()
        {
            if (Un != null) Un.Enabled = !Un.Enabled;
            if (Fl != null) Fl.Enabled = !Fl.Enabled;
        }
        public static string I(string data)
        {
            return (lang["Info"] + " " + lang[data]).Replace("/n", Environment.NewLine);
        }
        public static string E(string data)
        {
            return (lang["Error"] + " " + lang[data]).Replace("/n", Environment.NewLine);
        }
        public static string L(string data)
        {
            return lang[data].Replace("/n", Environment.NewLine);
        }
        public static bool CheckDevice(ComboBox Ld)
        {
            tool.LOG(tool.I("CheckCon"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            if (!tool.port.StartsWith("COM"))
            {
                tool.LOG(tool.E("DeviceNotCon"));
                return false;
            }
            if (Ld.Text.Length < 5)
            {
                tool.LOG(tool.E("ErrLdr"));
                return false;
            }
            return true;
        }
    }
}