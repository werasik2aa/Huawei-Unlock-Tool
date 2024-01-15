using HuaweiUnlocker.DIAGNOS;
using System;
using System.Collections.Generic;

namespace HuaweiUnlocker.TOOLS
{
    public class OemInfo
    {
        public static Dictionary<string, byte[]> Data = new Dictionary<string, byte[]>();
        public static UInt32 OffsetHMAC, OffsetUsrLock = 0x0001B000, OffsetBSN = 0x00034000, OffsetBUILDID = 0x0000E000, OffsetVendor = 0x00011000, OffsetProp = 0x00042000, OffsetModel = 0x0004C000, OffsetPVers = 0x0004D000;
        public static void ReadInfo(string path)
        {
            Data.Add("BUILDID", CRC.HexFileRead(path, OffsetBUILDID, 2048));
            Data.Add("USR-LOCK", CRC.HexFileRead(path, OffsetUsrLock, 2048));
            Data.Add("BSN_BID", CRC.HexFileRead(path, OffsetBSN, 2048));
            Data.Add("Vendor", CRC.HexFileRead(path, OffsetVendor, 2048));
            Data.Add("Properties", CRC.HexFileRead(path, OffsetProp, 2048));
            Data.Add("Model", CRC.HexFileRead(path, OffsetModel, 2048));
            Data.Add("ModelVersion", CRC.HexFileRead(path, OffsetPVers, 2048));
        }
    }
}
