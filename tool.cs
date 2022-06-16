using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace HuaweiUnlocker
{
    public class tool
    {
        public static TextBox LOGGE, PORTER;
        public static bool loadedhose = false;
        public static bool debug = false;
        public static bool error;
        public static string port = "NaN";
        private static List<string> notdevice = new List<string>();
        private static List<string> current = new List<string>();
        private static bool first = true;

        public static bool LoadLoader(string device, string pathloader)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + port + " -f " + '"' + pathloader + '"';
            if (debug) { LOG("===FLASHLOADER===/n/n" + command + "/n"); LOG(subcommand); }
            try
            {
                LOG("INFO: Flashing Loader to: " + device);
                var state = RUN(command, subcommand);
                loadedhose = state;
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
            if (debug) { LOG("===UNLOCKER===/n/n" + command + "/n"); LOG(subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                LOG("INFO: Trying to unlock: " + device);
                return RUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool FlashPartsXml(string xml, string loader, string path)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + port + " --sendxml=" + '"' + xml + '"' + " --search_path=" + path;
            if (debug) { LOG("===Flash Partitions===/n/n" + command + "/n"); LOG(subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                LOG("INFO: Flashing RAW partitions: " + path);
                return RUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        public static bool FlashPartsRaw(string loader, string file)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + port + " --sendimage=" + file;
            if (debug) { LOG("===Flash Partitions===/n/n" + command + "/n"); LOG(subcommand); }
            if (!loadedhose)
                if (!LoadLoader("HUAWEI", loader)) { loadedhose = false; return false; }
                else loadedhose = true;
            try
            {
                LOG("INFO: Flashing IMAGE: " + file);
                return RUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
                return false;
            }
        }
        private static bool isError(string i)
        {
            bool state = false;
            i = i.ToLower();
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2")) state = true;
            return state;
        }
        public static void LOG(string i)
        {
            if (PORTER == null || LOGGE == null) return;
            LOGGE.Text = (tool.LOGGE.Text + "/n" + i).Replace("/n", Environment.NewLine);

            StreamWriter se = File.AppendText("log.txt");
            se.WriteLine(i.Replace("/n", Environment.NewLine));
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
                if (!notdevice.Contains(a) && !first && port != a) { PORTER.Text = "Device port: " + a; port = a; LOG("INFO: Connected PORT: " + a); break; }
            }
            if (first) first = false;
            if (!current.Contains(port)) { if (port != "NaN") LOG("INFO: Disconnected PORT: " + port); port = "NaN"; PORTER.Text = "Connect Your Device"; loadedhose = false; }
        }
        public static string PickLoader(string dev)
        {
            foreach (var a in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        private static bool RUN(string command, string subcommand)
        {
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = subcommand;
            p.Start();
            string a = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            if (debug) LOG(a);
            return !isError(a);
        }
    }
}
