using System;
using static HuaweiUnlocker.LangProc;
using System.Collections.Generic;
using HuaweiUnlocker.DIAGNOS;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Linq;

namespace HuaweiUnlocker.FlashTool
{
    public static class FlashToolQClegacy
    {
        public static int CurPartLenght;
        public static bool LoadLoader(string pathloader)
        {
            if (DeviceInfo.Port.ComName == "NaN" || DeviceInfo.Port.ComName == "")
            {
                LOG(2, "NoDEVICE");
                DeviceInfo.loadedhose = false;
                return false;
            }
            if (!DeviceInfo.loadedhose)
            {
                LOG(0, "CPort", DeviceInfo.Port.FullName);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + pathloader + '"';
                if (debug) LOG(-1, "===.ELF / .MBN SECTOR===" + newline + newline);
                try
                {
                    LOG(0, "FireHose", pathloader);
                    DeviceInfo.loadedhose = DeviceInfo.loadedhose || SyncRUN(command, subcommand);
                    Progress(20);
                    return DeviceInfo.loadedhose;
                }
                catch (Exception e)
                {
                    if (debug) LOG(1, e.ToString());
                    return false;
                }
            }
            return true;
        }
        public static bool GetIdentifier()
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -info";
            if (debug) LOG(-1, "===.GET INDETIFIER.===" + newline + newline);
            if (SyncRUN(command, subcommand))
            {
                LOG(0, "HW_ID: ", DeviceInfo.HWID);
                LOG(0, "SW_ID: ", DeviceInfo.SWID);
                LOG(0, "OEM_PK_HASH: ", DeviceInfo.PK_HASH);
                LOG(0, "SBL_VER: ", DeviceInfo.SBLV);
                LOG(0, "CPU: ", DeviceInfo.CPUName);
                return true;
            }
            return false;
        }
        public static async void Unlock(string loader, string path)
        {
            CurTask = Task.Run(() =>
            {
                string command = "Tools\\emmcdl.exe";
                string subcommand = "Custom";
                if (debug) LOG(-1, "===UNLOCKER===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; return false; }
                try
                {
                    LOG(0, "Unlocker", DeviceInfo.Name);
                    LOG(0, "Unlocker", "ReadGPT");
                    if (!ReadGPT(loader))
                    { LOG(2, "ERR_ReadGPT"); CurTask.Dispose(); }
                    string[] fileEntries = Directory.GetFiles(path);
                    foreach (string fileName in fileEntries)
                    {
                        string partname = fileName.Split('\\').Last().Split('.').First();
                        if (fileName.EndsWith(".xml")) continue;
                        LOG(0, "Writer", partname + newline);
                        subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b " + partname + " " + '"' + fileName + '"';
                        CurPartLenght = int.Parse(DeviceInfo.Partitions[partname].BlockNumSectors);
                        if (!SyncRUN(command, subcommand))
                            LOG(2, "FailUnl");
                        else Progress(50);
                    }
                    if (!File.Exists(path + "\\kernel") & !File.Exists(path + "\\kernel.img")) LOG(1, "WarnUnl");
                    Progress(100);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    LOG(2, "FailUnl");
                    return false;
                }
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static bool Write(string partition, string loader, string path)
        {
            Progress(2);
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b " + partition + " " + '"' + path + '"';
            if (debug) LOG(-1, "===WRITE PARTITION===" + newline + newline);
            if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
            try
            {
                Progress(50);
                LOG(0, "Writer", partition + newline);
                if (!SyncRUN(command, subcommand))
                    LOG(0, "EwPE", partition);
                return true;
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static async void Erase(string partition, string loader)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -e " + partition;
                if (debug) LOG(-1, "===ERASE PARTITION===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(50);
                    LOG(0, "Eraser", partition);
                    if (!SyncRUN(command, subcommand))
                        LOG(1, "ErPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static bool ReadGPT(string loader)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -gpt";
            try
            {
                DeviceInfo.Partitions = new Dictionary<string, Partition>();
                if (debug) { LOG(-1, "===READ GPT===" + newline + newline); }
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); return false; }
                return SyncRUN(command, subcommand) & DeviceInfo.Partitions.Count > 0;
            }
            catch { }
            return true;
        }
        public static bool UnlockFrp(string loader)
        {
            Progress(2);
            if (debug) LOG(-1, "===UNLOCK FRP===" + newline + newline);
            string command = "Tools\\emmcdl.exe";
            string subcommandDEV = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b devinfo " + '"' + "Tools/frpUnlocked.img" + '"';
            string subcommandFRP = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b frp " + '"' + "Tools/frpPartition.img" + '"';
            if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                Progress(30);
                LOG(0, "Unlocker", "ReadGPT");
                if (DeviceInfo.Partitions.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(2, "ERR_ReadGPT"); return false; }
                bool status = false;
                if (DeviceInfo.Partitions.ContainsKey("devinfo"))
                {
                    LOG(0, "Writer", "DEVINFO: " + DeviceInfo.Partitions["devinfo"].BlockLength + newline);
                    if (!SyncRUN(command, subcommandDEV))
                        LOG(0, "EwPE", "devinfo");
                    else
                        LOG(0, "Success", "frpUnlocked.img - devinfo");
                    status = true;
                }
                else
                    LOG(1, "FailFrpD", "NO DEVINFO Partition... Continuing" + newline);

                if (DeviceInfo.Partitions.ContainsKey("frp"))
                {
                    LOG(0, "Writer", "FRP: " + DeviceInfo.Partitions["frp"].BlockLength + newline);
                    if (!SyncRUN(command, subcommandFRP))
                        LOG(0, "EwPE", "frp");
                    else
                        return LOG(0, "Success", "frpPartition.img - frp");
                }
                else
                    LOG(2, "FailFrpD", "NO FRP Partition...!!! FAILED TO remove frp" + newline);

                if (status)
                    LOG(1, "Warning", "WarnFrp");
                return status;
            }
            catch (Exception e)
            {
                if (debug) LOG(-1, e.ToString());
                return false;
            }
        }
        public static async void FlashPartsXml(string rawxml, string patchxml, string loader, string path)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\fh_loader.exe";
                string subcommand = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --showpercentagecomplete --sendxml=" + '"' + rawxml + '"' + " --search_path=" + '"' + path + '"';
                string subcommandp = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --sendxml=" + '"' + patchxml + '"' + " --search_path=" + '"' + path + '"';
                if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(0);
                    LOG(0, "Flasher", path);
                    LOG(0, "Info: ", patchxml);
                    if (!String.IsNullOrEmpty(patchxml))
                    {
                        if (!File.Exists(patchxml))
                            LOG(1, "NotFoundF", patchxml);
                        else
                        if (!SyncRUN(command, subcommandp))
                            LOG(2, "EwRGPT", patchxml);
                        else 
                            LOG(0, "IwRGPT");
                    }
                    LOG(0, "Info", rawxml);
                    if (!String.IsNullOrEmpty(rawxml))
                    {
                        if (!File.Exists(rawxml))
                            LOG(1, "NotFoundF", rawxml);
                        else
                        if (!SyncRUN(command, subcommand))
                            LOG(2, "ErrXML2", rawxml);
                        else
                            LOG(0, "RrGPTXMLS");
                    }
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static bool EraseMemory(string loader)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --erase=0 --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=emmc";
            if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline);
            if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                Progress(50);
                LOG(0, "Erasing MEMORY!!");
                return SyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static async void FlashPartsRaw(string loader, string file)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\fh_loader.exe";
                string subcommand = " --port=\\\\.\\" + DeviceInfo.Port.ComName + " --sendimage=" + '"' + file + '"' + " --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=eMMC";
                if (debug) LOG(-1, "===Flash Partitions RAW===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(0);
                    LOG(0, "EemmcWPS", file);
                    LOG(1, "WarnLarge");
                    if (!SyncRUN(command, subcommand))
                        LOG(2, "ErrBin2");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static async void Dump(string loader, string savepath)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                Progress(20);
                if (DeviceInfo.Partitions.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(2, "ERR_ReadGPT"); CurTask.Dispose(); }
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d 0 " + int.Parse(DeviceInfo.Partitions["userdata"].BlockStart) + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER===" + newline + newline);
                try
                {
                    CurPartLenght = int.Parse(DeviceInfo.Partitions["userdata"].BlockStart);
                    LOG(0, "Dump", "0" + "<-TO->" + CurPartLenght);
                    if (SyncRUN(command, subcommand))
                        LOG(2, "DumpingE");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                }
                return false;
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static async void Dump(int i, int o, string partition, string loader, string savepath)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d " + i + " " + o + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER PARTITION===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(30);
                    if (DeviceInfo.Partitions.Count == 0)
                        if (!ReadGPT(loader))
                        { LOG(2, "ERR_ReadGPT"); CurTask.Dispose(); }
                    LOG(0, "DumpTp", partition);
                    CurPartLenght = int.Parse(DeviceInfo.Partitions[partition].BlockLength);
                    if (!SyncRUN(command, subcommand))
                        LOG(0, "EdPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                }
                return false;
            }, token);
            await CurTask; Tab.Enabled = true;
        }
        public static async void Dump(string partition, string loader, string savepath)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d " + partition + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER PARTITION===" + newline + newline);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(30);
                    if (DeviceInfo.Partitions.Count == 0)
                        if (!ReadGPT(loader))
                        { LOG(2, "ERR_ReadGPT"); CurTask.Dispose(); }
                    LOG(0, "DumpTp", partition);
                    CurPartLenght = int.Parse(DeviceInfo.Partitions[partition].BlockLength);
                    if (!SyncRUN(command, subcommand))
                        LOG(0, "EdPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                }
                return false;
            }, token);
            await CurTask; Tab.Enabled = true;
        }
    }
}