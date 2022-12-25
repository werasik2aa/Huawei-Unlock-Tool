using Microsoft.VisualBasic.Logging;
using System;
using System.Linq;
using System.Text;

namespace HuaweiUnlocker.Utils
{
    public class DIAG
    {
        public static string DBDA = "NaN", PCUI = "NaN";
        public static bool AT_SEND(string cmd)
        {
            if (PCUI == "NaN") return false;
            if (SerialManager.OpenPort(PCUI, false))
                return SerialManager.Write(cmd, false, false);
            else return false;
        }
        public static bool DIAG_SEND(string cmd, string subcmd, string len, bool DBside, bool crc)
        {
            if ((DBside ? DBDA : PCUI) == "NaN") return false;
            if (SerialManager.OpenPort((DBside ? DBDA : PCUI), false))
                return SerialManager.Write(cmd + subcmd + len, crc, true);
            return false;
        }
        public static byte[] READ()
        {
            return SerialManager.Read(false, "");
        }
        public static void BROOTFORCE_HW_CMD()
        {
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.IMEI, CMD.LENDB, true, true);
            if (status)
            {
                for(Int32 i = 0x00; i != 0xFFFF; i++)
                {
                    byte[] cmd = CRC.HexStringToBytes(CMD.REQ.HW_CMD + i.ToString("X"));
                    status = DIAG_SEND(CMD.REQ.HW_CMD, i.ToString("X"), CMD.LENDB, true, true);
                    if (status)
                    {
                        byte[] msg = READ();
                        if (msg != null && cmd.Length >= msg.Length)
                        {
                            LangProc.LOG("REQUEST: " + CMD.REQ.HW_CMD + i.ToString("X"));
                            LangProc.LOG("RESPONSE: " + CRC.HexDump(msg));
                        }
                    }
                }
            }
        }
        public static string AUTH()
        {
            LangProc.LOG("======AUTH BEGIN=====");
            LangProc.LOG("Trying to AUTH PHONE!");
            LangProc.LOG("GETTING RSA!");
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_DIAG_RSA_KEY, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg.Length > 30)
                {
                    byte[] RSAmaybe = msg.Skip(8).ToArray();
                    for (int i = 0; i != 5; i++)
                        RSAmaybe[RSAmaybe.Length - 1 - i] = 00;
                    LangProc.LOG(CRC.HexDump(msg));
                    LangProc.LOG("Trying to AUTH PHONE!");
                    status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_AUTHTKN+CRC.ByteArrayToString(RSAmaybe), CMD.LENDB, true, true);
                    if (status)
                    {
                        msg = READ();
                        if(msg != null)
                        {
                            LangProc.LOG(CRC.HexDump(msg));
                            return CRC.HexToIMEI(CRC.BytesToHexString(msg));
                        }
                    }
                }
            }
            LangProc.LOG("======AUTH END=====");
            return "NaN";
        }
        public static bool SW_PCUI_TODIAG()
        {
            bool status = AT_SEND(CMD.PCUI.GOTO_QCDMG);
            if (status)
            {
                var str = Encoding.ASCII.GetString(READ()).ToLower();
                status = str == "" || str == " " || str.Contains("ok");
                if (status)
                    LangProc.LOG(LangProc.E("ESwQC") + "or it's already in QCDMG : "+ LangProc.L("SwQC"));
            }
            else
                LangProc.LOG(LangProc.E("ESwQC"));
            return status;
        }
        public static bool REBOOT()
        {
            bool status = DIAG_SEND(CMD.REQ.REBOOT_OR_3RECOVERY, "", "", true, true);
            if (status)
                LangProc.LOG(LangProc.I("RbQC"));
            else
                LangProc.LOG(LangProc.E("ERbQC"));
            return status;
        }
        public static bool To_Three_Recovery()
        {
            bool status = AT_SEND(CMD.PCUI.GOTO_QCDMG);
            if (status)
            {
                string msg = CRC.GetASCIIString(READ()).ToLower();
                status = (msg.Contains("ok") || msg == "") && !msg.Contains("error");
                if (status)
                {
                    if (msg.Contains("ok"))
                        LangProc.LOG(LangProc.I("SwQC"));
                    if (msg == "")
                        LangProc.LOG(LangProc.I("ASwQC"));
                    status = DIAG_SEND(CMD.REQ.GET_NV, "00" + CMD.LENPC, "", false, true) && DIAG_SEND(CMD.REQ.GET_NV, CMD.DBADAPTER.NV_FIRMWAREINFO, "00", false, true);
                    if (status)
                    {
                        status = DIAG_SEND(CMD.REQ.REBOOT_OR_3RECOVERY, "", "", false, true);
                        if (status)
                            LangProc.L("RbQC1");
                    }
                    else
                        LangProc.L("ERbQC1");

                }
                else
                    LangProc.LOG(LangProc.E("ESwQC"));
            }
            else
                LangProc.LOG(LangProc.E("ESwQC"));
            return status;
        }
        public static string GET_IMEI1()
        {
            LangProc.LOG(LangProc.I("RwImei"));
            bool status = DIAG_SEND(CMD.REQ.GET_NV, CMD.DBADAPTER.IMEI, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg != null)
                {
                    string imeiraw = CRC.HexToIMEI(CRC.BytesToHexString(msg));
                    string info = "";
                    for (int i = 7; i != 22; i++) info += imeiraw[i];
                    if (LangProc.debug)
                    {
                        LangProc.LOG("=================GET_IMEI1()=====================");
                        LangProc.LOG("HEX: " + CRC.HexDump(msg));
                        LangProc.LOG("STRING: " + info);
                        LangProc.LOG("=================================================");
                    }
                    return info;
                }
            }
            return "NaN";
        }
        public static string[] GET_FIRMWAREINFO()
        {
            LangProc.LOG(LangProc.I("RwFirmwareInfo"));
            bool status = DIAG_SEND(CMD.REQ.GET_NV, CMD.DBADAPTER.NV_FIRMWAREINFO, CMD.LENDB, true, true);
            string[] data = new string[2];
            data[1] = data[0] = "";
            if (status)
            {
                byte[] msg = READ();
                if (msg != null)
                {
                    string info = CRC.GetASCIIString(msg);
                    for (int i = 0; i != 16; i++)
                    {
                        data[0] += info[2 + i];
                        data[1] += info[18 + i];
                    }
                    if (LangProc.debug)
                    {
                        LangProc.LOG("=================GET_FIRMWAREINFO()=====================");
                        LangProc.LOG("HEX: " + CRC.HexDump(msg));
                        LangProc.LOG("STRING: " + info);
                        LangProc.LOG("=====================================================");
                    }
                }
            }
            return data;
        }
        public static string[] GET_BOARDINFO()
        {
            LangProc.LOG(LangProc.I("RwBSN"));
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_BOARDINFO, CMD.LENDB + "62A77E", true, false);
            string[] data = new string[5];
            data[1] = data[0] = "NaN";
            if (status)
            {
                byte[] msg = READ();
                if (msg != null && msg.Length > 20)
                {
                    data[0] = "";
                    data[1] = "";
                    string info = CRC.GetASCIIString(msg);
                    for (int i = 0; i != 16; i++)
                    {
                        data[0] += info[4 + i];
                        data[1] += info[20 + i];
                    }
                    if (LangProc.debug)
                    {
                        LangProc.LOG("=================GET_BOARDINFO()=====================");
                        LangProc.LOG("HEX: " + CRC.HexDump(msg));
                        LangProc.LOG("STRING: " + info);
                        LangProc.LOG("=====================================================");
                    }
                }
            }
            return data;
        }
        public static string GET_SECRET_KEY_CRYPTED()
        {
            LangProc.LOG(LangProc.I("RwRSA"));
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_DIAG_RSA_KEY, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg != null)
                {
                    string info = CRC.GetASCIIString(msg);
                    if (LangProc.debug)
                    {
                        LangProc.LOG("=================GET_SECRET_KEY_CRYPTED()=====================");
                        LangProc.LOG("HEX: " + CRC.HexDump(msg));
                        LangProc.LOG("STRING: " + info);
                        LangProc.LOG("==============================================================");
                    }
                    return info;
                }
            }
            return "FailedTo Read Key Please AUTH!";
        }
    }
}
