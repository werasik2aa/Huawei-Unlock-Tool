using System;
using System.IO.Ports;
using System.Text;
using System.IO;
using static HuaweiUnlocker.tool;
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
        public static bool Open(string port, int baudrate, Parity parity, int bits, StopBits stopbits, Handshake handsh)
        {
            if (opened)
            {
                OpenedPort.Close();
                OpenedPort = null;
                opened = false;
            }
            OpenedPort = new SerialPort(port, baudrate, parity, bits, stopbits);
            OpenedPort.Handshake = Handshake.RequestToSendXOnXOff;
            OpenedPort.ReadBufferSize = 500;
            OpenedPort.WriteTimeout = 1000;
            OpenedPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            try
            {
                OpenedPort.Open();
            }
            catch
            {
                LOG("ERROR: PORT IS USED BY ANOTHER APPLICATION");
                opened = false;
                return false;
            }
            opened = true;
            OpenedPortName = port;
            OpenedBaudrate = baudrate;
            OpenedParity = parity;
            OpenedBits = bits;
            OpenedStopBits = stopbits;
            LOG("Connected To port");
            return opened;
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
                    Action action = () => LOG(CRC.HexDump(message));
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();
                }
            }
            catch { }
        }
        public static void SendMessge(byte[] data, bool tocrc, bool hexstring)
        {
            if (!opened) return;
            if (!tocrc)
            {
                LOG("SENDED: " + CRC.BytesToHexString(data) + " /|\\ "  + Encoding.ASCII.GetString(data));
                OpenedPort.Write(data, 0, data.Length);
                return;
            }
            byte[] newdata = ENCODE(data, hexstring);
            LOG("SENDED: " + CRC.BytesToHexString(newdata) + " /|\\ " + Encoding.ASCII.GetString(newdata));
            OpenedPort.Write(newdata, 0, newdata.Length);
        }
        public static void SendFile(string data, bool tocrc, bool hexstring)
        {
            if (!opened) return;
            byte[] filedata = File.ReadAllBytes(data);
            if (!tocrc)
            {
                LOG("SENDED FILE: " + data);
                OpenedPort.Write(filedata, 0, filedata.Length);
                return;
            }
            byte[] newdata = ENCODE(filedata, hexstring);
            LOG("SENDED ENCODED+CRC FILE: " + data);
            OpenedPort.Write(newdata, 0, newdata.Length);
        }
        public static void SendMessge(string data, bool tocrc,bool tobytes, bool hexstring)
        {
            if (!opened) return;
            if (!tocrc)
            {
                LOG("SENDED: " + data + " /|\\ ");
                if (!tobytes)
                    OpenedPort.Write(data);
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(data);
                    OpenedPort.Write(bytes, 0, bytes.Length);
                }
                return;
            }
            byte[] newdata = ENCODE(data, hexstring);
            LOG("SENDED: " + CRC.BytesToHexString(newdata) + " " + Encoding.ASCII.GetString(newdata));
            OpenedPort.Write(newdata, 0, newdata.Length);
        }
        public static void SendMessge(uint[] data, bool tobytes, bool tocrc, bool hexstring)
        {
            if (!opened) return;
            byte[] a = GetBytes(data);
            if (!tocrc)
            {
                OpenedPort.Write(a, 0, a.Length);
                return;
            }
            a = ENCODE(data, hexstring);
            BitConverter.GetBytes(13);
            LOG("SENDED: " + CRC.BytesToHexString(a) + " " + Encoding.ASCII.GetString(a));
            OpenedPort.Write(a, 0, a.Length);
        }
        public static Byte[] GetBytes(uint[] data)
        {
            byte[] byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 4);
            return byteArray;
        }

        public static Byte[] ENCODE(uint[] data, bool tohex)
        {
            byte[] byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, data.Length * 4);
            return CRC.GetBufferWithCRC(byteArray);
        }
        public static Byte[] ENCODE(byte[] data, bool hexstring)
        {
            return CRC.GetBufferWithCRC(data);
        }
        public static Byte[] ENCODE(string data, bool hexstring)
        {
            if(hexstring) data.Replace(" ", "");
            return CRC.GetBufferWithCRC(data, hexstring);
        }
    }
}