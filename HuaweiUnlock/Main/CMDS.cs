using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiUnlocker
{
    public class CMD
    {
        public static string LENDB = "0000000000000000000000000000000000000000000000000000000000000000";
        public static string AUTHL = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        public static string LENPC = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        public struct Port_D
        {
            public string ComName;
            public string DeviceName;
        }
        public class REQ
        {
            public static string REBOOT_OR_3RECOVERY = "3A";
            public static string SET_STATE = "29";
            public static string HW_CMD = "4BC9";
            public static string GET_NV = "26";
            public static string GET_TIME = "00";
        }
        public class PCUI
        {
            public static string GET_INFO = "ATI\r";
            public static string GOTO_QCDMG = "AT$QCDMG=115200\r";
            public static string REBOOT_3RECOVERY = "3A";
            public static string PCUI_UNKNOWN = "2600";
            public static string PCUI_UNKNOWN2 = "2672";
        }
        public class DBADAPTER
        {
            public static string OEM_DIAG_RSA_KEY = "EDEE";
            public static string OEM_BOARDINFO = "3CFF";
            public static string NV_FIRMWAREINFO = "72";
            public static string OEM_AUTH = "31FF";
            public static string OEM_AUTHTKN = "CCEE";
            public static string OME_FACTORY_RESET = "28FF01";
            public static string REBOOT = "3A";
            public static string IMEI = "2602";
            public static string TIME_DATA = "0078F07E";
        }
    }
}
