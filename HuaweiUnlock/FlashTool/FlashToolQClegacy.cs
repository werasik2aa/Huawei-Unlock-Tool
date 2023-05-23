using System;
using static HuaweiUnlocker.LangProc;
using System.Collections.Generic;
using HuaweiUnlocker.DIAGNOS;
using System.IO;

namespace HuaweiUnlocker.FlashTool
{
    public static class FlashToolQClegacy
    {
        public static Port_D TxSide;
        public static bool LoadLoader(string device, string pathloader)
        {
            if (TxSide.ComName == "NaN" || TxSide.ComName == "")
            {
                LOG(2, "NoDEVICE");
                loadedhose = false;
                return false;
            }
            LOG(0, "CPort", TxSide.DeviceName);
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + pathloader + '"';
            if (debug) LOG(-1, "===.ELF / .MBN SECTOR===" + newline + newline + command + newline + subcommand);
            try
            {
                LOG(0, "FireHose", device);
                if (!loadedhose)
                    loadedhose = SyncRUN(command, subcommand);
                progr.Value = 20;
                return loadedhose;
            }
            catch (Exception e)
            {
                if (debug) LOG(1, e.ToString());
                return false;
            }
        }
        public static bool Unlock(string device, string loader, string path)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + TxSide.ComName + " --sendxml=" + '"' + path + "\\rawprogram0.xml" + '"' + " --noprompt --showpercentagecomplete --search_path=" + '"' + path + '"';
            if (debug) LOG(-1, "===UNLOCKER===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader(device, loader)) { loadedhose = false; return false; }
            try
            {
                progr.Value = 40;
                LOG(0, "Unlocker", device + newline);
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool Write(string partition, string loader, string path)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -b " + partition + " " + '"' + path + '"';
            if (debug) LOG(-1, "===WRITE PARTITION===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 50;
                LOG(0, "Writer", partition);
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool Erase(string partition, string loader)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -e " + partition;
            if (debug) LOG(-1, "===ERASE PARTITION===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 50;
                LOG(0, "Eraser", partition);
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool ReadGPT(string loader)
        {
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -gpt";
            try
            {
                GPTTABLE = new Dictionary<string, int[]>();
                if (debug) { LOG(-1, "===READ GPT===" + newline + newline + command + newline + subcommand); }
                if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
                return SyncRUN(command, subcommand);
            }
            catch { }
            return true;
        }
        public static bool UnlockFrp(string loader)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -e frp";
            if (debug) LOG(-1, "===UNLOCK FRP===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 30;
                LOG(0, "Unlocker", "FRP: Universal" + newline + "GETGPT: INIT!");
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader)) { LOG(2, "FailFrpD"); return false; }

                bool status = false;
                if (GPTTABLE.ContainsKey("devinfo"))
                {
                    LOG(0, "Writer", "DEVINFO Write lenght: " + GPTTABLE["frp"][1]);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -b devinfo " + "Tools/frpUnlocked.img";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(1, "FailFrpD", "NO DEVINFO Partition... Continuing");

                if (GPTTABLE.ContainsKey("frp"))
                {
                    progr.Value = 50;
                    cur = GPTTABLE["frp"][1];
                    LOG(0, "Eraser", "FRP erase lenght: " + GPTTABLE["frp"][1]);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -e frp";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(1, "FailFrpD", "NO FRP Partition...");

                return status;
            }
            catch (Exception e)
            {
                if (debug) LOG(-1, e.ToString());
                return false;
            }
        }
        public static bool FlashPartsXml(string rawxml, string patchxml, string loader, string path)
        {
            progr.Value = 2;
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + TxSide.ComName + " --sendxml=" + '"' + rawxml + '"' + "--noprompt --showpercentagecomplete --search_path=" + '"' + path + '"';
            string subcommandp = "--port=\\\\.\\" + TxSide.ComName + " --sendxml=" + '"' + patchxml + '"' + " --search_path=" + '"' + path + '"';
            if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 0;
                LOG(0, "Flasher", path);
                if (!String.IsNullOrEmpty(patchxml))
                {
                    if (!File.Exists(patchxml))
                        LOG(1, "NotFoundF", patchxml);
                    else
                    if (!SyncRUN(command, subcommandp))
                    {
                        LOG(2, "EwRGPT", patchxml);
                        return false;
                    }
                    else LOG(0, "IwRGPT");
                }
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool EraseMemory(string loader)
        {
            string command = "Tools\\fh_loader.exe";
            string subcommand = "--port=\\\\.\\" + TxSide.ComName + " --erase=0 --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=emmc";
            if (debug) LOG(-1, "===Flash Partitions XML===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 50;
                LOG(0, "Erasing MEMORY!!");
                return SyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool FlashPartsRaw(string loader, string file)
        {
            progr.Value = 2;
            string command = "Tools\\fh_loader.exe";
            string subcommand = " --port=\\\\.\\" + TxSide.ComName + " --sendimage=" + '"' + file + '"' + " --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=eMMC";
            if (debug) LOG(-1, "===Flash Partitions RAW===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 0;
                LOG(0, "EemmcWPS", file);
                LOG(-1, "First Init takes more time for sparse img before Flash: ");
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
                return false;
            }
        }
        public static bool Dump(string loader, string savepath)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -d 0 " + GPTTABLE["userdata"][0] + " -o " + '"' + savepath + '"';
            if (debug) LOG(-1, "===DUMPER===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 30;
                LOG(0, "DumpTr", newline);
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(2, "Unknown"); return false; }
                LOG(0, "Dump", "0" + "<-TO->" + GPTTABLE["userdata"][0]);
                cur = GPTTABLE["userdata"][0];
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
            }
            return false;
        }
        public static bool Dump(int i, int o, string partition, string loader, string savepath)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -d " + i + " " + o + " -o " + '"' + savepath + '"' + "\\" + partition;
            if (debug) LOG(-1, "===DUMPER PARTITION===" + newline + newline + command + newline + subcommand);
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(2, "Fail"); return false; }
            try
            {
                progr.Value = 30;
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(2, "Unknown"); return false; }
                LOG(0, "DumpTp", partition + newline);
                cur = GPTTABLE[partition][1];
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(2, e.ToString());
            }
            return false;
        }
    }
}
