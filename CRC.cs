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
        public const ulong CRC_SEED = 0xffffUL;
        public static readonly ulong[] CRCTable = new ulong[] {
            0x0000UL, 0x1189UL, 0x2312UL, 0x329BUL, 0x4624UL, 0x57ADUL, 0x6536UL, 0x74BFUL,
            0x8C48UL, 0x9DC1UL, 0xAF5AUL, 0xBED3UL, 0xCA6CUL, 0xDBE5UL, 0xE97EUL, 0xF8F7UL,
            0x1081UL, 0x0108UL, 0x3393UL, 0x221AUL, 0x56A5UL, 0x472CUL, 0x75B7UL, 0x643EUL,
            0x9CC9UL, 0x8D40UL, 0xBFDBUL, 0xAE52UL, 0xDAEDUL, 0xCB64UL, 0xF9FFUL, 0xE876UL,
            0x2102UL, 0x308BUL, 0x0210UL, 0x1399UL, 0x6726UL, 0x76AFUL, 0x4434UL, 0x55BDUL,
            0xAD4AUL, 0xBCC3UL, 0x8E58UL, 0x9FD1UL, 0xEB6EUL, 0xFAE7UL, 0xC87CUL, 0xD9F5UL,
            0x3183UL, 0x200AUL, 0x1291UL, 0x0318UL, 0x77A7UL, 0x662EUL, 0x54B5UL, 0x453CUL,
            0xBDCBUL, 0xAC42UL, 0x9ED9UL, 0x8F50UL, 0xFBEFUL, 0xEA66UL, 0xD8FDUL, 0xC974UL,
            0x4204UL, 0x538DUL, 0x6116UL, 0x709FUL, 0x0420UL, 0x15A9UL, 0x2732UL, 0x36BBUL,
            0xCE4CUL, 0xDFC5UL, 0xED5EUL, 0xFCD7UL, 0x8868UL, 0x99E1UL, 0xAB7AUL, 0xBAF3UL,
            0x5285UL, 0x430CUL, 0x7197UL, 0x601EUL, 0x14A1UL, 0x0528UL, 0x37B3UL, 0x263AUL,
            0xDECDUL, 0xCF44UL, 0xFDDFUL, 0xEC56UL, 0x98E9UL, 0x8960UL, 0xBBFBUL, 0xAA72UL,
            0x6306UL, 0x728FUL, 0x4014UL, 0x519DUL, 0x2522UL, 0x34ABUL, 0x0630UL, 0x17B9UL,
            0xEF4EUL, 0xFEC7UL, 0xCC5CUL, 0xDDD5UL, 0xA96AUL, 0xB8E3UL, 0x8A78UL, 0x9BF1UL,
            0x7387UL, 0x620EUL, 0x5095UL, 0x411CUL, 0x35A3UL, 0x242AUL, 0x16B1UL, 0x0738UL,
            0xFFCFUL, 0xEE46UL, 0xDCDDUL, 0xCD54UL, 0xB9EBUL, 0xA862UL, 0x9AF9UL, 0x8B70UL,
            0x8408UL, 0x9581UL, 0xA71AUL, 0xB693UL, 0xC22CUL, 0xD3A5UL, 0xE13EUL, 0xF0B7UL,
            0x0840UL, 0x19C9UL, 0x2B52UL, 0x3ADBUL, 0x4E64UL, 0x5FEDUL, 0x6D76UL, 0x7CFFUL,
            0x9489UL, 0x8500UL, 0xB79BUL, 0xA612UL, 0xD2ADUL, 0xC324UL, 0xF1BFUL, 0xE036UL,
            0x18C1UL, 0x0948UL, 0x3BD3UL, 0x2A5AUL, 0x5EE5UL, 0x4F6CUL, 0x7DF7UL, 0x6C7EUL,
            0xA50AUL, 0xB483UL, 0x8618UL, 0x9791UL, 0xE32EUL, 0xF2A7UL, 0xC03CUL, 0xD1B5UL,
            0x2942UL, 0x38CBUL, 0x0A50UL, 0x1BD9UL, 0x6F66UL, 0x7EEFUL, 0x4C74UL, 0x5DFDUL,
            0xB58BUL, 0xA402UL, 0x9699UL, 0x8710UL, 0xF3AFUL, 0xE226UL, 0xD0BDUL, 0xC134UL,
            0x39C3UL, 0x284AUL, 0x1AD1UL, 0x0B58UL, 0x7FE7UL, 0x6E6EUL, 0x5CF5UL, 0x4D7CUL,
            0xC60CUL, 0xD785UL, 0xE51EUL, 0xF497UL, 0x8028UL, 0x91A1UL, 0xA33AUL, 0xB2B3UL,
            0x4A44UL, 0x5BCDUL, 0x6956UL, 0x78DFUL, 0x0C60UL, 0x1DE9UL, 0x2F72UL, 0x3EFBUL,
            0xD68DUL, 0xC704UL, 0xF59FUL, 0xE416UL, 0x90A9UL, 0x8120UL, 0xB3BBUL, 0xA232UL,
            0x5AC5UL, 0x4B4CUL, 0x79D7UL, 0x685EUL, 0x1CE1UL, 0x0D68UL, 0x3FF3UL, 0x2E7AUL,
            0xE70EUL, 0xF687UL, 0xC41CUL, 0xD595UL, 0xA12AUL, 0xB0A3UL, 0x8238UL, 0x93B1UL,
            0x6B46UL, 0x7ACFUL, 0x4854UL, 0x59DDUL, 0x2D62UL, 0x3CEBUL, 0x0E70UL, 0x1FF9UL,
            0xF78FUL, 0xE606UL, 0xD49DUL, 0xC514UL, 0xB1ABUL, 0xA022UL, 0x92B9UL, 0x8330UL,
            0x7BC7UL, 0x6A4EUL, 0x58D5UL, 0x495CUL, 0x3DE3UL, 0x2C6AUL, 0x1EF1UL, 0x0F78UL};
        public const byte ESC_ASYNC = 0x7d;
        public const byte ESC_COMPL = 0x20;
        public const byte FLAG_ASYNC = 0x7e;
        public const int PRLPacketSize = 40;

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

        public static byte[] GetBufferWithCRC(string s, bool hex)
        {
            if (!hex)
                return GetBufferWithCRC(Encoding.ASCII.GetBytes(s), (int)Math.Round((double)(((double)s.Length) / 2.0)));
            else
                return GetBufferWithCRC(HexStringToBytes(s), (int)Math.Round((double)(((double)s.Length) / 2.0)));
        }
        public static byte[] GetBufferWithCRC(byte[] s, bool hex)
        {
            return GetBufferWithCRC(s, (int)Math.Round((double)(((double)s.Length) / 2.0)));
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
    }
}