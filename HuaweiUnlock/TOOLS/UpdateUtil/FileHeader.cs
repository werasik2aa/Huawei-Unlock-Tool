using System;
using System.Runtime.InteropServices;

namespace HuaweiUnlocker.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct FileHeader
    {
        public UInt32 HeaderId;
        public UInt32 HeaderSize;
        public UInt32 Unknown1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] HardwareId;
        public UInt32 FileSequence;
        public UInt32 FileSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileDate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] FileType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Blank1;
        public UInt16 HeaderChecksum;
        public UInt16 BlockSize;
        public UInt16 Blank2;
        public static readonly int Size = Marshal.SizeOf(typeof(FileHeader));
        public static FileHeader Create()
        {
            var result = new FileHeader
            {
                HardwareId = new byte[8],
                FileDate = new byte[16],
                FileTime = new byte[16],
                FileType = new byte[16],
                Blank1 = new byte[16]
            };

            return result;
        }
    }
}
