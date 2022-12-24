using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Management;
using System.Linq;
using static HuaweiUnlocker.CMD;

namespace HuaweiUnlocker
{
    public static class MISC
    {
        public static TextBox LOGGBOX;
        public static bool loadedhose = false;
        public static bool debug = false;
        public static string log, newline = Environment.NewLine;
        private static StreamWriter se;
        public static ProgressBar progr;
        public static Form Fl;
        public static int cur;
        public static Dictionary<string, int[]> GPTTABLE = new Dictionary<string, int[]>();
        private static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static bool RUN(string command, string subcommand, bool getgpt, bool bysectors)
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
                        string a = outtext.Split(' ')[outtext.Split(' ').Length-1].Replace("%}", "");
                        LOGGBOX.AppendText((newline + "INFO: Percent: ") + a + "%");
                        int percent = int.Parse(a.Split('.')[0]);
                        progr.Value = percent;
                        progr.Maximum = 100;
                    } else if (outtext.ToLower().Contains("remain"))
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

        public static bool isError(string i)
        {
            log = "";
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
            {}
            return req;
        }
        public static string PickLoader(string dev)
        {
            foreach (var a in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\qc_boot\\" + dev))
                if (a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) return a;
            return "";
        }
        public static void ReadLngFile()
        {
            lang = new Dictionary<string, string>();
            Stream ss = File.OpenRead("lang.ini");
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
            if (GETPORT("qdloader 9008").ComName == "NaN")
            {
                LOG(E("DeviceNotCon"));
                loadedhose = false;
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