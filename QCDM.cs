using System;
using System.IO.Ports;
using System.Text;

namespace HuaweiUnlocker
{
    public class QCDM
    {
        public static SerialPort OpenedPort = null;
        public static string OpenedPortName = "";
        private static int OpenedBaudrate, OpenedBits;
        private static Parity OpenedParity;
        private static StopBits OpenedStopBits;
        public static bool opened = false;
        public static bool tohex,encode = false;
        public static bool Open(string port, int baudrate, Parity parity, int bits, StopBits stopbits, Handshake handsh)
        {
            if (opened)
            {
                OpenedPort.Close();
                OpenedPort = null;
                opened = false;
            }
            OpenedPort = new SerialPort(port, baudrate, parity, bits, stopbits);
            OpenedPort.Handshake = Handshake.XOnXOff;
            OpenedPort.ReadBufferSize = 500;
            OpenedPort.WriteTimeout = 1000;
            OpenedPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            try
            {
                opened = true;
                OpenedPort.Open();
            }
            catch
            {
                tool.LOG("ERROR: PORT IS USED BY ANOTHER APPLICATION");
                return false;
            }
            OpenedPortName = port;
            OpenedBaudrate = baudrate;
            OpenedParity = parity;
            OpenedBits = bits;
            OpenedStopBits = stopbits;
            tool.LOG("Connected To port");
            return opened;
        }
        public static void SendMessge(byte[] data)
        {
            if (!opened) return;
            if (!encode)
            {
                tool.LOG("SENDED: " + CRC.BytesToHexString(data) + " " + Encoding.ASCII.GetString(data));
                OpenedPort.Write(data, 0, data.Length);
                return;
            }
            byte[] a = ENCODE(data);
            tool.LOG("SENDED: " + CRC.BytesToHexString(a) + " " + Encoding.ASCII.GetString(a));
            OpenedPort.Write(a, 0, a.Length);
        }
        public static void SendMessge(string data)
        {
            if (!opened) return;
            if (!encode)
            {
                OpenedPort.Write(data+"\n\r");
                return;
            }
            byte[] a = ENCODE(data);
            tool.LOG("SENDED: " + CRC.BytesToHexString(a) + " " + Encoding.ASCII.GetString(a));
            OpenedPort.Write(a, 0, a.Length);
        }
        public static void SendMessge(uint[] data)
        {
            if (!opened) return;
            byte[] a = GetBytes(data);
            if (!encode)
            {
                OpenedPort.Write(a, 0, a.Length);
                return;
            }
            a = ENCODE(data);
            BitConverter.GetBytes(13);
            tool.LOG("SENDED: " + CRC.BytesToHexString(a) + " " + Encoding.ASCII.GetString(a));
            OpenedPort.Write(a, 0, a.Length);
        }
        public static void DataReceived(object sender, EventArgs e)
        {
            if (!opened) return;
            SerialPort sp = (SerialPort)sender;
            try
            {
                if (sp.ReadByte() != -1)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] message = new byte[sp.BytesToRead];
                    for (int pos = 0; pos < sp.BytesToRead; pos++) message[pos] = (byte)sp.ReadByte();
                    Action action = () => tool.LOG(CRC.HexDump(message));
                    if (tool.LOGGE.InvokeRequired)
                        tool.LOGGE.Invoke(action);
                    else
                        action();
                }
            }
            catch { }
        }
        public static Byte[] GetBytes(uint[] data)
        {
            byte[] byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 4);
            return byteArray;
        }
        public static Byte[] ENCODE(uint[] data)
        {
            byte[] byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 4);
            return CRC.GetBufferWithCRC(byteArray, tohex);
        }
        public static Byte[] ENCODE(byte[] data)
        {
            return CRC.GetBufferWithCRC(data, tohex);
        }
        public static Byte[] ENCODE(string data)
        {
            return CRC.GetBufferWithCRC(data, tohex);
        }
    }
}