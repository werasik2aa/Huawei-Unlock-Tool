using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
namespace HuaweiUnlocker.DIAGNOS
{
    public class CRC : HashAlgorithm
    {
        private readonly ushort[] _table = new ushort[256];
        private readonly ushort _polynomial;
        private readonly ushort _xorValue;
        private readonly byte[] _initialSum;

        private void InitializeTable()
        {
            for (ushort i = 0; i < _table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ _polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                _table[i] = value;
            }
        }
        public override void Initialize()
        {
            HashValue = _initialSum;
        }
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var sum = BitConverter.ToUInt16(HashValue, 0);
            var i = ibStart;
            var size = (cbSize - ibStart) * 8;

            while (size >= 8)
            {
                var v = array[i++];
                sum = (ushort)((_table[(v ^ sum) & 0xFF] ^ (sum >> 8)) & 0xFFFF);
                size -= 8;
            }

            if (size != 0)
            {
                for (var n = array[i] << 8; ; n >>= 1)
                {
                    if (size == 0) break;
                    size -= 1;
                    var flag = ((sum ^ n) & 1) == 0;
                    sum >>= 1;
                    if (flag) sum ^= _polynomial;
                }
            }

            HashValue = BitConverter.GetBytes(sum);
            HashSizeValue = HashValue.Length;
        }
        protected override byte[] HashFinal()
        {
            var result = BitConverter.GetBytes((ushort)((BitConverter.ToUInt16(HashValue, 0) ^ _xorValue) & 0xFFFF));
            HashValue = _initialSum;
            return result;
        }
        public UInt16 ComputeSum(byte[] buffer)
        {
            return ComputeSum(buffer, 0, buffer.Length);
        }
        public UInt16 ComputeSum(byte[] buffer, int offset, int count)
        {
            return BitConverter.ToUInt16(ComputeHash(buffer, offset, count), 0);
        }
        public override bool CanReuseTransform
        {
            get { return true; }
        }
        public override bool CanTransformMultipleBlocks
        {
            get { return true; }
        }
        public CRC(ushort initialSum = 0xFFFF, ushort polynomial = 0x8408, ushort xorValue = 0xFFFF)
        {
            _initialSum = BitConverter.GetBytes(initialSum);
            _polynomial = polynomial;
            _xorValue = xorValue;

            // Init table
            InitializeTable();

            // Initialize sum
            HashValue = _initialSum;
        }
        public const ulong CRC_SEED = 0xffffUL;
        private static readonly ulong[] CRCqualcomm = new ulong[] {
        0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
        0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
        0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
        0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
        0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
        0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
        0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
        0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
        0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
        0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
        0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
        0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
        0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
        0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
        0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
        0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
        0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
        0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
        0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
        0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
        0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
        0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
        0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
        0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
        0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
        0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
        0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
        0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
        0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
        0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
        0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
        0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78,
        0x62A7, 0x5E28, 0x1479, 0xDE98, 0x1027, 0x62A7, 0x0C0B, 0x11B3,
        0xD1A7
    };
        private static readonly ushort[] CRChisi = new ushort[] {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
            0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
            0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
            0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
            0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
            0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
            0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
            0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
            0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
            0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
            0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
            0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
            0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
            0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
            0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
            0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
            0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
            0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
            0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0,
        };
        private static readonly ushort[] crcHDLC =
        {
            0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF, 0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C,
            0xDBE5, 0xE97E, 0xF8F7, 0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E, 0x9CC9, 0x8D40,
            0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876, 0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434,
            0x55BD, 0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5, 0x3183, 0x200A, 0x1291, 0x0318,
            0x77A7, 0x662E, 0x54B5, 0x453C, 0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974, 0x4204,
            0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB, 0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1,
            0xAB7A, 0xBAF3, 0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A, 0xDECD, 0xCF44, 0xFDDF,
            0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72, 0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
            0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1, 0x7387, 0x620E, 0x5095, 0x411C, 0x35A3,
            0x242A, 0x16B1, 0x0738, 0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70, 0x8408, 0x9581,
            0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7, 0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76,
            0x7CFF, 0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036, 0x18C1, 0x0948, 0x3BD3, 0x2A5A,
            0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E, 0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5, 0x2942,
            0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD, 0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226,
            0xD0BD, 0xC134, 0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C, 0xC60C, 0xD785, 0xE51E,
            0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3, 0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
            0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232, 0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1,
            0x0D68, 0x3FF3, 0x2E7A, 0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1, 0x6B46, 0x7ACF,
            0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9, 0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9,
            0x8330, 0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
        };
        public static ushort ComputeChecksum(byte[] bytes)
        {
            var crc = 0xFFFF;
            for (var i = 0; i < bytes.Length; ++i)
                crc = ((crc >> 8) ^ crcHDLC[(crc ^ bytes[i]) & 0xFF]) & 0xFFFF;

            crc = ~crc;
            crc = ((crc >> 8) & 0xFF) | (crc << 8);
            return (ushort)crc;
        }

