using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                            MISC.LOG("REQUEST: " + CMD.REQ.HW_CMD + i.ToString("X"));
                            MISC.LOG("RESPONSE: " + CRC.HexDump(msg));
                        }
                    }
                }
            }
        }
        public static void AUTH()
        {
            MISC.LOG("======AUTH BEGIN=====");
            MISC.LOG("Trying to AUTH PHONE!");
            MISC.LOG("GETTING RSA!");
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_DIAG_RSA_KEY, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg.Length > 30)
                {
                    byte[] RSAmaybe = msg.Skip(8).ToArray();
                    for (int i = 0; i != 5; i++)
                        RSAmaybe[RSAmaybe.Length - 1 - i] = 00;
                    MISC.LOG(CRC.HexDump(msg));
                    MISC.LOG("Trying to AUTH PHONE!");
                    status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_AUTHTKN+CRC.ByteArrayToString(RSAmaybe), CMD.LENDB, true, true);
                    if (status)
                        MISC.LOG(CRC.HexDump(READ()));
                }
            }
            MISC.LOG("======AUTH END=====");
        }
        public static void SW_PCUI_TODIAG()
        {
            bool status = AT_SEND(CMD.PCUI.GOTO_QCDMG);
            if (status)
            {
                if (Encoding.ASCII.GetString(READ()).ToLower().Contains("ok"))
                    MISC.LOG(MISC.I("SwQC"));
                else
                    MISC.LOG(MISC.E("ESwQC") + "or it's already in QCDMG");
            }
            else
                MISC.LOG(MISC.E("ESwQC"));
        }
        public static void REBOOT()
        {
            bool status = DIAG_SEND(CMD.REQ.REBOOT_OR_3RECOVERY, "", "", true, true);
            if (status)
            {
                MISC.LOG(MISC.I("RbQC"));
            }
            else
                MISC.LOG(MISC.E("ERbQC"));
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
                        MISC.LOG(MISC.I("SwQC"));
                    if (msg == "")
                        MISC.LOG(MISC.I("ASwQC"));
                    status = DIAG_SEND(CMD.REQ.GET_NV, "00" + CMD.LENPC, "", false, true) && DIAG_SEND(CMD.REQ.GET_NV, CMD.DBADAPTER.NV_FIRMWAREINFO, "00", false, true);
                    if (status)
                    {
                        status = DIAG_SEND(CMD.REQ.REBOOT_OR_3RECOVERY, "", "", false, true);
                        if (status)
                            MISC.L("RbQC1");
                    }
                    else
                        MISC.L("ERbQC1");

                }
                else
                    MISC.LOG(MISC.E("ESwQC"));
            }
            else
                MISC.LOG(MISC.E("ESwQC"));
            return status;
        }
        public static string GET_IMEI1()
        {
            MISC.LOG(MISC.I("RwImei"));
            bool status = DIAG_SEND(CMD.REQ.GET_NV, CMD.DBADAPTER.IMEI, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg != null)
                {
                    string imeiraw = CRC.HexToIMEI(CRC.BytesToHexString(msg));
                    string info = "";
                    for (int i = 7; i != 22; i++) info += imeiraw[i];
                    if (MISC.debug)
                    {
                        MISC.LOG("=================GET_IMEI1()=====================");
                        MISC.LOG("HEX: " + CRC.HexDump(msg));
                        MISC.LOG("STRING: " + info);
                        MISC.LOG("=================================================");
                    }
                    return info;
                }
            }
            return "NaN";
        }
        public static string[] GET_FIRMWAREINFO()
        {
            MISC.LOG(MISC.I("RwFirmwareInfo"));
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
                    if (MISC.debug)
                    {
                        MISC.LOG("=================GET_FIRMWAREINFO()=====================");
                        MISC.LOG("HEX: " + CRC.HexDump(msg));
                        MISC.LOG("STRING: " + info);
                        MISC.LOG("=====================================================");
                    }
                }
            }
            return data;
        }
        public static string[] GET_BOARDINFO()
        {
            MISC.LOG(MISC.I("RwBSN"));
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
                    if (MISC.debug)
                    {
                        MISC.LOG("=================GET_BOARDINFO()=====================");
                        MISC.LOG("HEX: " + CRC.HexDump(msg));
                        MISC.LOG("STRING: " + info);
                        MISC.LOG("=====================================================");
                    }
                }
            }
            return data;
        }
        public static string GET_SECRET_KEY_CRYPTED()
        {
            MISC.LOG(MISC.I("RwRSA"));
            bool status = DIAG_SEND(CMD.REQ.HW_CMD, CMD.DBADAPTER.OEM_DIAG_RSA_KEY, CMD.LENDB, true, true);
            if (status)
            {
                byte[] msg = READ();
                if (msg != null)
                {
                    string info = CRC.GetASCIIString(msg);
                    if (MISC.debug)
                    {
                        MISC.LOG("=================GET_SECRET_KEY_CRYPTED()=====================");
                        MISC.LOG("HEX: " + CRC.HexDump(msg));
                        MISC.LOG("STRING: " + info);
                        MISC.LOG("==============================================================");
                    }
                    return info;
                }
            }
            return "FailedTo Read Key Please AUTH!";
        }
    }
}
