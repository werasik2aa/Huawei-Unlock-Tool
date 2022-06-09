using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using HuaweiUnlocker;

namespace HuaweiUnlocker
{
    public class dload
    {
        internal static bool LoadLoader(string comport, string device, string loader, string path)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + comport + " -f " + path + "\\" + loader;
            if (Huawei.debug) { Huawei.LOG("===FLASHLOADER===/n/n" + command + "/n"); Huawei.LOG(subcommand); }
            try
            {
                Huawei.LOG("INFO: Flashing Loader to: " + device);
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = command;
                p.StartInfo.Arguments = subcommand;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (Huawei.debug) Huawei.LOG(output);
                return !isError(output);
            }
            catch (Exception e)
            {
                if (Huawei.debug) Huawei.LOG(e.ToString());
                return false;
            }
        }
        internal static bool Unlock(string comport, string device, string loader, string path)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\"+comport+" --sendxml="+path+"\\rawprogram0.xml" + " --search_path="+path;
            if (Huawei.debug) { Huawei.LOG("===UNLOCKER===/n/n" + command + "/n"); Huawei.LOG(subcommand); }
            if (!Huawei.loadedhose)
                if (!LoadLoader(comport, device, loader, path)) return false;
                else Huawei.loadedhose = true;
            try
            {
                Huawei.LOG("INFO: Trying to unlock: " + device);
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = command;
                p.StartInfo.Arguments = subcommand;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (Huawei.debug) Huawei.LOG(output);
                return !isError(output);
            }
            catch (Exception e)
            {
                if (Huawei.debug) Huawei.LOG(e.ToString());
                return false;
            }
        }
        private static bool isError(string i)
        {
            bool state = false;
            i = i.ToLower();
            if (i.Contains("failed") || i.Contains("error") || i.Contains("fail") || i.Contains("status: 2")) state = true;
            Huawei.error = state;
            return state;
        }
    }
}
