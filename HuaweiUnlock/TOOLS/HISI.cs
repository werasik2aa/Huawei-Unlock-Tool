using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using static HuaweiUnlocker.LangProc;
using System.Text.RegularExpressions;
using System.Text;

namespace HuaweiUnlocker.TOOLS
{
    public class HISI
    {
        private Fastboot fb = new Fastboot();
        public bool DisableFBLOCK;
        public string BSN = "NaN";
        public string BVER = "NaN";
        public string AVER = "NaN";
        public string ASerial = "NaN";
        public string BLKEY = "NaN";
        public bool SetHWDogState(byte state)
        {
            foreach (var command in new[] { "hwdog certify set", "backdoor set" })
            {
                LOG("Trying: " + command);
                Fastboot.Response res = fb.Command("oem " + command + " " + state);
                if (debug) LOG(Encoding.UTF8.GetString(res.RawData));
                if (res.Status == Fastboot.FastbootStatus.Ok || res.Payload.Contains("equal"))
                {
                    LOG(command + " success");
                    return true;
                }
            }

           LOG("Failed to set FBLOCK state!");
           return false;
        }
        public bool ReadInfo(bool wait)
        {
            if (fb.Connect(wait)) {
                string serial = fb.GetSerialNumber();
                LOG(I("SerialnTag") + serial);
                ASerial = serial;

                Fastboot.Response bsn = fb.Command("oem read_bsn");
                if (bsn.Status == Fastboot.FastbootStatus.Ok) {
                    LOG(I("BSNTag") + bsn.Payload);
                    BSN = bsn.Payload;
                }

                Fastboot.Response model = fb.Command("oem get-product-model");
                LOG(I("ModelTag") + model.Payload);
                AVER = model.Payload;

                Fastboot.Response build = fb.Command("oem get-build-number");
                LOG(I("BuildIdTag") + build.Payload.Replace(":", ""));
                BVER = build.Payload.Replace(":", "");

                Fastboot.Response fblock = fb.Command("oem lock-state info");
                bool state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");

                if (!state)
                {
                    fblock = fb.Command("oem backdoor info");
                    state = Regex.IsMatch(fblock.Payload, @"FB[\w: ]{1,}UNLOCKED");
                }

                LOG(I("FBLOCK-Tag") + (state ? "unlocked" : "locked"));
                if (debug) LOG(Encoding.UTF8.GetString(fblock.RawData));

                if (!state)
                    LOG(E("HISIInfoS"));
                else
                {
                    string factoryKey = ReadFactoryKey();
                    if (factoryKey != null)
                    {
                        LOG(I("KEYTag") + factoryKey);
                        BLKEY = factoryKey;
                    }
                }
                return true;
            }
            return false;
        }
        public void SetProp(string prop, byte[] value)
        {
            LOG(I("WritingPropTAG") + prop);
            var cmd = new List<byte>();

            cmd.AddRange(Encoding.ASCII.GetBytes($"getvar:nve:{prop}@"));
            cmd.AddRange(value);

            var res = fb.Command(cmd.ToArray());

            if(debug) LOG(Encoding.UTF8.GetString(res.RawData));

            if (!res.Payload.Contains("set nv ok"))
                throw new Exception("Failed to set: "+ res.Payload);
        }
        public string ReadFactoryKey()
        {
            var res = fb.Command("getvar:nve:WVLOCK");
            var match = Regex.Match(res.Payload, @"[\w\d]{16}");

            return match.Success ? match.Value : null;
        }
        public void SetFBLOCK(int state)
        {
            try
            {
                SetProp("FBLOCK", new[] { (byte) state });
            }
            catch (Exception ex)
            {
                LOG(I("FBLOCKSetTag"));
                LOG(ex.Message);
                SetHWDogState((byte)state);
            }
        }
        public void WriteBOOTLOADERKEY(string KEY)
        {
            SetFBLOCK(1);

            try
            {
                SetProp("WVLOCK", Encoding.ASCII.GetBytes(KEY));
                SetProp("USRKEY", SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(KEY)));
            }
            catch (Exception ex)
            {
                LOG("Failed to set the key.");
                LOG(ex.Message);
            }
        }
    }
}
