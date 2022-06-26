using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace HuaweiUnlocker
{

    public class CRC
    {
        public const ushort CRC_SEED = 0xffff;
        public static readonly ushort[] CRCTable = new ushort[] { 
            0, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf, 0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
            0x1081, 0x108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e, 0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
            0x2102, 0x308b, 0x210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd, 0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
            0x3183, 0x200a, 0x1291, 0x318, 0x77a7, 0x662e, 0x54b5, 0x453c, 0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
            0x4204, 0x538d, 0x6116, 0x709f, 0x420, 0x15a9, 0x2732, 0x36bb, 0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
            0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x528, 0x37b3, 0x263a, 0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
            0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x630, 0x17b9, 0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
            0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x738, 0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
            0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7, 0x840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
            0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036, 0x18c1, 0x948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
            0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5, 0x2942, 0x38cb, 0xa50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
            0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134, 0x39c3, 0x284a, 0x1ad1, 0xb58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
            0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3, 0x4a44, 0x5bcd, 0x6956, 0x78df, 0xc60, 0x1de9, 0x2f72, 0x3efb,
            0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232, 0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0xd68, 0x3ff3, 0x2e7a,
            0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1, 0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0xe70, 0x1ff9,
            0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330, 0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0xf78
        };
        public const byte ESC_ASYNC = 0x7d;
        public const byte ESC_COMPL = 0x20;
        public const byte FLAG_ASYNC = 0x7e;
        public const int PRLPacketSize = 40;
        private static byte[] testBytes = new byte[] { 0x75, 110 };

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

        private static ushort ComputeCRC(ushort crc, byte data)
        {
            return Convert.ToUInt16((ushort)(((ushort)(crc >> 8)) ^ CRCTable[(crc ^ data) & 0xff]));
        }

        private static string FlipByteBitsToString(string singleByte)
        {
            if (singleByte == "0")
                return "0";
            if (singleByte == "1")
                return "8";
            if (singleByte == "2")
                return "4";
            if (singleByte == "3")
                return "C";
            if (singleByte == "4")
                return "2";
            if (singleByte == "5")
                return "A";
            if (singleByte == "6")
                return "6";
            if (singleByte == "7")
                return "E";
            if (singleByte == "8")
                return "1";
            if (singleByte == "9")
                return "9";
            if (singleByte == "A")
                return "5";
            if (singleByte == "B")
                return "D";
            if (singleByte == "C")
                return "3";
            if (singleByte == "D")
                return "B";
            if (singleByte == "E")
                return "7";
            return "F";
        }

        public static byte[] GetBufferWithCRC(string s, bool asci)
        {
            if(!asci) 
                return GetBufferWithCRC(HexStringToBytes(s), (int)Math.Round((double)(((double)s.Length) / 2.0)));
            else
                return GetBufferWithCRC(Encoding.ASCII.GetBytes(s), (int)Math.Round((double)(((double)s.Length) / 2.0)));
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
                    crc = ComputeCRC(crc, data[j]);

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
            string str3 = "";
            try
            {
                foreach (byte num in byteInput)
                    str3 = str3 + Conversion.Hex(num).PadLeft(2, '0');
            }
            catch (Exception e)
            { tool.LOG("ERROR: " + e); }
            return str3;
        }
        public static string FormatHexStr(string hexStr)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                for (int i = 0; i <= hexStr.Length - 1; i += 2)
                    builder.Append(hexStr.Substring(i, 2) + " ");
            }
            catch (Exception e)
            {
                tool.LOG("ERROR: " + e);
            }
            return builder.ToString().TrimEnd(new char[0]);
        }

        public static string GetASCIIString(byte[] bytes)
        {
            string str4 = "", str3 = "";
            string str = BytesToHexString(bytes);
            try
            {
                int num2 = str.Length - 1;
                for (int i = 0; i <= num2; i++)
                    if (str.Substring(i, 2) == "00")
                        i++;
                    else
                    {
                        str3 = str3 + str.Substring(i, 2);
                        i++;
                    }
                while (str3.Length > 0)
                {
                    str4 = str4 + Convert.ToChar(Convert.ToUInt64(str3.Substring(0, 2), 0x10)).ToString();
                    str3 = str3.Substring(2, str3.Length - 2);
                }
            }
            catch (Exception e)
            {
                tool.LOG("ERROR: " + e);
                str4 = "-1";
            }
            return str4;
        }

        public static byte[] HexStringToBytes(string strInput)
        {
            byte[] buffer;
            try
            {
                int startIndex = 0;
                int index = 0;
                buffer = new byte[((int)Math.Round((double)((((double)strInput.Length) / 2.0) - 1.0))) + 1];
                while (strInput.Length > (startIndex + 1))
                {
                    buffer[index] = Convert.ToByte(Convert.ToInt64(strInput.Substring(startIndex, 2), 0x10));
                    startIndex += 2;
                    index++;
                }
            }
            catch (Exception)
            {
                tool.LOG("ERROR: Hex String To Byte Array Conversion Error!");
                buffer = null;
            }
            return buffer;
        }

        public static string ReverseHex(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= value.Length - 2; i += 2)
                builder.Append(new string(value.Substring(i, 2).Reverse<char>().ToArray<char>()));
            return FormatHexStr(Microsoft.VisualBasic.Strings.StrReverse(builder.ToString()));
        }
        public static string ReverseHexToIMEI(string hexInput)
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
    }
}

