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

        private Fastboot fb;
        public string BSN = "NaN";
        public string BNUM = "NaN";
        public string AVER = "NaN";
        public string MODEL = "NaN";
        public string BLKEY = "NaN";
        public string FBLOCKSTATE = "NaN";

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

        public bool ReadInfo()
        {
            if (IsConnected())
            {
                string serial = fb.GetSerialNumber();
                LOG(0, "SerialnTag", serial);
                AVER = serial;

                Fastboot.Response bsn = fb.Command("oem read_bsn");
                if (bsn.Status == Fastboot.FastbootStatus.Ok)
                {
                    LOG(0, "BSNTag", bsn.Payload);
                    BSN = bsn.Payload;
                }

                Fastboot.Response model = fb.Command("oem get-product-model");
                LOG(0, "ModelTag", model.Payload);
                MODEL = model.Payload;

                Fastboot.Response build = fb.Command("oem get-build-number");
                LOG(0, "BuildIdTag", build.Payload.Replace(":", ""));
                BNUM = build.Payload.Replace(":", "");

                Fastboot.Response fblock = fb.Command("oem lock-state info");
                bool state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
                if (!state)
                {
                    fblock = fb.Command("oem backdoor info");
                    state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
                }
                FBLOCKSTATE = state ? "UNLOCKED" : "LOCKED";
                LOG(0, "FBLOCK-Tag", FBLOCKSTATE);
                if (debug) LOG(1, Encoding.UTF8.GetString(fblock.RawData));
                if (!state)
                {
                    LOG(2, "HISIInfoS");
                    return false;
                }
                else
                {
                    string factoryKey = ReadFactoryKey();
                    if (factoryKey != null)
                    {
                        LOG(0, "HISIOldKey", factoryKey);
                        BLKEY = factoryKey;
                    }
                }
                return true;
            }
            return false;
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

        public string ReadFactoryKey()
        {
            var res = fb.Command("getvar:nve:WVLOCK");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : null;
        }
        public string ReadUnlockCodeMethod2()
        {
            var res = fb.Command("oem get_identifier_token");
            return res.ToString();
        }

        public void WriteBOOTLOADERKEY(string key)
        {
            var fblockState = (byte)1;

            try
            {
                SetNVMEProp("FBLOCK", new[] { fblockState });
            }
            catch (Exception ex)
            {
                LOG(2, "Failed to set the FBLOCK, using the alternative method...");
                if(debug) LOG(0, ex.Message);
                SetHWDogState(fblockState);
            }

            try
            {
                SetNVMEProp("WVLOCK", Encoding.ASCII.GetBytes(key));
                SetNVMEProp("USRKEY", GetSHA256(key));
            }
            catch (Exception ex)
            {
                LOG(2, "Failed to set the key.");
                if(debug) LOG(2, ex.Message);
            }
        }
        public string UnlockSec_Method2()
        {
            if(fb.Connect())
            {
                var res = fb.Command("oem sec_unlock");
                LOG(0, res.ToString());
                return res.ToString();
            }
            return "NaN";
        }
        public bool IsConnected()
        {
            return fb.Connect();
        }
        public string Reboot()
        {
            if (IsConnected())
            {
                var res = fb.Command("reboot");
                LOG(0, res.ToString());
                return res.ToString();
            }
            return "NaN";
        }
        public void StartUnlockPRCS(bool frp, string key, Bootloader d, string port)
        {
            fb = new Fastboot();

            try
            {
                FlashBootloader(d, port);

                if (frp)
                {
                    LOG(1, "Unlocker", "(KIRIN FRP)");
                    if (ReadInfo()) UnlockFRP();
                }
                LOG(0, "[Fastboot] ", "CheckCon");
                if (IsConnected())
                {
                    if (ReadInfo())
                    {
                        ReadFactoryKey();
                        WriteBOOTLOADERKEY(key);
                        LOG(0, "HISINewKey", key);
                    }
                    else
                    {
                        LOG(1, "HISINewKeyErr");
                        LOG(0, "HISINewKeyErr2");
                        UnlockSec_Method2();
                        WriteBOOTLOADERKEY(key);
                        BLKEY = ReadUnlockCodeMethod2();
                        LOG(0, "[Fastboot] BL-CODE: " + key + " OR " + BLKEY.ToString());
                    }
                    fb.Disconnect();
                }
            }
            catch (Exception ex)
            {
                LOG(2, ex.Message);
                if (debug) LOG(2, ex.StackTrace);
            }
        }
    }
}
