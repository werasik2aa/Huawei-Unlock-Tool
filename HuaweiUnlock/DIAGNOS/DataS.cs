using System;

namespace HuaweiUnlocker.DIAGNOS
{
    public class DataS
    {
        public const string MTK_dloadonly = "dl_only";
        public const string MTK_firmwareupgrade = "firmware_upgrade";
        public const string MTK_formatallanddload = "fm_and_dl";
        public const string MTK_formatall = "fm";
        public struct GPT_Struct
        {
            public int StartAdress;
            public int Length;
            public string ValueString;
            public GPT_Struct(int startAdress = -1, int length = -1, string value = "")
            {
                StartAdress = startAdress;
                Length = length;
                ValueString = value;
            }
        }
        public struct OemInfoHdr
        {
            public byte[] Magic;
            public int Offset;
            public long DataLenPage;
            public UInt32 Version;
            public UInt32 ID;
            public UInt32 Type;
            public UInt32 DataLenOem;
            public UInt32 Age;
        }
        public static string IdentifyCPUbyID(string id)
        {
            switch (id)
            {
                case "009F00E1": return "APQ8056";
                case "009710E1": return "APQ8056";
                case "007190E1": return "APQ8056";
                case "009830E1": return "APQ8076";
                case "009D00E1": return "APQ8076";
                case "009000E1": return "APQ8084";
                case "009010E1": return "APQ8084";
                case "009300E1": return "APQ8092";
                case "009630E1": return "APQ8092";
                case "008000E1": return "MSM8226";
                case "009150E1": return "MSM8226";
                case "007210E1": return "MSM8930";
                case "0072C0E1": return "MSM8930";
                case "0090B0E1": return "MSM8939";
                case "050E10E1": return "MSM8939";
                case "000460E1": return "MSM8953";
                case "F00460E1": return "MSM8953";
                case "009700E1": return "MSM8956";
                case "009B00E1": return "MSM8956";
                case "007B00E1": return "MSM8974";
                case "007B20E1": return "MSM8974";
                case "007B40E1": return "MSM8974AB";
                case "007BA0E1": return "MSM8974AB";
                case "006B10E1": return "MSM8974AC";
                case "007B60E1": return "MSM8974AC";
                case "009640E1": return "MSM8992";
                case "009690E1": return "MSM8992";
                case "000630E1": return "MSM8996AU";
                case "0006F0E1": return "MSM8996AU";
                case "1006F0E1": return "MSM8996AU";
                case "4006F0E1": return "MSM8996AU";
                case "30020000": return "MSM8998";
                case "0005E0E1": return "MSM8998";
                case "00FFF0E1": return "MSM8998";
                case "000020E1": return "MSM8998";
                case "30070000": return "SDM630";
                case "0007E0E1": return "SDM630";
                case "000AC0E1": return "SDM636";
                case "000CC0E1": return "SDM636";
                case "F00CC0E1": return "SDM636";
                case "30060000": return "SDM660";
                case "0008C0E1": return "SDM660";
                case "0009C0E1": return "SDM660";
                case "0007D0E1": return "SDM660";
                case "001080E1": return "SDM712";
                case "60040000": return "SDM712";
                case "60000000": return "SDM845";
                case "0008B0E1": return "SDM845";
                //MAIN HUAWEI
                case "0x009600e1": return "MSM8909";
                case "0x000460e1": return "MSM8953";
                case "0x0091b0e1": return "MSM8929";
                case "0x006220e1": return "MSM7227A";
                case "0x009470e1": return "MSM8996";
                case "0x009900e1": return "MSM8976";
                case "0x009b00e1": return "MSM8976";
                case "0x008A30E1": return "MSM8930";
                case "0x0004f0e1": return "MSM8937";
                case "0x0090b0e1": return "MSM8936";
                case "0x009180e1": return "MSM8928";
                case "0x008140e1": return "MSM8x10";
                case "0x008050e2": return "MSM8926";
                case "0x0005f0e1": return "MSM8996";
                case "0x007B80E1": return "MSM8974";
                case "0x009400e1": return "MSM8994";
                case "0x008150e1": return "MSM8x10";
                case "0x008050e1": return "MSM8926";
                case "0x000560e1": return "MSM8917";
                case "0x007050e1": return "MSM8916";
                case "0x008110e1": return "MSM8210";
                default: return "Unknown";
            }
        }
    }
}
