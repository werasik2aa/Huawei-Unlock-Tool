using System;
using static HuaweiUnlocker.LangProc;
using System.Collections.Generic;
using HuaweiUnlocker.DIAGNOS;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
            LOG(0, "CPort", DeviceInfo.Port.FullName);

            if (!DeviceInfo.loadedhose)
            {
                if (GetIdentifier())
                {
                    string command = "Tools\\emmcdl.exe";
                    string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + pathloader + '"';
                    if (debug) LOG(-1, "===.ELF / .MBN SECTOR===" + newline + newline + command + newline + subcommand);
                    try
                    {
                        LOG(0, "FireHose", DeviceInfo.Name + " : " + pathloader);
                        if (!DeviceInfo.loadedhose)
                            DeviceInfo.loadedhose = SyncRUN(command, subcommand);
                        Progress(20);
                        return DeviceInfo.loadedhose;
                    }
                    catch (Exception e)
                    {
                        if (debug) LOG(1, e.ToString());
                        return false;
                    }
                }
                else return false;
            } return true;
        }
        public static bool GetIdentifier()
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -info";
            if (debug) LOG(-1, "===.GET INDETIFIER.===" + newline + newline + command + newline + subcommand);
            if (SyncRUN(command, subcommand))
            {
                LOG(0, "HW_ID", DeviceInfo.HWID);
                LOG(0, "SW_ID", DeviceInfo.SWID);
                LOG(0, "OEM_PK_HASH", DeviceInfo.PK_HASH);
                LOG(0, "SBL_VER", DeviceInfo.SBLV);
                return true;
            }
            return false;
        }
        public static async void Unlock(string loader, string path)
        {
            CurTask = Task.Run(() =>
            {
                string command = "Tools\\fh_loader.exe";
                string subcommand = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --sendxml=" + '"' + path + "\\rawprogram0.xml" + '"' + " --noprompt --showpercentagecomplete --search_path=" + '"' + path + '"';
                if (debug) LOG(-1, "===UNLOCKER===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; return false; }
                try
                {
                    Progress(40);
                    LOG(0, "Unlocker", newline);
                    if(!SyncRUN(command, subcommand))
                        LOG(2, "FailUnl");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    LOG(2, "FailUnl");
                    return false;
                }
            });
            await CurTask;
        }
        public static async void Write(string partition, string loader, string path)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b " + partition + " " + '"' + path + '"';
                if (debug) LOG(-1, "===WRITE PARTITION===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(50);
                    LOG(0, "Writer", partition);
                    if(!SyncRUN(command, subcommand))
                        LOG(0, "EwPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            });
            await CurTask;
        }
        public static async void Erase(string partition, string loader)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -e " + partition;
                if (debug) LOG(-1, "===ERASE PARTITION===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(50);
                    LOG(0, "Eraser", partition);
                    if(!SyncRUN(command, subcommand))
                        LOG(1, "ErPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            });
            await CurTask;
        }
        public static bool ReadGPT(string loader)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -gpt";
            try
            {
                DeviceInfo.Partitions = new Dictionary<string, Partition>();
                if (debug) { LOG(-1, "===READ GPT===" + newline + newline + command + newline + subcommand); }
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); return false; }
                return SyncRUN(command, subcommand);
            }
            catch { }
            return true;
        }
        public static bool UnlockFrp(string loader)
        {
            Progress(2);
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -e frp";
            if (debug) LOG(-1, "===UNLOCK FRP===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                Progress(30);
                LOG(0, "Unlocker", "ReadGPT");
                if (DeviceInfo.Partitions.Count == 0)
                    if (!ReadGPT(loader)) 
                    { LOG(2, "FailFrpD"); return false; }
                bool status = false;
                if (DeviceInfo.Partitions.ContainsKey("devinfo"))
                {
                    LOG(0, "Writer", "DEVINFO Write lenght: " + DeviceInfo.Partitions["frp"].BlockLength);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -b devinfo " + "Tools/frpUnlocked.img";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(1, "FailFrpD", newline + "NO DEVINFO Partition... Continuing");

                if (DeviceInfo.Partitions.ContainsKey("frp"))
                {
                    Progress(50);
                    CurPartLenght = int.Parse(DeviceInfo.Partitions["frp"].BlockLength);
                    LOG(0, "Eraser", "FRP erase lenght: " + DeviceInfo.Partitions["frp"].BlockLength);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -e frp";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(1, "FailFrpD", newline + "NO FRP Partition...");

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
                string subcommand = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --sendxml=" + '"' + rawxml + '"' + "--noprompt --showpercentagecomplete --search_path=" + '"' + path + '"';
                string subcommandp = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --sendxml=" + '"' + patchxml + '"' + " --search_path=" + '"' + path + '"';
                if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(0);
                    LOG(0, "Flasher", path);
                    if (!String.IsNullOrEmpty(patchxml))
                    {
                        if (!File.Exists(patchxml))
                            LOG(1, "NotFoundF", patchxml);
                        else
                        if (!SyncRUN(command, subcommandp))
                            LOG(2, "EwRGPT", patchxml);
                        else LOG(0, "IwRGPT");
                    }
                   if(!SyncRUN(command, subcommand))
                        LOG(2, "EemmcXML_WPE");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            });
            await CurTask;
        }
        public static bool EraseMemory(string loader)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + DeviceInfo.Port.ComName + " --erase=0 --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=emmc";
            if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline + command + newline + subcommand);
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
                if (debug) LOG(-1, "===Flash Partitions RAW===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(0);
                    LOG(0, "EemmcWPS", file);
                    LOG(-1, "First Init takes more time for sparse img before Flash: ");
                    if (!SyncRUN(command, subcommand))
                        LOG(2, "ErrBin2");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                    return false;
                }
            });
            await CurTask;
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
                    { LOG(2, "Unknown"); CurTask.Dispose(); }
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d 0 " + int.Parse(DeviceInfo.Partitions["userdata"].BlockStart) + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER===" + newline + newline + command + newline + subcommand);
                try
                {
                    CurPartLenght = int.Parse(DeviceInfo.Partitions["userdata"].BlockStart);
                    LOG(0, "Dump", "0" + "<-TO->" + CurPartLenght);
                    if(SyncRUN(command, subcommand))
                        LOG(2, "DumpingE");
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                }
                return false;
            });
            await CurTask;
        }
        public static async void Dump(int i, int o, string partition, string loader, string savepath)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d " + i + " " + o + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER PARTITION===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(30);
                    if (DeviceInfo.Partitions.Count == 0)
                        if (!ReadGPT(loader))
                        { LOG(2, "Unknown"); CurTask.Dispose(); }
                    LOG(0, "DumpTp", partition);
                    CurPartLenght = int.Parse(DeviceInfo.Partitions[partition].BlockLength);
                    if(!SyncRUN(command, subcommand))
                        LOG(0, "EdPE", partition);
                    return true;
                }
                catch (Exception e)
                {
                    if (debug) LOG(2, e.ToString());
                }
                return false;
            });
            await CurTask;
        }
        public static async void Dump(string partition, string loader, string savepath)
        {
            CurTask = Task.Run(() =>
            {
                Progress(2);
                string command = "Tools\\emmcdl.exe";
                string subcommand = "-p " + DeviceInfo.Port.ComName + " -f " + '"' + loader + '"' + " -d " + partition + " -o " + '"' + savepath + '"';
                if (debug) LOG(-1, "===DUMPER PARTITION===" + newline + newline + command + newline + subcommand);
                if (!LoadLoader(loader)) { DeviceInfo.loadedhose = false; LOG(2, "Fail"); CurTask.Dispose(); }
                try
                {
                    Progress(30);
                    if (DeviceInfo.Partitions.Count == 0)
                        if (!ReadGPT(loader))
                        { LOG(2, "Unknown"); CurTask.Dispose(); }
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
            });
            await CurTask;
        }
    }
}