        public static byte[] ComputeChecksumBytes(byte[] bytes)
        {
            var crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }
        public static ushort GetChecksum(byte[] data)
        {
            int crc = 0;
            foreach (var octet in data) crc = ((crc << 8) | octet) ^ CRChisi[(crc >> 8) & 0xFF];
            for (int i = 0; i < 2; i++) crc = (crc << 8) ^ CRChisi[(crc >> 8) & 0xFF];
            return (ushort)(crc & 0xFFFF);
        }
        private static bool CheckByte(ref byte result, byte chkByte)
        {
            switch (chkByte)
            {
                case 0x7d:
                case 0x7e:
                    result = (byte)(chkByte ^ 0x20);
                    return true;
            }
            result = chkByte;
            return false;
        }

        private static ushort CRCIT(ushort crc, byte data)
        {
            return Convert.ToUInt16((ushort)(((ushort)(crc >> 8)) ^ CRCqualcomm[(crc ^ data) & 0xff]));
        }

        public static byte[] GetBufferWithCRC(string s, bool hex)
        {
            if (!hex)
                return GetBufferWithCRC(Encoding.ASCII.GetBytes(s), (int)Math.Round(s.Length / 2.0));
            else
                return GetBufferWithCRC(HexStringToBytes(s), (int)Math.Round(s.Length / 2.0));
        }
        public static byte[] GetBufferWithCRC(byte[] s)
        {
            return GetBufferWithCRC(s, (int)Math.Round((double)(((double)s.Length) / 2.0)));
        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        private static byte[] GetBufferWithCRC(byte[] data, int count)
        {
            ushort crc = 0;
            List<byte> list = new List<byte>();
            byte result = 0;
            try
            {
                int num5 = count - 1;
                for (int i = 0; i <= num5; i++)
                {
                    if (CheckByte(ref result, data[i]))
                        list.Add(0x7d);
                    list.Add(result);
                }
                crc = 0xffff;
                int num6 = count - 1;
                for (int j = 0; j <= num6; j++)
                    crc = CRCIT(crc, data[j]);

                crc = (ushort)(crc ^ 0xffff);

                if (CheckByte(ref result, Convert.ToByte((int)(crc & 0xff))))
                    list.Add(0x7d);

                list.Add(result);

                if (CheckByte(ref result, Convert.ToByte((int)(((ushort)(crc >> 8)) & 0xff))))
                    list.Add(0x7d);
                list.Add(result);
                list.Add(0x7e);
            }
            catch (Exception)
            {
                byte[] buffer = list.ToArray();
                return buffer;
            }
            return list.ToArray();
        }

        public static byte[] InvertBytes(byte[] inByte)
        {
            long num = (long)Math.Round(Conversion.Val("&H" + BytesToHexString(inByte)));
            byte[] bytes = BitConverter.GetBytes((long)(-1L ^ num));
            return new byte[] { bytes[1], bytes[0] };
        }
        public static byte[] InverseBytes(byte[] bytes)
        {
            Array.Reverse(bytes, 0, bytes.Length);
            return bytes;
        }
        public static string BytesToHexString(byte[] byteInput)
        {
            try
            {
                string str3 = "";
                foreach (byte num in byteInput) str3 = str3 + Conversion.Hex(num).PadLeft(2, '0');
                return str3;
            }
            catch (Exception e)
            { LangProc.LOG(2, e.Message); }
            return "";
        }
        public static string HexStringFormatter(string hexStr)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                for (int i = 0; i <= hexStr.Length - 1; i += 2)
                    builder.Append(hexStr.Substring(i, 2) + " ");
            }
            catch (Exception e)
            { LangProc.LOG(2, e.Message); }
            return builder.ToString().TrimEnd(new char[0]);
        }
        public static byte[] HexStringToBytes(string strInput)
        {
            try
            {
                int startIndex = 0;
                byte[] buffer = new byte[strInput.Length];
                for (int i = 0; strInput.Length > (startIndex + 1); i++)
                {
                    buffer[i] = Convert.ToByte(Convert.ToInt64(strInput.Substring(startIndex, 2), 0x10));
                    startIndex += 2;
                }
                return buffer;
            }
            catch (Exception)
            {
                LangProc.LOG(2, "Hex String To Byte Array Conversion Error!");
            }
            return null;
        }
        public static string HexToIMEI(string hexInput)
        {
            string str = hexInput;
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            str = hexInput.Remove(0, 1);
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= str.Length - 2; i += 2)
                builder.Append(new string(str.Substring(i, 2).Reverse<char>().ToArray<char>()));
            return builder.ToString().TrimStart(new char[] { 'A' });
        }
        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }
        public static string GetASCIIString(byte[] bytes)
        {
            string text2 = "", text = "", maintxt = BytesToHexString(bytes);
            try
            {
                for (int i = 0; i <= maintxt.Length - 1; i++)
                {
                    if (maintxt.Substring(i, 2) != "00")
                        text2 += maintxt.Substring(i, 2);
                    i++;
                }
                while (text2.Length > 0)
                {
                    text += Convert.ToChar(Convert.ToUInt64(text2.Substring(0, 2), 16));
                    text2 = text2.Substring(2, text2.Length - 2);
                }
            }
            catch (Exception e)
            {
                LangProc.LOG(2, e.Message);
                text = "-1";
            }
            return text;
        }
        public static byte[] Encryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    encryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static byte[] Decryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    decryptedData = RSA.Decrypt(Data, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public static byte[] HexFileRead(string path, UInt32 Offset, int count)
        {
            BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));
            reader.BaseStream.Position = Offset;
            byte[] a = reader.ReadBytes(count);
            reader.Close();
            reader.Dispose();
            return a;
        }
        public static byte[] EncryptData(string pathcrt, byte[] data)
        {
            try
            {
                X509Certificate2 collection = new X509Certificate2();
                collection.Import(pathcrt);
                RSA csp = (RSA)collection.PublicKey.Key;
                return csp.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
            catch (Exception ex)
            {
                LangProc.LOG(2, ex.Message);
                return null;
            }
        }
        public static string DecryptData(string pathcrt, byte[] data)
        {
            LangProc.LOG(-1, "=============DECRypter OEM=============");
            try
            {
                var Password = "123"; //Note This Password is That Password That We Have Put On Generate Keys  
                var collection = new X509Certificate2();
                collection.Import(File.ReadAllBytes(pathcrt), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                if (collection.HasPrivateKey)
                {
                    LangProc.LOG(0, "Decrypting");
                    RSA csp = (RSA)collection.PrivateKey;
                    var privateKey = collection.PrivateKey as RSACryptoServiceProvider;
                    var keys = Encoding.ASCII.GetString(csp.Decrypt(data, RSAEncryptionPadding.Pkcs1));
                    return keys;
                }
                LangProc.LOG(2, "No private");
            }
            catch (Exception ex) { LangProc.LOG(2, ex.Message);}
            LangProc.LOG(2, "WRONG CER: ");
            return null;
        }
    }
}