namespace HuaweiUnlocker.DIAGNOS
{
    public class DataS
    {
        public const string MTK_dloadonly = "dl_only";
        public const string MTK_firmwareupgrade = "firmware_upgrade";
        public const string MTK_formatallanddload = "fm_and_dl";
        public const string MTK_formatall = "fm";
        public struct GPT_Struct
        {
            public int StartAdress;
            public int Length;
            public string ValueString;
            public GPT_Struct(int startAdress = -1, int length = -1, string value = "")
            {
                StartAdress = startAdress;
                Length = length;
                ValueString = value;
            }
        }
        public struct CMD_PKT
        {
            public string CMD;
            public string SUB_CMD;
            public string SUB_CMD2;
            public string DATA;
            public CMD_PKT(int ALLLEN, string Cmd, string SUB = "", string SUB2 = "", string data = "")
            {
                CMD = Cmd;
                SUB_CMD = SUB;
                SUB_CMD2 = SUB2;
                DATA = data;
                while (GetString().Length < ALLLEN) DATA += "00";
            }
            public byte[] GetBytes()
            {
                return CRC.HexStringToBytes(CMD + SUB_CMD + SUB_CMD2 + DATA);
            }
            public string GetString()
            {
                return CMD + SUB_CMD + SUB_CMD2 + DATA;
            }
        }
        public static bool GetStatus(byte[] buff)
        {
            if (buff.Length == 0 && buff != null) return false;
            string hex = CRC.BytesToHexString(buff);
            bool fail = false;
            if (fail = hex.StartsWith("15"))
                LangProc.LOG(2, "Command Not supported or Wrong or CRC WRONG [BAD_CMD]");
            else if (fail = hex.StartsWith("13"))
                LangProc.LOG(2, "Please Authentiticate phone First!");
            else if(fail = hex.StartsWith("14"))
                LangProc.LOG(2, "Command Not supported [BAD_CMD]");
            else if (fail = hex.StartsWith("4BC9CCEE0400E4CCF0BE00"))
                LangProc.LOG(2, "Firmware Editing is unavailable! Please Remove firmware protection! Need to LSM_INIT");
            return !fail;
        }
        public static string GetStatusStr(byte[] buff)
        {
            if (buff.Length <= 0 || buff == null) return "[ERROR] No Response";
            if (buff[0] == 19 || buff[0] == 21) return "[ERROR] Acces Denied or Command Wrong";
            return "[INFO] Success";
        }
    }
}
