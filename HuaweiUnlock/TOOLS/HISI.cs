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
        public string BNUM = "NaN";
        public string AVER = "NaN";
        public string MODEL = "NaN";
        public string BLKEY = "NaN";
        public string FBLOCKSTATE = "NaN";
        public bool SetHWDogState(byte state)
        {
            foreach (var command in new[] { "hwdog certify set", "backdoor set" })
            {
                LOG(0, "Trying: " + command);
                Fastboot.Response res = fb.Command("oem " + command + " " + state);
                if (debug) LOG(0, Encoding.UTF8.GetString(res.RawData));
                if (res.Status == Fastboot.FastbootStatus.Ok || res.Payload.Contains("equal"))
                {
                    LOG(0, command + " success");
                    return true;
                }
            }

           LOG(2, "Failed to set FBLOCK state!");
           return false;
        }
        public bool ReadInfo(bool wait)
        {
            if (fb.Connect()) {
                string serial = fb.GetSerialNumber();
                LOG(0, "SerialnTag", serial);
                AVER = serial;

                Fastboot.Response bsn = fb.Command("oem read_bsn");
                if (bsn.Status == Fastboot.FastbootStatus.Ok) {
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
                    LOG(2, "HISIInfoS");
                else
                {
                    string factoryKey = ReadFactoryKey();
                    if (factoryKey != null)
                    {
                        LOG(0, "KEYTag", factoryKey);
                        BLKEY = factoryKey;
                    }
                }
                return true;
            }
            return false;
        }
        public void SetProp(string prop, byte[] value)
        {
            LOG(0, "WritingPropTAG", prop);
            var cmd = new List<byte>();

            cmd.AddRange(Encoding.ASCII.GetBytes($"getvar:nve:{prop}@"));
            cmd.AddRange(value);

            var res = fb.Command(cmd.ToArray());

            if(debug) LOG(1, Encoding.UTF8.GetString(res.RawData));

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
                LOG(1, "FBLOCKSetTag");
                LOG(2, ex.Message);
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
                LOG(2, "Failed to set the key.", ex.Message);
            }
        }
    }
}
