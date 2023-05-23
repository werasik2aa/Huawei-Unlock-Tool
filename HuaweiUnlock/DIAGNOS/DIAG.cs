using HuaweiUnlocker.TOOLS;
using static HuaweiUnlocker.LangProc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using HuaweiUnlocker.UI;

namespace HuaweiUnlocker.DIAGNOS
{
    public class DIAG
    {
        public string DBDA = "NaN", PCUI = "NaN";
        public byte[] AT_SEND(string CMD)
        {
            if (PCUI == "NaN") return null;
            if (SerialManager.Open(PCUI, 115200, false))
                SerialManager.Write(CMD, false, false);

            return SerialManager.Read(false, "");
        }
        public byte[] DIAG_SEND(string CMD, string subCMD, int len, bool DBside, bool crc)
        {
            string zeros = "";
            while (zeros.Length < len) zeros += "00";
            if ((DBside ? DBDA : PCUI) == "NaN") return null;
            if (SerialManager.Open((DBside ? DBDA : PCUI), 115200, false))
                SerialManager.Write(CMD + subCMD + zeros, crc, true);
            return SerialManager.Read(false, "");
        }
        public string RandomUnlockCode()
        {
            Random e = new Random(6666);
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16).Select(s => s[e.Next(s.Length)]).ToArray());
        }
        public string TestHack()
        {
            LOG(-1, "================MANUFACTURE HACK PORT Custom METHOD================");
            //86217804396518625011
            var a = Encoding.ASCII.GetBytes("86217804396518625011");
            var b = GET_SECRET_KEY_CRYPTED();
            return CRC.HexDump(OEM_AUTH_WITH(HDLC.Encode(CRC.EncryptData("1.cer", a), false)));
        }
        public void HACKdbPort()
        {
            LOG(-1, "================MANUFACTURE HACK PORT OLD HCU METHOD================");
            byte[] status = DIAG_SEND("", "4BC954EE4BC9EDEE00000000000000000000000000000000000000000000000000000000000400005E197E", 0, true, false);
            if (status != null) LOG(-1, CRC.BytesToHexString(status));
            status = DIAG_SEND("", "4BC954EE4BC9B2EE000000000000000000000000000000000000000000000000000000000004000007D97E", 0, true, false);
            if(status != null) LOG(-1, CRC.BytesToHexString(status));
        }
        public bool FACTORY_RESET()
        {
            LOG(-1, "=================FACTORY_RESET()=====================");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_FACTORY_RESET, 60, true, true);
            return CMDS.GetStatus(status);
        }
        public bool SW_PCUI_TODIAG()
        {
            LOG(-1, "=================SW_PCUI_TODIAG()=====================");
            byte[] status = AT_SEND(CMDS.PCUI.AT_GOTO_QCDMG);
            string msg = CRC.GetASCIIString(status).ToLower();
            if (msg.Contains("ok") || msg == "" && !msg.Contains("error"))
            {
                if (msg.Contains("ok"))
                    LOG(0, "SwQC");
                if (msg == "")
                    LOG(1, "ASwQC");
            }
            else
                LOG(2, "ESwQC");
            return msg.Length >= 2;
        }
        public bool REBOOT()
        {
            LOG(-1, "=================REBOOT()=====================");
            byte[] status = DIAG_SEND(CMDS.REBOOT_OR_3RECOVERY, "", 0, true, true);
            if (CMDS.GetStatus(status))
                LOG(0, "RbQC");
            else
                LOG(2, "ERbQC");
            return CMDS.GetStatus(status);
        }
        public bool To_Three_Recovery(string path, string authcode)
        {
            LOG(-1, debug ? "=================To_Three_Recovery()=====================" : "Rebooting to UPGRADE mode");
            bool status = SW_PCUI_TODIAG();
            if (status)
            {
                if (CMDS.GetStatus(DIAG_SEND(CMDS.REBOOT_OR_3RECOVERY, "", 0, false, true)))
                {
                    LOG(0, "RbQC1");
                    if (!String.IsNullOrEmpty(path))
                    {
                        byte[] filedata = File.ReadAllBytes(path);
                        DIAG_SEND(CRC.BytesToHexString(filedata), "", 0, false, true);
                    }
                }
                else
                    LOG(2, "ERbQC1");
            }
            else
                LOG(2, "ESwQC", "ERbQC1");

            return status;
        }
        public string GET_IMEI1()
        {
            LOG(-1, debug ? "=================GET_IMEI1()=====================" : "RwImei");
            byte[] status = DIAG_SEND(CMDS.GET_NV, CMDS.DBADAPTER.NV_IMEI, CMDS.LENDB, true, true);
            if (CMDS.GetStatus(status))
            {
                string imeiraw = CRC.HexToIMEI(CRC.BytesToHexString(status));
                string info = "";
                for (int i = 7; i != 22; i++) info += imeiraw[i];
                return info;
            }
            return "NaN";
        }
        public string[] GET_FIRMWAREINFO()
        {
            LOG(-1, debug ? "=================GET_FIRMWAREINFO()=====================" : "RwFirmwareInfo");
            byte[] status = DIAG_SEND( CMDS.GET_NV, CMDS.DBADAPTER.NV_FIRMWAREINFO, CMDS.LENDB, true, true);
            string[] data = new string[2];
            data[1] = data[0] = "";
            if (CMDS.GetStatus(status))
            {
                string info = CRC.GetASCIIString(status);
                for (int i = 0; i != 16; i++)
                {
                    data[0] += info[2 + i];
                    data[1] += info[18 + i];
                }
            }
            return data;
        }
        public string[] GET_BOARDINFO()
        {
            LOG(-1, debug ? "=================GET_BOARDINFO()=====================" : "RwBSN");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_BOARDINFO, CMDS.LENDB, true, true);
            string[] data = new string[5];
            data[1] = data[0] = "NaN";
            if (CMDS.GetStatus(status))
            {
                data[0] = "";
                data[1] = "";
                string info = CRC.GetASCIIString(status);
                for (int i = 0; i != 16; i++)
                {
                    data[0] += info[4 + i];
                    data[1] += info[20 + i];
                }
            }
            return data;
        }
        public byte[] GET_SECRET_KEY_CRYPTED()
        {
            LOG(-1, debug ? "=================GET_SECRET_KEY_CRYPTED()=====================" : "RwRSA");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_GET_DIAG_KEY, CMDS.LENDB, true, true);
            if (CMDS.GetStatus(status))
            {
                byte[] encrypted = status.Skip(4).ToArray();
                Array.Resize<byte>(ref encrypted, encrypted.Length - 7);
                return encrypted;
            }
            return null;
        }
        public byte[] OEM_AUTH_WITH(byte[] data)
        {
            if (debug) LOG(-1, "=================OEM_AUTH_WITH()=====================");
            return DIAG_SEND(CMDS.HW_CMD + CMDS.DBADAPTER.OEM_AUTH_WITH, CRC.BytesToHexString(data), CMDS.LENRSA2, true, true);
        }
        public bool OEM_REWRITE_KEY(string KEY)
        {
            if (debug) LOG(-1, "=================OEM_REWRITE_KEY()=====================");
            byte[] status = DIAG_SEND(CMDS.HW_CMD + CMDS.DBADAPTER.OEM_REWRITE_KEY, CMDS.DONGLEINFO + CRC.EncryptData("1.cer", Encoding.ASCII.GetBytes(KEY)), CMDS.LENKEY, true, true);
            return CMDS.GetStatus(status);
        }
    }
}
