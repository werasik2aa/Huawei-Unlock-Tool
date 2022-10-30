using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace HuaweiUnlocker
{
    public static class tool
    {
        public static TextBox LOGGE, PORTER;
        public static bool loadedhose = false;
        public static bool debug = false;
        public static bool error;
        public static string port = "NaN";
        private static List<string> notdevice = new List<string>();
        private static List<string> current = new List<string>();
        private static bool first = true;
        public static StreamWriter se;
        public static bool getgpt = false;
        private static string newline = Environment.NewLine;
        public static ProgressBar progr;
        private static string log, EndStor, DevPart, FrpPart, KernelPart;
        public static Unlock Un;
        public static FlashTool Fl;
        public static QXDterminal QXD;
        private static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static bool LoadLoader(string device, string pathloader)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + pathloader + '"';
            if (debug) { LOG("===FLASHLOADER===" + newline + newline + command + newline + subcommand); }
            try
            {
                LOG(I("FireHose") + device);
                var state = RUN(command, subcommand, false);
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
                return RUN(command, subcommand, false);
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
                return RUN(command, subcommand, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool Flash(string partition, string loader, string file)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -b " + partition + " " + '"' + file + '"';
            if (debug) { LOG("===FLASH PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 50;
                LOG(I("Eraser") + partition);
                return RUN(command, subcommand, false);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool UnlockFrp(string loader)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -gpt";
            if (debug) { LOG("===UNLOCKER FRP===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.getgpt = false; tool.LOG(E("Fail")); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 30;
                getgpt = true;
                LOG(I("Unlocker") +"FRP: Universal" + newline + "GETGPT: " + getgpt);
                var a = RUN(command, subcommand, false);
                if (!a) { getgpt = false; tool.error = true; LOGGE.AppendText(newline + E("FailFrpD")); return true; }
                progr.Value = 50;
                command = "Tools\\emmcdl.exe";
                subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -e " + FrpPart;
                LOGGE.AppendText(newline + I("Eraser") + FrpPart + newline);
                return RUN(command, subcommand, false);
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
            if (debug) { LOG("===Flash Partitions===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG("ERROR: Device failed to load loader or port occupied"); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 0;
                LOG(I("Flasher") + path);
                return RUN(command, subcommand, false);
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
            progr.Maximum = 100;
            string command = "Tools\\fh_loader.exe";
            string subcommand = " --port=\\\\.\\" + port + " --sendimage=" + '"' + file + '"' + " --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=eMMC";
            if (debug) { LOG("===Flash Partitions===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; tool.LOG("ERROR: Device failed to load loader or port occupied"); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 0;
                LOG("INFO: Flashing EMMC IMAGE: " + file);
                return RUN(command, subcommand, true);
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
            string subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -gpt";
            if (debug) { LOG("===UNLOCKER FRP===" + newline + newline + command + newline + subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { tool.getgpt = false; loadedhose = false; tool.LOG(E("Fail")); return false; }
                else loadedhose = true;
            try
            {
                progr.Value = 30;
                LOG(I("DumpTr") + newline);
                getgpt = true;
                var a = RUN(command, subcommand, false);
                if (!a) { getgpt = false; tool.error = true; LOGGE.AppendText(newline + E("Unknown")); return true; }
                progr.Maximum = int.Parse(EndStor);
                progr.Value = int.Parse(EndStor);
                command = "Tools\\emmcdl.exe";
                subcommand = "-p " + port + " -f " + '"' + loader + '"' + " -d 0 " + EndStor + " -o " + savepath;
                LOGGE.AppendText(newline + I("Dump") + "0" + "<-TO->" + EndStor);
                return RUN(command, subcommand, true);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        private static bool isError(string i)
        {
            log = "";
            bool state = false;
            i = i.ToLower();
            if (!getgpt) { FrpPart = ""; DevPart = ""; KernelPart = ""; EndStor = ""; }
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2")) state = true;
            return state;
        }
        public static void LOG(string i)
        {
            if (PORTER == null || LOGGE == null) return;
            LOGGE.AppendText((newline + i));

            se = new StreamWriter("log.txt");
            se.WriteLine(LOGGE.Text);
            se.Close();
        }
        public static void PORTFIND()
        {
            if (PORTER == null || LOGGE == null) return;
            current.Clear();
            foreach (var a in SerialPort.GetPortNames())
            {
                current.Add(a);
                if (first) notdevice.Add(a);
                if (!notdevice.Contains(a) && !first && port != a) { PORTER.Text = "Device port: " + a; port = a; LOG(I("CPort") + a); break; }
            }
            if (first) first = false;
            if (!current.Contains(port)) { if (port != "NaN") LOG(I("DPort") + port); port = "NaN"; PORTER.Text = L("CLabel"); loadedhose = false; }
        }
        public static string PickLoader(string dev)
        {
            foreach (var a in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        private static bool RUN(string command, string subcommand, bool big)
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
                if (getgpt && outtext.ToLower().Contains("partition name:"))
                {
                    if (outtext.ToLower().Contains("frp"))
                        FrpPart = "frp";
                    if (outtext.ToLower().Contains("devinfo"))
                        DevPart = "devinfo";
                    if (outtext.ToLower().Contains("userdata"))
                        EndStor = outtext.Split(' ')[6];
                    if (outtext.ToLower().Contains("kernel"))
                        FrpPart = "frp";
                }
                if (big)
                {
                    if (outtext.Contains("%"))
                    {
                        string a = outtext.Split(' ')[outtext.Split(' ').Length - 1].Replace("%}", "");
                        if (!debug) LOGGE.AppendText((newline + "INFO: Percent:-") + a + "%");
                        try { progr.Value = Convert.ToInt32(a); }
                        catch { }
                    }
                    if (outtext.ToLower().Contains("remain"))
                    {
                        if (!debug) LOGGE.AppendText(newline + "INFO: Sectors: " + int.Parse(outtext.Split(' ')[2]));
                        progr.Value = int.Parse(outtext.Split(' ')[2]);
                    }
                }

                log = log + newline + outtext;
                if (debug) LOGGE.AppendText((newline + outtext));
            }
            if (!getgpt) return !isError(log); else return true;
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
    }
}