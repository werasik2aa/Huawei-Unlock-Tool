using HuaweiUnlocker.DIAGNOS;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.TOOLS
{
    public class HISI
    {
        public delegate void RunWorkerCompletedHandler();
        public event RunWorkerCompletedHandler RunWorkerCompleted;

        private Fastboot fb = new Fastboot();
        public string BSN = "NaN";
        public string BNUM = "NaN";
        public string AVER = "NaN";
        public string MODEL = "NaN";
        public string BLKEY = "NaN";
        public string FBLOCKSTATE = "NaN";
        public bool FBLOCK = true;
        public void FlashBootloader(Bootloader bootloader, string port)
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

                flasher.Write(image.Path, (int)image.Address, x => {
                    Progress(dsize + (int)(size / 100f * x), asize);
                });

                dsize += size;
            }

            flasher.Close();
        }
        public bool ReadInfo(int waittime = 100)
        {
            if (fb.Connect(waittime))
            {
                GetASerial();
                GetModelProduct();
                GetModelBSN();
                GetBuildID();
                GetFBLockState();
                ReadAllMethods();
                return true;
            }
            return false;
        }
        public string GetASerial()
        {
            string serial = fb.GetSerialNumber();
            LOG(0, "SerialnTag", serial);
            return AVER = serial;
        }
        public string GetModelBSN()
        {
            Fastboot.Response bsn = fb.Command("oem read_bsn");
            if (bsn.Status == Fastboot.FastbootStatus.Ok)
            {
                LOG(0, "BSNTag", bsn.Payload);
                BSN = bsn.Payload;
            }
            return BSN;
        }
        public string GetModelProduct()
        {
            Fastboot.Response model = fb.Command("oem get-product-model");
            LOG(0, "ModelTag", model.Payload);
            return MODEL = model.Payload;
        }
        public string GetBuildID()
        {
            Fastboot.Response build = fb.Command("oem get-build-number");
            LOG(0, "BuildIdTag", build.Payload.Replace(":", ""));
            return BNUM = build.Payload.Replace(":", "");
        }
        public bool GetFBLockState()
        {
            Fastboot.Response fblock = fb.Command("oem lock-state info");
            bool state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
            if (!state)
            {
                fblock = fb.Command("oem backdoor info");
                state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
            }
            LOG(0, "FBLOCKTag", FBLOCKSTATE = (FBLOCK = state) ? "UNLOCKED" : "LOCKED");
            if(!state)
                LOG(2, "HISIInfoS");
            return FBLOCK;
        }
        public void UnlockFRP()
        {
            LOG(0, "Unlocker", "FRP (BETA)");
            string command = "Tools\\fastboot.exe";
            string subcommand = "flash devinfo Tools\\frpUnlocked.img";
            string subcommand2 = "flash frp Tools\\frpPartition.img";
            fb.Command("oem erase frp");
            fb.Command("oem unlock-frp");
            fb.Command("oem frp-erase");
            fb.Command("oem frp-unlock");
            fb.Command("oem format cache");
            LOG(1, "THIS IS BETA!");
            SyncRUN(command, subcommand);
            SyncRUN(command, subcommand2);
            LOG(1, "THIS IS BETA AND MAY NOT WORK!");
            LOG(1, "Recomended to open the fastboot and flash devinfo(frpunlocked.img) or frp(frpPartition.img) (frp from program Tools folder!");
        }
        public void SetNVMEProp(string prop, byte[] value)
        {
            LOG(0, $"Writing {prop}...");

            var cmd = new List<byte>();

            cmd.AddRange(Encoding.ASCII.GetBytes($"getvar:nve:{prop}@"));
            cmd.AddRange(value);

            var res = fb.Command(cmd.ToArray());

            LOG(0, "", res.ToString());

            if (!res.Payload.Contains("set nv ok"))
            {
                throw new Exception($"Failed to set: {res.Payload}");
            }
        }

        public static byte[] GetSHA256(string str)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.ASCII.GetBytes(str));
            }
        }

        public void SetHWDogState(byte state)
        {
            foreach (var command in new[] { "hwdog certify set", "backdoor set" })
            {
                LOG(0, $"Trying {command}...");
                var res = fb.Command($"oem {command} {state}");
                LOG(0, "", res.ToString());
                if (res.Status == Fastboot.FastbootStatus.Ok || res.Payload.Contains("equal"))
                {
                    LOG(0, $"{command}: success");
                    return;
                }
            }

            LOG(2, "Failed to set FBLOCK state!");
        }
        public void ReadAllMethods()
        {
            if ((BLKEY = ReadFactoryKeyMethod2()).Length >= 8)
                LOG(0, "HISIOldKey", " POSSIBLE-> " + BLKEY);
            if ((BLKEY = ReadFactoryKey()).Length >= 8)
                LOG(0, "HISIOldKey", " POSSIBLE-> " + BLKEY);
            if ((BLKEY = ReadIndentifier()).Length >= 8)
                LOG(0, "HISIOldKey", " POSSIBLE-> " + BLKEY);
        }
        public string ReadFactoryKey()
        {
            var res = fb.Command("getvar:nve:WVLOCK");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : "NaN";
        }
        public string ReadFactoryKeyMethod2()
        {
            var res = fb.Command("getvar:nve:USRKEY");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : "NaN";
        }
        public string ReadIndentifier()
        {
            var res = fb.Command("oem get_identifier_token");
            return res.ToString();
        }
        public void UnlockFBLOCK()
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
        public void LockFBLOCK()
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
        public string WriteBOOTLOADERKEY(string key)
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
                if(debug) LOG(2, ex.Message);
            }
            return "NaN";
        }
        public string UnlockSec_Method2()
        {
            if(IsDeviceConnected())
            {
                var res = fb.Command("oem sec_unlock");
                LOG(0, res.Payload);
                return res.Payload;
            }
            return "NaN";
        }
        public bool IsDeviceConnected()
        {
            return fb.device != null;
        }
        public string Reboot()
        {
            var res = fb.Command("reboot");
            LOG(0, res.Payload);
            return res.Payload;
        }
        public void WriteKirinBootloader(Bootloader d, string port)
        {
            FlashBootloader(d, port);
            LOG(0, "Unlocker", "[KIRIN FBLOCK]");
            if (ReadInfo())
            {
                DeviceInfo.loadedhose = true;
                UnlockFBLOCK();
                if (!GetFBLockState())
                {
                    LOG(1, "HISINewKeyErr");
                    LOG(0, "HISINewKeyErr2");
                    UnlockSec_Method2();
                }
            }
        }
        public void StartUnlockPRCS(bool frp, string key, Bootloader d, string port)
        {
            fb = new Fastboot();

            try
            {
                FlashBootloader(d, port);

                LOG(0, "[Fastboot] ", "CheckCon");
                if (fb.Connect())
                {
                    if (!frp)
                    {
                        UnlockFBLOCK();
                        if (!FBLOCK)
                        {
                            LOG(1, "HISINewKeyErr");
                            LOG(0, "HISINewKeyErr2");
                            UnlockSec_Method2();
                        }
                        ReadAllMethods();
                        LOG(0, "HISINewKey", BLKEY = WriteBOOTLOADERKEY(key));
                    }
                    else
                    {
                        LOG(1, "Unlocker", "(KIRIN FRP)");
                        UnlockFRP();
                    }
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
    }
}
