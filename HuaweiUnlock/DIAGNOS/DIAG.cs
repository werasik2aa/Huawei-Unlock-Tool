using HuaweiUnlocker.TOOLS;
using static HuaweiUnlocker.LangProc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace HuaweiUnlocker.DIAGNOS
{
    public class DIAG
    {
        public string DBDA = "NaN", PCUI = "NaN";
        public byte[] AT_SEND(string CMD)
        {
            if (PCUI == "NaN") return null;
            if (SerialManager.OpenPort(PCUI, false))
                SerialManager.Write(CMD, false, false);

            return SerialManager.Read(false, "");
        }
        public byte[] DIAG_SEND(string CMD, string subCMD, int len, bool DBside, bool crc)
        {
            string zeros = "";
            while (zeros.Length < len) zeros += "00";
            if ((DBside ? DBDA : PCUI) == "NaN") return null;
            if (SerialManager.OpenPort((DBside ? DBDA : PCUI), false))
                SerialManager.Write(CMD + subCMD + zeros, crc, true);
            return SerialManager.Read(false, "");
        }
        public string RandomUnlockCode()
        {
            Random e = new Random(6666);
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16).Select(s => s[e.Next(s.Length)]).ToArray());
        }
        public byte[] AUTHCODE(byte[] data)
        {
            SHA256 sha256 = SHA256.Create();
            RSA rsa = RSA.Create();
            byte[] SignedHash = new byte[256];
            try
            {
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                RSAFormatter.SetHashAlgorithm("SHA256");
                SignedHash = RSAFormatter.CreateSignature(sha256.ComputeHash(data));
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
            return SignedHash;
        }
        public bool FACTORY_RESET()
        {
            LOG("=================FACTORY_RESET()=====================");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_FACTORY_RESET, 60, true, true);
            return CMDS.GetStatus(status);
        }
        public bool SW_PCUI_TODIAG()
        {
            LOG("=================SW_PCUI_TODIAG()=====================");
            byte[] status = AT_SEND(CMDS.PCUI.AT_GOTO_QCDMG);
            string msg = CRC.GetASCIIString(status).ToLower();
            if (msg.Contains("ok") || msg == "" && !msg.Contains("error"))
            {
                if (msg.Contains("ok"))
                    LOG(I("SwQC"));
                if (msg == "")
                    LOG(I("ASwQC"));
            }
            else
                LOG(E("ESwQC"));
            return msg.Length >= 2;
        }
        public bool REBOOT()
        {
            LOG("=================REBOOT()=====================");
            byte[] status = DIAG_SEND(CMDS.REBOOT_OR_3RECOVERY, "", 0, true, true);
            if (CMDS.GetStatus(status))
                LOG(I("RbQC"));
            else
                LOG(E("ERbQC"));
            return CMDS.GetStatus(status);
        }
        public bool To_Three_Recovery(string path, string authcode)
        {
            LOG(debug ? "=================To_Three_Recovery()=====================" : "Rebooting to UPGRADE mode");
            bool status = SW_PCUI_TODIAG();
            if (status)
            {
                if (CMDS.GetStatus(DIAG_SEND(CMDS.REBOOT_OR_3RECOVERY, "", 0, false, true)))
                    LOG(I("RbQC1"));
                else
                    LOG(E("ERbQC1"));
                if (!String.IsNullOrEmpty(path))
                {
                    byte[] filedata = File.ReadAllBytes(path);
                    DIAG_SEND(CRC.BytesToHexString(filedata), "", 0, false, true);
                }
            }
            else
            {
                LOG(E("ESwQC"));
                LOG(E("ERbQC1"));
            }

            return status;
        }
        public string GET_IMEI1()
        {
            LOG(debug ? "=================GET_IMEI1()=====================" : I("RwImei"));
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
            LOG(debug ? "=================GET_FIRMWAREINFO()=====================" : I("RwFirmwareInfo"));
            byte[] status = DIAG_SEND(CMDS.GET_NV, CMDS.DBADAPTER.NV_FIRMWAREINFO, CMDS.LENDB, true, true);
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
            LOG(debug ? "=================GET_BOARDINFO()=====================" : I("RwBSN"));
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
            if (debug) LOG(debug ? "=================GET_SECRET_KEY_CRYPTED()=====================" : I("RwRSA"));
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_GET_DIAG_KEY, CMDS.LENDB, true, true);
            if (CMDS.GetStatus(status))
            {
                byte[] encrypted = status.Skip(4).ToArray();
                Array.Resize<byte>(ref encrypted, encrypted.Length - 7);
                return encrypted;
            }
            return null;
        }
        public bool OEM_AUTH_WITH(byte[] data)
        {
            if (debug) LOG("=================OEM_AUTH_WITH()=====================");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DBADAPTER.OEM_AUTH_WITH + CRC.BytesToHexString(data), 512, true, true);
            return status.Length >= 40;
        }
        public bool OEM_REWRITE_KEY(string KEY)
        {
            if (debug) LOG("=================OEM_REWRITE_KEY()=====================");
            RSACryptoServiceProvider v = new RSACryptoServiceProvider(128);
            v.FromXmlString("<RSAKeyValue><Modulus>" + File.ReadAllText("Tools/key.pub").Replace(" @unknown", "") + "</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            byte[] status = DIAG_SEND(CMDS.HW_CMD, CMDS.DONGLEINFO + CRC.BytesToHexString(Encoding.ASCII.GetBytes(KEY)) + CMDS.DONGLEAUTH + CRC.BytesToHexString(v.Encrypt(Encoding.ASCII.GetBytes("OCTOPLUS"), RSAEncryptionPadding.Pkcs1)), 510, true, true);
            return CMDS.GetStatus(status);
        }
    }
}
