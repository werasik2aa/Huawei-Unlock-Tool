using HuaweiUnlocker.TOOLS;
using static HuaweiUnlocker.LangProc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace HuaweiUnlocker.DIAGNOS
{
    public class DIAG
    {
        public static string DBDA = "NaN", PCUI = "NaN";
        public static byte[] AT_SEND(string CMD)
        {
            if (PCUI == "NaN") return null;
            if (SerialManager.Open(PCUI, 115200, false))
                SerialManager.Write(CMD, false, false);

            return SerialManager.Read(false, "");
        }
        public static byte[] DIAG_SEND(byte[] data, bool DBside, bool crc)
        {
            if ((DBside ? DBDA : PCUI) == "NaN") return null;
            if (SerialManager.Open(DBside ? DBDA : PCUI, 115200, false))
                SerialManager.Write(data, crc);
            return SerialManager.Read(false, "");
        }
        public static byte[] DIAG_SEND(DataS.CMD_PKT data, bool DBside, bool crc)
        {
            if ((DBside ? DBDA : PCUI) == "NaN") return null;
            if (SerialManager.Open(DBside ? DBDA : PCUI, 115200, false))
                SerialManager.Write(data.GetBytes(), crc);
            return SerialManager.Read(false, "");
        }
        public static string RandomUnlockCode()
        {
            Random e = new Random(6666);
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16).Select(s => s[e.Next(s.Length)]).ToArray());
        }
        public static bool REBOOT()
        {
            LOG(0, "REBOOT works([BoardFactory]->FASTBOOT [ServiceROM]->SYSTEM)");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(0, "3A");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                LOG(0, "RbQC");
            else
                LOG(2, "ERbQC");
            return DataS.GetStatus(status);
        }
        public static bool SW_PCUI_TODIAG()
        {
            LOG(0, "SW_PCUI_TODIAG()");
            byte[] status = AT_SEND("AT$QCDMG=115200\r");
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
        public static bool To_Three_Recovery()
        {
            LOG(0, "Rebooting to UPGRADE mode");
            bool status = SW_PCUI_TODIAG();
            if (status)
            {
                DataS.CMD_PKT pkt = new DataS.CMD_PKT(0, "3A");
                if (DataS.GetStatus(DIAG_SEND(pkt, false, true)))
                    LOG(0, "RbQC1");
                else
                    LOG(2, "ERbQC1");
            }
            else
                LOG(2, "ESwQC", "ERbQC1");
            return status;
        }
        public static byte[] TEST()
        {
            return AUTH_PHONE(CRC.EncryptData("1.pem", CRC.HexStringToBytes(READ_SECRET_KEY())));
        }
        public static void THREE_RECOVERY_WRITE(string path, string img, string authcode)
        {
            if (!String.IsNullOrEmpty(path))
            {
                LOG(0, "Auth code in header of Update.APP (HW8986яяяяя) example in hex");
                LOG(0, "SEnd header");
                byte[] filedata = File.ReadAllBytes(path + "\\"+ img + ".header");
                DIAG_SEND(filedata, false, true);

                //NEED REWORK CRC TABLE AND PARTITIONALY SEND FILES
                LOG(0, "Send File");
                filedata = File.ReadAllBytes(path + "\\" + img + ".img");
                LOG(0, "Send File");
                DIAG_SEND(filedata, false, true);
            }
        }
        public static string READ_TIME_IMSI()
        {
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(2, "00");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return Encoding.ASCII.GetString(status);
            return "NoData";
        }
        public static string READ_IMEI_1()
        {
            LOG(0, "RwImei");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(6, "26", "2602");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
            {
                string don = "";
                string info = CRC.HexToIMEI(CRC.BytesToHexString(status));
                for (int i = 7; i != 22; i++) don += info[i];
                return don;
            }
            return "NoData";
        }
        public static string READ_IMEI_2()
        {
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(16, "4b", "28", "EE", "01000000");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
            {
                string don = "";
                string info = CRC.HexToIMEI(CRC.BytesToHexString(status.Skip(4).Take(status.Length - 4).ToArray()));
                for (int i = 7; i != 22; i++) don += info[i];
                return don;
            }
            return "Need_Auth";
        }
        public static string READ_WIFI_MAC()
        {
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(6, "26", "4612");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return CRC.HexToIMEI(CRC.BytesToHexString(status.Skip(3).Take(status.Length - 4).ToArray()));
            return "Need_Auth";
        }
        public static string READ_BLTU_MAC()
        {
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(6, "26", "BF01");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return CRC.HexToIMEI(CRC.BytesToHexString(status.Skip(3).Take(status.Length - 4).ToArray()));
            return "Need_Auth";
        }
        public static string READ_COUNTRY_CODE()
        {
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(78, "4B", "C9", "67FF");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return CRC.HexToIMEI(CRC.BytesToHexString(status.Skip(3).Take(status.Length - 4).ToArray()));
            return "Need_Auth";
        }
        public static string[] READ_BSN_BUILD_ID()
        {
            LOG(0, "RwBSN");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(262, "26", "7200");
            byte[] status = DIAG_SEND(pkt, true, true);
            string[] f = new string[2];
            if (DataS.GetStatus(status))
            {
                f[0] = Encoding.ASCII.GetString(status.Skip(3).Take(16).ToArray());
                f[1] = Encoding.ASCII.GetString(status.Skip(28).Take(16).ToArray());
            }
            else
                f[0] = f[1] = "Need_Auth";
            return f;
        }
        public static byte[] WRITE_CMD_STRANGE_AVOID(string data, int len = 0)
        {
            LOG(0, "Strange data: " + data + " ???WORK AROUND???");
            while (data.Length < len) data += "00";
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(80 + data.Length - 8, "4B", "C9", "54EE", data + "000000000000000000000000000000000000000000000000000000000004");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return status;
            return null;
        }
        public static string[] READ_FIRMWARE_INFO()
        {
            LOG(0, "RwFirmwareInfo");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(72, "4B", "C9", "3CFF");
            byte[] status = DIAG_SEND(pkt, true, true);
            string[] f = new string[1];
            if (DataS.GetStatus(status))
            {
                status = CRC.HexStringToBytes(CRC.BytesToHexString(status.Skip(4).Take(status.Length - 8).ToArray()).Replace("00", "").Trim('0'));
                string NewData = Encoding.ASCII.GetString(status).Replace(" ", newline).Replace("_SBL1", "_SBL1" + newline).Replace("_APPSBOOT", "_APPSBOOT" + newline).Replace("_BOOT", "_BOOT" + newline).Replace("MSM_PRO", "MSM_PRO" + newline);
                f = NewData.Split('\n');
                return f;
            }
            else
                f[0] = "Need_Auth";

            return f;
        }
        public static string READ_SECRET_KEY()
        {
            LOG(0, "RwRSA");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(72, "4B", "C9", "EDEE");
            byte[] status = DIAG_SEND(pkt, true, true);
            if (DataS.GetStatus(status))
                return CRC.BytesToHexString(status.Skip(4).Take(status.Length - 8).ToArray()).TrimEnd('0');
            return "Need_Auth";
        }
        public static byte[] AUTH_PHONE(byte[] hexdata)
        {
            LOG(0, "Trying Authentitcate PHONE...");
            return AUTH_PHONE(CRC.BytesToHexString(hexdata));
        }
        public static byte[] AUTH_PHONE(string hexdata)
        {
            LOG(0, "Trying Authentitcate PHONE...");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(1040, "4B", "C9", "CCEE", hexdata);
            byte[] status = DIAG_SEND(pkt, true, true);
            DataS.GetStatus(status);
            return status;
        }
        public static bool REWRITE_BOOTLOADER_KEYQC(string key, string donglename, string dongledatarsaoraespksx)
        {
            LOG(0, "Trying Write Code...");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(1260, "4B", "C9", "75EE", "00000800" + donglename + "00000000000000001027010001000008" + key + "0000000000000000000000000000000000000000000000000000000000000000" + dongledatarsaoraespksx);
            byte[] status = DIAG_SEND(pkt, true, true);
            return DataS.GetStatus(status);
        }
        public static bool FACTORY_RESET_MNF(int autoreboot)
        {
            if (autoreboot != 1 && autoreboot != 0)
            {
                LOG(2, "Reboot Should be state 0 or 1");
                return false;
            }
            LOG(0, "Trying Factory Reset...");
            DataS.CMD_PKT pkt = new DataS.CMD_PKT(70, "4B", "C9", "28FF", "0" + autoreboot);
            byte[] status = DIAG_SEND(pkt, true, true);
            return DataS.GetStatus(status);
        }
    }
}
