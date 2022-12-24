using System;
using System.IO.Ports;
using System.IO;
using static HuaweiUnlocker.MISC;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HuaweiUnlocker
{
    public class SerialManager
    {
        public static SerialPort Port = new SerialPort();
        private static FileStream ReadTempFile;
        public static bool Open(string port, int baudrate, Parity parity, int bits, StopBits stopbits, Handshake handsh, bool isFileStream)
        {
            if (Port.IsOpen)
                if (Port.PortName != port)
                    CloseSerial();
                else return true;

            Port = new SerialPort(port, baudrate, parity, bits, stopbits);
            Port.Handshake = Handshake.RequestToSendXOnXOff;
            if (isFileStream)
                Port.DataReceived += new SerialDataReceivedEventHandler(FileWritter);
            if (MISC.debug)
                Port.DataReceived += new SerialDataReceivedEventHandler(LogWritter);
            Port.ReadBufferSize = 500;
            Port.WriteTimeout = 1000;
            try
            {
                Port.Open();
            }
            catch
            {
                LOG("ERROR: PORT IS USED BY ANOTHER APPLICATION OR NOT AVAILABLE");
            }
            if (Port.IsOpen && port != Port.PortName)
                LOG("Connected To port: " + port);
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
                    LOG(CRC.HexDump(msg));
                }
                else return;
            }
            catch { }
        }
        public static void CloseSerial()
        {
            LOG("Clossing Connection To Port: " + Port.PortName);
            Port.Close();
        }
        public static byte[] Read(bool tofile, string path)
        {
            if (!Port.IsOpen)
            {
                LOG(E("DeviceNotCon"));
                return null;
            }
            //Thread.Sleep(100);
            List<byte> msg = new List<byte>();
            try
            {
                if (MISC.debug) LOG("[DBG] Reading:");
                while (Port.BytesToRead > 0)
                {
                    if (!Port.IsOpen)
                    {
                        LOG(E("DeviceNotCon"));
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
                return decoded;
            }
            catch { return null; }
        }
        public static bool Write(string hexstr, bool crc, bool isHex)
        {
            if (!Port.IsOpen)
            {
                LOG(E("DeviceNotCon"));
                return false;
            }
            try
            {
                if (isHex)
                {
                    byte[] decoded = CRC.HexStringToBytes(hexstr);
                    if (crc)
                    {
                        byte[] encoded = ENCODE(decoded);
                        if (MISC.debug) LOG("[DBG] Writing: CRCdata:" + newline + CRC.HexDump(encoded));
                        Port.Write(encoded, 0, encoded.Length);
                    }
                    else
                    {
                        if (MISC.debug) LOG("[DBG] Writing:" + newline + CRC.HexDump(decoded));
                        Port.Write(decoded, 0, decoded.Length);
                    }
                }
                else Port.Write(hexstr + "\n\r~");
            }
            catch { return false; }
            return true;
        }
        public static bool Write(string path, bool crc, bool isHex, bool toBytes)
        {
            if (!Port.IsOpen)
            {
                LOG(E("DeviceNotCon"));
                return false;
            }
            try
            {
                if (!toBytes)
                {
                    foreach (string line in System.IO.File.ReadLines(path))
                    {
                        byte[] decoded = CRC.HexStringToBytes(line);
                        if (crc)
                        {
                            byte[] encoded = ENCODE(decoded);
                            LOG("Writing CRCdata:" + newline + CRC.HexDump(decoded));
                            Port.Write(encoded, 0, encoded.Length);
                            return true;
                        }
                        else
                        {
                            LOG("Writing:" + newline + CRC.HexDump(decoded));
                            Port.Write(decoded, 0, decoded.Length);
                        }
                    }
                }
                else
                {
                    byte[] decoded = File.ReadAllBytes(path);
                    if (crc)
                    {
                        byte[] encoded = ENCODE(decoded);
                        LOG("Writing: CRCdata: \n" + CRC.HexDump(encoded));
                        Port.Write(encoded, 0, encoded.Length);
                    }
                    else
                    {
                        LOG("Writing: \n" + CRC.HexDump(decoded));
                        Port.Write(decoded, 0, decoded.Length);
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public static bool Write(byte[] decoded, bool crc, bool isHex)
        {
            if (!Port.IsOpen)
            {
                LOG(E("DeviceNotCon"));
                return false;
            }
            try
            {
                if (crc)
                {
                    byte[] encoded = ENCODE(decoded);
                    LOG("Writing: CRCdata: \n" + CRC.HexDump(encoded));
                    Port.Write(encoded, 0, encoded.Length);
                    return true;
                }
                LOG("Writing: \n" + CRC.HexDump(decoded));
                Port.Write(decoded, 0, decoded.Length);
            }
            catch { return false; }
            return true;
        }
        public static Byte[] ENCODE(byte[] data)
        {
            return CRC.GetBufferWithCRC(data);
        }
        public static Byte[] ENCODE(string data, bool hexstring)
        {
            if (hexstring) data.Replace(" ", "");
            return CRC.GetBufferWithCRC(data, hexstring);
        }
        public static bool OpenPort(string port, bool isFileStream)
        {
            if(debug)LOG("[DBG] PORT INTERACT: "+port);
            return Open(port, 115200, Parity.None, 8, StopBits.OnePointFive, Handshake.None, isFileStream);
        }
    }
}