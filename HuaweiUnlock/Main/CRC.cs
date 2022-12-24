using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace HuaweiUnlocker
{
    public class CRC
    {
        public const ulong CRC_SEED = 0xffffUL;
        public static readonly ulong[] CRCTable = new ulong[] {
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
    0x62A7, 0x5E28, 0x1479, 0xDE98, 0x1027, 0x62A7, 0x0C0B, 0x11B3
    };
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
            return Convert.ToUInt16((ushort)(((ushort)(crc >> 8)) ^ CRCTable[(crc ^ data) & 0xff]));
        }

        public static byte[] GetBufferWithCRC(string s, bool hex)
        {
            if (!hex)
                return GetBufferWithCRC(Encoding.ASCII.GetBytes(s), (int)Math.Round(s.Length/ 2.0));
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
        public static string BytesToHexString(byte[] byteInput)
        {
            try
            {
                string str3 = "";
                foreach (byte num in byteInput) str3 = str3 + Conversion.Hex(num).PadLeft(2, '0');
                return str3;
            }
            catch (Exception e)
            { MISC.LOG("ERROR: " + e); }
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
            { MISC.LOG("ERROR: " + e); }
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
                MISC.LOG("ERROR: Hex String To Byte Array Conversion Error!");
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
            string text2 = "", text= "", maintxt = BytesToHexString(bytes);
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
            catch (Exception ex)
            {
                MISC.LOG("ERROR: " + ex);
                text = "-1";
            }
            return text;
        }
    }
}