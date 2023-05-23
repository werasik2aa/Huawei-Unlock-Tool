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
using System.Linq.Expressions;

namespace HuaweiUnlocker
{
    public static class LangProc
    {
        public static TextBox LOGGBOX;
        public static bool loadedhose = false, debug = false;
        public static string log, loge, newline = Environment.NewLine, PrevFolder = "c:\\";
        private static StreamWriter se = new StreamWriter("log.txt");
        public static NProgressBar progr;
        public static TabControl Tab;
        public static int cur;
        public static Dictionary<string, int[]> GPTTABLE = new Dictionary<string, int[]>();
        private static Action action;
        public static bool AsyncRUN(string command, string subcommand)
        {
            loge = "SUCCESS";
            Tab.Enabled = false;
            LOG(-1, "===START EVENT===");
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = subcommand;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += new DataReceivedEventHandler(eventrd);
            p.Exited += new EventHandler(DoneSend);
            p.Start();
            p.BeginOutputReadLine();
            return !isError(loge);
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
                if (debug) LOGGBOX.AppendText(newline + outtext);
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
        private static void DoneSend(object sender, EventArgs e)
        {
            Process p = (Process)sender;
            if (loge.Contains("success") || loge.Contains("complete") || loge.Contains("status 0"))
            {
                action = () => LOGGBOX.AppendText(newline + "===END EVENT===" + newline);
                if (LOGGBOX.InvokeRequired)
                    LOGGBOX.Invoke(action);
                else
                    action();
            }
            else if (isError(loge))
            {
                action = () => LOGGBOX.AppendText(newline + "Unknown ERROR" + newline + "===END EVENT===" + newline);
                if (LOGGBOX.InvokeRequired)
                    LOGGBOX.Invoke(action);
                else
                    action();
            }
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
        private static void eventrd(object sender, DataReceivedEventArgs e)
        {
            Process p = (Process)sender;
            if (!String.IsNullOrEmpty(e.Data))
            {
                string outtext = e.Data.ToLower();
                int percent = 1;
                if (outtext.Contains("%") || outtext.ToLower().Contains("remain"))
                {
                    if (outtext.Contains("%"))
                        percent = int.Parse(outtext.Split(' ').Last().Replace("%}", "").Split('.')[0]);
                    else if (outtext.Contains("remain"))
                    {
                        int dS = cur - int.Parse(outtext.Split(' ')[2]);
                        if (cur != 0)
                            percent = (int)Math.Round((double)(100 * dS / cur));
                        if (percent <= 0) percent = 1;
                    }
                    action = () => { LOGGBOX.Text = LOGGBOX.Text.Substring(0, LOGGBOX.Text.LastIndexOf(newline)); LOGGBOX.AppendText(newline + percent + "'%"); };
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();

                    action = () => progr.Value = percent;

                    if (progr.InvokeRequired)
                        progr.Invoke(action);
                    else
                        action();
                    Thread.Sleep(10);
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
            }
        }
        public static bool isError(string i)
        {
            i = i.ToLower();
            if (i.Contains("the operation completed successfully") || i.Contains("success")) return false;
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2") || i.Contains("failed to write hello response back to device")) return true;
            return false;
        }
        public static void LOG(int o, string i, object j = null, string sepa = " ")
        {
            string state = "";
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
            if (j != null)
            {
                j = Language.isExist(j.ToString()) ? Language.Get(j.ToString()) : j;
                LOGGBOX.AppendText(newline + state + i + sepa + j.ToString());
            }
            else
                LOGGBOX.AppendText(newline + state + i);
            se.WriteLine(LOGGBOX.Text);
        }
        public static Port_D GETPORT(string name, ComboBox combox, bool auto)
        {
            Port_D req = new Port_D();
            req.ComName = "NaN";
            req.DeviceName = "Not Found";
            if (auto)
                try
                {
                    foreach (ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"").Get())
                    {
                        string DEVICEname = queryObj["Name"].ToString();
                        if (DEVICEname.ToLower().Contains(name))
                        {
                            req.ComName = DEVICEname.Split(' ').Last().Replace("(", "").Replace(")", "");
                            req.DeviceName = DEVICEname;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    LOG(2, "NO PERMISSIONS, TRY LAUNCH WITH ADMIN RIGHTS");
                }
            else
            {
                if (!String.IsNullOrEmpty(combox.Text))
                {
                    req.ComName = combox.Text.Split(' ').Last().Replace("(", "").Replace(")", "");
                    req.DeviceName = combox.Text;
                }
                else
                    LOG(1, "Select port please");
            }
            return req;
        }
        public static List<Port_D> GETPORTLIST(string name)
        {
            List<Port_D> req = new List<Port_D>();
            try
            {
                foreach (ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"").Get())
                {
                    string DEVICEname = queryObj["Name"].ToString();
                    Port_D reqq = new Port_D();
                    reqq.ComName = DEVICEname.Split(' ').Last().Replace("(", "").Replace(")", "");
                    reqq.DeviceName = DEVICEname;
                    req.Add(reqq);
                }
            }
            catch (Exception)
            {
                LOG(2, "[ERROR] NO PERMISSIONS, TRY LAUNCH WITH ADMIN RIGHTS");
            }
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
        public static void ClearLog()
        {
            LOGGBOX.Text = "";
        }
        public static bool CheckDevice(string path)
        {
            LOGGBOX.Text = "";
            LOG(0, "CheckCon");
            if (FlashToolQClegacy.TxSide.ComName == "NaN")
            {
                LOG(2, "DeviceNotCon");
                loadedhose = false;
                return false;
            }
            if (!path.Contains(":\\"))
            {
                LOG(2, "ErrLdr");
                return false;
            }
            if (!File.Exists(path))
            {
                LOG(2, "ErrLdr");
                return false;
            }
            return true;
        }
    }
}