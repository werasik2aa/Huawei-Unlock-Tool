using System;
using static HuaweiUnlocker.LangProc;
using System.Collections.Generic;
using System.Threading;
using HuaweiUnlocker.DIAGNOS;
using System.Collections.Concurrent;

namespace HuaweiUnlocker.FlashTool
{
    public static class FlashToolQClegacy
    {
        public static Port_D TxSide;
        public static bool LoadLoader(string device, string pathloader)
        {
            if (TxSide.ComName == "NaN" || TxSide.ComName == "")
            {
                LOG(E("NoDEVICE"));
                loadedhose = false;
                return false;
            }
            LOG(I("CPort") + TxSide.DeviceName);
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + pathloader + '"';
            if (debug) { LOG("===.ELF / .MBN SECTOR===" + newline + newline + command + newline + subcommand); }
            try
            {
                LOG(I("FireHose") + device);
                if (!loadedhose)
                    loadedhose = SyncRUN(command, subcommand);
                progr.Value = 20;
                return loadedhose;
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
            string subcommand = "--port=\\\\.\\" + TxSide.ComName + " --sendxml=" + '"' + path + "\\rawprogram0.xml" + '"' + " --search_path=" + '"' + path + '"';
            if (debug) { LOG("===UNLOCKER===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader(device, loader)) { loadedhose = false; return false; }
            try
            {
                progr.Value = 40;
                LOG(I("Unlocker") + device);
                return AsyncRUN(command, subcommand);
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
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -b " + partition + " " + '"' + path + '"';
            if (debug) { LOG("===WRITE PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 50;
                LOG(I("Writer") + partition);
                return AsyncRUN(command, subcommand);
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
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -e " + partition;
            if (debug) { LOG("===ERASE PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 50;
                LOG(I("Eraser") + partition);
                return AsyncRUN(command, subcommand);
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
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -gpt";
            try
            {
                GPTTABLE = new Dictionary<string, int[]>();
                if (debug) { LOG("===READ GPT===" + newline + newline + command + newline + subcommand); }
                if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
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
            if (debug) { LOG("===UNLOCK FRP===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 30;
                LOG(I("Unlocker") + "FRP: Universal" + newline + "GETGPT: INIT!");
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader)) { LOG(E("FailFrpD")); return false; }

                bool status = false;
                if (GPTTABLE.ContainsKey("devinfo"))
                {
                    LOG(I("Writer") + "DEVINFO Write lenght: " + GPTTABLE["frp"][1]);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -b devinfo " + "Tools/frpUnlocked.img";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(I("FailFrpD") + "NO DEVINFO Partition CONTINUING!!!");

                if (GPTTABLE.ContainsKey("frp"))
                {
                    progr.Value = 50;
                    cur = GPTTABLE["frp"][1];
                    LOG(I("Eraser") + "FRP erase lenght: " + GPTTABLE["frp"][1]);
                    command = "Tools\\emmcdl.exe";
                    subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -e frp";
                    status = SyncRUN(command, subcommand);
                }
                else
                    LOG(I("FailFrpD") + "NO FRP Partition!!!");

                return status;
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
            string subcommand = "--port=\\\\.\\" + TxSide.ComName + " --sendxml=" + '"' + xml + '"' + " --search_path=" + '"' + path + '"';
            if (debug) { LOG("===Flash Partitions XML===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 0;
                LOG(I("Flasher") + path);
                return AsyncRUN(command, subcommand);
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
            string subcommand = " --port=\\\\.\\" + TxSide.ComName + " --sendimage=" + '"' + file + '"' + " --noprompt --showpercentagecomplete --zlpawarehost=1 --memoryname=eMMC";
            if (debug) { LOG("===Flash Partitions RAW===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 0;
                LOG("INFO: Flashing EMMC IMAGE: " + file);
                LOG("INFO: First Init takes more time for sparse img before Flash: ");
                return AsyncRUN(command, subcommand);
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
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -d 0 " + GPTTABLE["userdata"][0] + " -o " + '"' + savepath + '"';
            if (debug) { LOG("===DUMPER===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 30;
                LOG(I("DumpTr") + newline);
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(E("Unknown")); return false; }
                LOG(I("Dump") + "0" + "<-TO->" + GPTTABLE["userdata"][0]);
                cur = GPTTABLE["userdata"][0];
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
            }
            return false;
        }
        public static bool Dump(int i, int o, string partition, string loader, string savepath)
        {
            progr.Value = 2;
            string command = "Tools\\emmcdl.exe";
            string subcommand = "-p " + TxSide.ComName + " -f " + '"' + loader + '"' + " -d " + i + " " + o + " -o " + '"' + savepath + '"' + "\\" + partition;
            if (debug) { LOG("===DUMPER PARTITION===" + newline + newline + command + newline + subcommand); }
            if (!LoadLoader("PHONE", loader)) { loadedhose = false; LOG(E("Fail")); return false; }
            try
            {
                progr.Value = 30;
                if (GPTTABLE.Count == 0)
                    if (!ReadGPT(loader))
                    { LOG(E("Unknown")); return false; }
                LOG(I("DumpTp") + partition + newline);
                cur = GPTTABLE[partition][1];
                return AsyncRUN(command, subcommand);
            }
            catch (Exception e)
            {
                if (debug) LOG(e.ToString());
            }
            return false;
        }
    }
}
