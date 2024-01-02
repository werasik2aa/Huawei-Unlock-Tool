using System;
using System.IO.Ports;
using System.IO;
using static HuaweiUnlocker.LangProc;
using System.Threading;
using System.Collections.Generic;
using HuaweiUnlocker.DIAGNOS;
using System.ComponentModel;

namespace HuaweiUnlocker.TOOLS
{
    public class SerialManager
    {
        public static SerialPort Port = new SerialPort();
        private static FileStream ReadTempFile;
        public static bool Open(string port, int baudrate, bool isFileStream)
        {
            if (Port.IsOpen)
                if (Port.PortName != port)
                    CloseSerial();
                else return true;
            if (debug) LOG(-1, "[DBG] PORT INTERACT: " + port);
            Port = new SerialPort
            {
                PortName = port,
                BaudRate = baudrate,
                DtrEnable = true,
                RtsEnable = true,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
            if (isFileStream)
                Port.DataReceived += new SerialDataReceivedEventHandler(FileWritter);
            try
            {
                Port.Open();
            }
            catch
            {
                LOG(2, "DeviceNotConDIAG");
            }
            if (Port.IsOpen && port != Port.PortName)
                LOG(2, "DeviceConDiag", port);
            return Port.IsOpen;
        }
        public static void FileWritter(object sender, EventArgs e)
        {
            if (!Port.IsOpen) return;
            SerialPort sp = (SerialPort)sender;
            try
            {
                if (Port.IsOpen)
                {
                    if (!File.Exists("Temp/tempDUMP.bin"))
                        ReadTempFile = new FileStream("Temp/tempDUMP.bin", FileMode.OpenOrCreate);
                    byte[] msg = new byte[Port.BytesToRead];
                    Port.Read(msg, 0, Port.BytesToRead);
                    ReadTempFile.Write(msg, 0, msg.Length);
                }
                else return;
            }
            catch { }
        }
        public static void LogWritter(object sender, EventArgs e)
        {
            if (!Port.IsOpen) return;
            SerialPort sp = (SerialPort)sender;
            try
            {
                if (Port.IsOpen)
                {
                    byte[] msg = new byte[Port.BytesToRead];
                    Port.Read(msg, 0, Port.BytesToRead);
                    LOG(-1, CRC.HexDump(msg));
                }
                else return;
            }
            catch { }
        }
        public static void CloseSerial()
        {
            LOG(-1, "[DBG] Clossing Connection To Port: " + Port.PortName);
            Port.Close();
            Port.Dispose();
        }
        public static byte[] Read(bool tofile, string path)
        {
            if (!Port.IsOpen)
            {
                LOG(2, "DeviceNotCon");
                return null;
            }
            Thread.Sleep(1000);
            List<byte> msg = new List<byte>();
            try
            {
                while (Port.BytesToRead > 0)
                {
                    if (!Port.IsOpen)
                    {
                        LOG(2, "DeviceNotCon");
                        return null;
                    }
                    msg.Add((byte)Port.ReadByte());
                }
                byte[] decoded = msg.ToArray();
                if (tofile)
                {
                    FileStream file = new FileStream(path, FileMode.OpenOrCreate);
                    file.WriteAsync(decoded, decoded.Length, decoded.Length);
                }
                LOG(-1, "[DBG] Reading: " + decoded.Length + " bytes" + Environment.NewLine + CRC.HexDump(decoded) + Environment.NewLine);
                return decoded;
            }
            catch { return null; }
        }
        public static bool Write(string str, bool crc, bool isHex)
        {
            if (!Port.IsOpen)
            {
                LOG(2, "DeviceNotCon");
                return false;
            }
            try
            {
                if (isHex)
                {
                    byte[] decoded = CRC.HexStringToBytes(str);
                    if (crc)
                    {
                        byte[] encoded = ENCODE(decoded);
                        if (debug) LOG(-1, "[DBG] Writing: CRCdata:" + newline + CRC.HexDump(encoded));
                        Port.Write(encoded, 0, encoded.Length);
                    }
                    else
                    {
                        if (debug) LOG(-1, "[DBG] Writing:" + newline + CRC.HexDump(decoded));
                        Port.Write(decoded, 0, decoded.Length);
                    }
                }
                else Port.Write(str + "\n\r~");
            }
            catch { return false; }
            return true;
        }
        public static bool Write(byte[] decoded, bool crc)
        {
            if (!Port.IsOpen)
            {
                LOG(2, "DeviceNotCon");
                return false;
            }
            try
            {
                if (crc)
                {
                    byte[] encoded = ENCODE(decoded);
                    LOG(-1, "[DBG] Writing:: CRCdata: \n" + CRC.HexDump(encoded));
                    Port.Write(encoded, 0, encoded.Length);
                    return true;
                }
                LOG(-1, "[DBG] Writing:: \n" + CRC.HexDump(decoded));
                Port.Write(decoded, 0, decoded.Length);
            }
            catch { return false; }
            return true;
        }
        public static byte[] ENCODE(byte[] data)
        {
            return CRC.GetBufferWithCRC(data, (int)Math.Round(data.Length / 2.0));
        }
    }
}