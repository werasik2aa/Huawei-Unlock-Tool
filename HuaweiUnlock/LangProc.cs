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

namespace HuaweiUnlocker
{
    public static class LangProc
    {
        public static TextBox LOGGBOX;
        public static bool loadedhose = false;
        public static bool debug = false;
        public static string log, loge, newline = Environment.NewLine;
        private static StreamWriter se;
        public static NProgressBar progr;
        public static TabControl Tab;
        public static Form Fl;
        public static int cur;
        public static Dictionary<string, int[]> GPTTABLE = new Dictionary<string, int[]>();
        private static Action action;
        private static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static string CURRENTlanguage = "English";
        public static bool AsyncRUN(string command, string subcommand)
        {
            loge = "SUCCESS";
            Tab.Enabled = false;
            LOG("===START EVENT===");
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = subcommand;
            p.OutputDataReceived += new DataReceivedEventHandler(eventrd);
            p.Start();
            p.BeginOutputReadLine();
            return !isError(log);
        }
        public static bool SyncRUN(string command, string subcommand)
        {
            log = "SUCCESS";
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
                if (outtext.ToLower().Contains("partition name:"))
                {
                    string[] partitionDATA = outtext.Split(' ');
                    int[] partitionlbl = new int[2];
                    partitionlbl[0] = int.Parse(partitionDATA[6]);
                    partitionlbl[1] = int.Parse(partitionDATA[10]);
                    GPTTABLE.Add(partitionDATA[3].ToLower(), partitionlbl);
                }

                log = log + newline + outtext;
                if (debug) LOGGBOX.AppendText((newline + outtext));
            }
            p.Dispose();
            return !isError(log);
        }
        private static void returnall(object sender, EventArgs e)
        {

            Action action = () => { LOGGBOX.AppendText(newline + "===END EVENT==="); };
            if (LOGGBOX.InvokeRequired)
                LOGGBOX.Invoke(action);
            else
                action();
        }
        private static void eventrd(object sender, DataReceivedEventArgs e)
        {
            Process p = (Process) sender;
            if (!String.IsNullOrEmpty(e.Data))
            {
                string outtext = e.Data.ToLower();
                int percent = 10;
                if (outtext.Contains("%") || outtext.ToLower().Contains("remain"))
                {
                    if (outtext.Contains("%"))
                        percent = int.Parse(outtext.Split(' ').Last().Replace("%}", "").Split('.')[0]);
                    else if (outtext.Contains("remain"))
                    {
                        int dS = cur - int.Parse(outtext.Split(' ')[2]);
                        if (dS < 0) dS = 1;
                        percent = (int)Math.Round((double)(100 * dS / cur));
                    }

                    if (percent < 0) percent = 1;
                    action = () => { LOGGBOX.Text = LOGGBOX.Text.Remove(LOGGBOX.Text.LastIndexOf(Environment.NewLine)); LOGGBOX.AppendText((newline + "INFO: Percent: '") + percent + "'%"); };
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();

                    action = () => progr.Value = percent;
                    if (progr.InvokeRequired)
                        progr.Invoke(action);
                    else
                        action();
                }
                loge = loge + outtext;
                if (debug)
                {
                    action = () => { LOGGBOX.AppendText((newline + outtext)); };
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();
                }
                Thread.Sleep(10);
                if (outtext.Contains("success") || outtext.Contains("complete") || outtext.Contains("status 0") || percent >=99)
                {
                    action = () => { LOGGBOX.Text = LOGGBOX.Text.Remove(LOGGBOX.Text.LastIndexOf(Environment.NewLine)); LOGGBOX.AppendText((newline + "INFO: Percent: '") + 100 + "'%"); LOGGBOX.AppendText(newline + "===END EVENT==="); };
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();

                    action = () => progr.Value = 100;
                    if (progr.InvokeRequired)
                        progr.Invoke(action);
                    else
                        action();

                    action = () => Tab.Enabled = true;
                    if (Tab.InvokeRequired)
                        Tab.Invoke(action);
                    else
                        action();
                    p.Dispose();
                }
                else if(isError(loge) && percent == 10)
                {
                    action = () => LOGGBOX.AppendText("ERROR: unknown ERROR" + newline + "===END EVENT===");
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();

                    action = () => progr.Value = 100;
                    if (progr.InvokeRequired)
                        progr.Invoke(action);
                    else
                        action();

                    action = () => Tab.Enabled = true;
                    if (Tab.InvokeRequired)
                        Tab.Invoke(action);
                    else
                        action();
                    p.Dispose();
                }
            }
        }

        public static bool isError(string i)
        {
            i = i.ToLower();
            if (i.Contains("the operation completed successfully") || i.Contains("success")) return false;
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2")) return true;
            return false;
        }
        public static void LOG(string i)
        {
            LOGGBOX.AppendText((newline + i));
            se = new StreamWriter("log.txt");
            se.WriteLine(LOGGBOX.Text);
            se.Close();
        }
        public static Port_D GETPORT(string name)
        {
            Port_D req = new Port_D();
            req.ComName = "NaN";
            req.DeviceName = "Not Found";
            try
            {
                foreach (ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"").Get())
                {
                    string DEVICEname = queryObj["Name"].ToString();
                    string[] DevideName = DEVICEname.Split(' ');
                    if (DEVICEname.ToLower().Contains(name))
                    {
                        req.ComName = DevideName[DevideName.Length - 1].Replace("(", "").Replace(")", "");
                        req.DeviceName = DEVICEname;
                        break;
                    }
                }
            }
            catch (Exception)
            { }
            return req;
        }
        public static string PickLoader(string dev)
        {
            string pth = Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev;
            if (!Directory.Exists(pth)) return "NaN";
            foreach (var a in Directory.GetFiles(pth))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        public static void ReadLngFile()
        {
            lang = new Dictionary<string, string>();
            Stream ss = File.OpenRead("Languages\\"+CURRENTlanguage+".ini");
            StreamReader readerL = new StreamReader(ss);
            string line = readerL.ReadLine();
            while ((line = readerL.ReadLine()) != null)
                lang.Add(line.Split(',')[0], line.Split(',')[1]);
            readerL.Close();
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
        public static bool CheckDevice(string path)
        {
            LOG(I("CheckCon"));
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) { process.Kill(); }
            if (FlashToolQClegacy.TxSide.ComName == "NaN")
            {
                LOG(E("DeviceNotCon"));
                loadedhose = false;
                return false;
            }
            if (!path.Contains(":\\"))
            {
                LOG(E("ErrLdr"));
                return false;
            }
            if (!File.Exists(path))
            {
                LOG(E("ErrLdr"));
                return false;
            }
            return true;
        }
    }
}