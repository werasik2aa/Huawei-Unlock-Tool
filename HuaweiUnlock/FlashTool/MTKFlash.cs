using HuaweiUnlocker.DIAGNOS;
using System;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.FlashTool
{
    public class MTKFlash
    {
        public static bool BromUnl()
        {
            return true;
        }

        public static bool FlashScatter(string ScatterPath, string DaPath, string DL_MODE, bool brom)
        {
            string command = "Tools\\mtkflash.exe";
            string subcommand = "-scatter " + ScatterPath + " -da " + DaPath + " -dl_type " + DL_MODE;
            if (debug) LOG(0, "==== MTK FLASH ====");
            if (!SyncRUN(command, subcommand))
                LOG(1, "ERR");
            return true;
        }
        public static bool ReadScatter(string ScatterPath)
        {
            if (debug) LOG(0, "==== READ SCATTER ====");
            return true;
        }
    }
}
