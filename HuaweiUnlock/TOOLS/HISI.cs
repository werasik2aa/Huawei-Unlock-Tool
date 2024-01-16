using HuaweiUnlocker.DIAGNOS;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.TOOLS
{
    public class HISI
    {
        public static Fastboot fb = new Fastboot();
        public static string BSN = "NaN";
        public static string BNUM = "NaN";
        public static string AVER = "NaN";
        public static string MODEL = "NaN";
        public static string BLKEY = "NaN";
        public static string FBLOCKSTATE = "NaN";
        public static bool FBLOCK = false;
        public static void Disconnect()
        {
            fb.Disconnect();
        }
        public static void FlashBootloader(Bootloader bootloader, string port)
        {
            var flasher = new ImageFlasher();

            LOG(0, "HISIVerifyHash");

            int asize = 0, dsize = 0;

            foreach (var image in bootloader.Images)
            {
                if (!image.IsValid)
                {
                    throw new Exception($"Image `{image.Role}` is not valid!");
                }

                asize += image.Size;
            }

            LOG(0, "CPort", port);
            flasher.Open(port);

            LOG(0, "Writer", bootloader.Name);

            foreach (var image in bootloader.Images)
            {
                var size = image.Size;

                LOG(0, "EwPS", image.Role);

                flasher.Write(image.Path, (int)image.Address, x =>
                {
                    Progress(dsize, asize);
                });

                dsize += size;
            }

            flasher.Close();
        }
        public static bool ReadInfo()
        {
            if (!IsDeviceConnected(100)) {
                LOG(1, "NoDEVICEAnsw", " [HISI] Maybe hisi Loaders Wont boot"); return false;
            } //if timeout and no device
            GetASerial();
            GetModelProduct();
            GetModelBSN();
            GetBuildID();
            ReadAllMethods();
            return GetFBLockState();
        }
        public static string GetASerial()
        {
            string serial = fb.GetSerialNumber();
            LOG(0, "SerialnTag", serial);
            return AVER = serial;
        }
        public static string GetModelBSN()
        {
            Fastboot.Response bsn = fb.Command("oem read_bsn");
            if (bsn.Status == Fastboot.FastbootStatus.Ok)
            {
                LOG(0, "BSNTag", bsn.Payload);
                BSN = bsn.Payload;
            }
            return BSN;
        }
        public static string GetModelProduct()
        {
            if (!IsDeviceConnected()) return ""; //if timeout and no device
            Fastboot.Response model = fb.Command("oem get-product-model");
            LOG(0, "ModelTag", model.Payload);
            return MODEL = model.Payload;
        }
        public static string GetBuildID()
        {
            if (!IsDeviceConnected()) return ""; //if timeout and no device
            Fastboot.Response build = fb.Command("oem get-build-number");
            LOG(0, "BuildIdTag", build.Payload.Replace(":", ""));
            return BNUM = build.Payload.Replace(":", "");
        }
        public static bool GetFBLockState()
        {
            if (!IsDeviceConnected()) return true; //if timeout and no device
            Fastboot.Response fblock = fb.Command("oem lock-state info");
            bool state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
            if (!state)
            {
                fblock = fb.Command("oem backdoor info");
                state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
            }
            LOG(0, "FBLOCKTag", FBLOCKSTATE = (FBLOCK = state) ? "UNLOCKED" : "LOCKED");
            if (!state) LOG(2, "HISIInfoS");
            return FBLOCK;
        }
        public static void UnlockFRP()
        {
            if (!IsDeviceConnected()) return; //if timeout and no device
            LOG(0, "Unlocker", "FRP (BETA)");
            fb.Command("oem erase frp");
            fb.Command("oem erase-frp");
            fb.Command("oem unlock-frp");
            fb.Command("oem frp-erase");
            fb.Command("oem frp-unlock");
            fb.Command("oem format cache");
            fb.UploadData("Tools\\frpUnlocked.img", "devinfo");
            fb.UploadData("Tools\\frpPartition.img", "frp");
        }
        public static void SetNVMEProp(string prop, byte[] value)
        {
            LOG(0, $"Writing {prop}...");

            var cmd = new List<byte>();

            cmd.AddRange(Encoding.ASCII.GetBytes($"getvar:nve:{prop}@"));
            cmd.AddRange(value);

            var res = fb.Command(cmd.ToArray());

            LOG(0, "", res.Payload);

            if (!res.Payload.Contains("set nv ok"))
            {
                throw new Exception($"Failed to set: {res.Payload}");
            }
        }

        public static byte[] GetSHA256(string str)
        {
            return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(str));
        }

        public static void SetHWDogState(byte state)
        {
            if (!IsDeviceConnected()) return; //if timeout and no device
            foreach (var command in new[] { "hwdog certify set", "backdoor set" })
            {
                LOG(0, $"Trying {command}...");
                var res = fb.Command($"oem {command} {state}");
                LOG(0, "", res.Payload);
                if (res.Status == Fastboot.FastbootStatus.Ok || res.Payload.Contains("equal"))
                {
                    LOG(0, $"{command}: success");
                    return;
                }
            }

            LOG(2, "Failed to set FBLOCK state!");
        }
        public static void ReadAllMethods()
        {
            if (!IsDeviceConnected()) return; //if timeout and no device
            if ((BLKEY = ReadFactoryKey()).Length >= 8)
                LOG(0, "HISIOldKey", " Method1-> " + BLKEY);
            if ((BLKEY = ReadFactoryKeyMethod2()).Length >= 8)
                LOG(0, "HISIOldKey", " Method2(SHA256)-> " + BLKEY);
            if ((BLKEY = ReadIndentifier()).Length >= 8)
                LOG(0, "HISIOldKey", " Method3-> " + BLKEY);
        }
        public static string ReadFactoryKey()
        {
            if (!IsDeviceConnected()) return "NaN";
            var res = fb.Command("getvar:nve:WVLOCK");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : "NaN";
        }
        public static string ReadFactoryKeyMethod2()
        {
            if (!IsDeviceConnected()) return "NaN";
            var res = fb.Command("getvar:nve:USRKEY");
            var match = CRC.BytesToHexString(res.RawData);
            return match;
        }
        public static string ReadIndentifier()
        {
            if (!IsDeviceConnected()) return "NaN";
            var res = fb.Command("oem get_identifier_token");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : "NaN";
        }
        public static void UnlockFBLOCK()
        {
            var fblockState = (byte)1;
            try
            {
                SetNVMEProp("FBLOCK", new[] { fblockState });
            }
            catch (Exception ex)
            {
                LOG(2, "Failed to set the FBLOCK, using the alternative method...");
                if (debug) LOG(0, ex.Message);
                SetHWDogState(fblockState);
            }
        }
        public static void LockFBLOCK()
        {
            var fblockState = (byte)0;
            try
            {
                SetNVMEProp("FBLOCK", new[] { fblockState });
            }
            catch (Exception ex)
            {
                LOG(2, "Failed to set the FBLOCK, using the alternative method...");
                if (debug) LOG(0, ex.Message);
                SetHWDogState(fblockState);
            }
        }
        public static string WriteKEY(string key)
        {
            try
            {
                SetNVMEProp("WVLOCK", Encoding.ASCII.GetBytes(key));
                SetNVMEProp("USRKEY", GetSHA256(key));
                return key;
            }
            catch (Exception ex)
            {
                LOG(2, "Failed to set the key.");
                if (debug) LOG(2, ex.Message);
            }
            return "NaN";
        }
        public static bool IsDeviceConnected(int time=10)
        {
            if (fb.device == null) fb.Connect(time);
            return fb.device != null;
        }
        public static string Reboot(string state = "")
        {
            if (!IsDeviceConnected()) return "NaN";
            var res = fb.Command("reboot" +state);
            LOG(0, res.Payload);
            return res.Payload;
        }
        public static void StartUnlockPRCS(bool frp, bool rb, string key, Bootloader d, string port)
        {
            try
            {
                FlashBootloader(d, port);

                LOG(0, "[Fastboot] ", "CheckCon");
                if (fb.Connect())
                {
                    DeviceInfo.loadedhose = true;
                    if (!frp)
                    {
                        UnlockFBLOCK();
                        ReadInfo();
                        LOG(0, "HISINewKey", BLKEY = WriteKEY(key));
                    }
                    else
                    {
                        LOG(1, "Unlocker", "(KIRIN FRP)");
                        UnlockFRP();
                    }
                    if (rb) Reboot();
                    fb.Disconnect();
                }
                else LOG(1, "NoDEVICEAnsw", " [HISI] Maybe hisi Loaders Wont boot");
            }
            catch (Exception ex)
            {
                LOG(2, ex.Message);
                if (debug) LOG(2, ex.StackTrace);
            }
        }
        public static bool TryUnlock(string key)
        {
            if (!IsDeviceConnected()) return false;
            if (fb.Command("oem unlock " + key.Trim()).Status == Fastboot.FastbootStatus.Ok)
                return true;
            else if (fb.Command("oem sec_unlock " + key.Trim()).Status == Fastboot.FastbootStatus.Ok)
                return true;
            else if (fb.Command("oem unlock-go " + key.Trim()).Status == Fastboot.FastbootStatus.Ok)
                return true;
            return false;
        }
        public static string GetPartitionList()
        {
            return fb.Command("getvar:ptable").Payload;
        }
    }
}
