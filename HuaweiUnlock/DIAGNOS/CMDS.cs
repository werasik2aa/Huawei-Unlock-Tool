using Microsoft.VisualBasic.Logging;
using System;
using System.Linq;

namespace HuaweiUnlocker.DIAGNOS
{
    public struct Port_D
    {
        public string ComName;
        public string DeviceName;
    }
    public struct Port_DP
    {
        public string Com1Name;
        public string DeviceName;
        public string Com2Name;
        public string Device2Name;
    }
    public class CMDS
    {
        public static bool GetStatus(byte[] buff)
        {
            if (buff.Length == 0) return false;
            return !(buff[0] == 19 || buff[0] == 21);
        }
        public static string REBOOT_OR_3RECOVERY = "3A";
        public static string SET_STATE = "29";
        public static string HW_CMD = "4BC9";
        public static string GET_NV = "26";
        public static string GET_TIME = "00";

        public static int LENDB = 64;
        public static int AUTHL = 512;
        public static int LENPC = 256;

        public static string DONGLEINFO = "75EE000008004F43544F504C555300000000000000001027010001000008";
        public static string DONGLEAUTH = "3E5827593A05CD3AE1CA0F8A61B1FAA78F1E187111BF34AAFC6ED23C6B965D7D5D000000000000000000000000000000000000000000000000000000000000000009";
        public struct PCUI
        {
            public static string AT_GET_INFO = "ATI\r";
            public static string AT_GOTO_QCDMG = "AT$QCDMG=115200\r";
            public static string PCUI_UNKNOWN = "2600";
            public static string PCUI_UNKNOWN2 = "2672";
        }
        public struct DBADAPTER
        {
            public static string OEM_GET_DIAG_KEY = "EDEE";
            public static string OEM_REWRITE_KEY = "75EE";
            public static string OEM_BOARDINFO = "3CFF";
            public static string OEM_WRITE_BSN = "4EFF";
            public static string OEM_AUTH_WITH = "CCEE";
            public static string OEM_FACTORY_RESET = "28FF01";
            public static string NV_IMEI = "2602";
            public static string NV_FIRMWAREINFO = "72";
            public static string ERASE_REGION_GUESS = "36FF";
            public static string UNKNOWN_0 = "31FF";
            public static string UNKNOWN_1 = "D6EE";
            public static string UNKNOWN_2 = "02EE";
            public static string UNKNOWN_3 = "55EE";
        }
    }
}
