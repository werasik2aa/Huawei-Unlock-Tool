using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.VisualBasic.CompilerServices;
namespace HuaweiUnlocker
{
    public class QCDM
    {
        public static SerialPort OpenedPort;
        public static string OpenedPortName;
        public static int OpenedBaudrate, OpenedBits;
        public static Parity OpenedParity;
        public static StopBits OpenedStopBits;
        public static bool opened = false;
        public static CheckBox CR, CL, Ads, ass;
        public static bool Open(string port, int baudrate, Parity parity, int bits, StopBits stopbits, Handshake handsh)
        {
            if (!opened && (OpenedPort = new SerialPort(port, baudrate, parity, bits, stopbits)) != null)
            {
                OpenedPort.Handshake = Handshake.RequestToSendXOnXOff;
                OpenedPort.ReadBufferSize = 500;
                OpenedPort.WriteTimeout = 1000;
                OpenedPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                OpenedPort.Open();
                opened = true;
                OpenedPortName = port;
                OpenedBaudrate = baudrate;
                OpenedParity = parity;
                OpenedBits = bits;
                OpenedStopBits = stopbits;
            }
            return opened;
        }
        public static void SendMessge(byte[] data)
        {
            if (!opened) return;
            data = CRC.GetBufferWithCRC(CRC.BytesToHexString(data), ass.Checked);
            OpenedPort.Write(data, 0, data.Length);
        }
        public static void SendMessge(string data)
        {
            if (!opened) return;
            byte[] command;
            data = data.Replace(" ", "");
            if (data.Length < 1)
            {
                tool.LOG("ERROR: Data lenght < 1");
                return;
            }
            command = CRC.GetBufferWithCRC(data, ass.Checked);
            tool.LOG("INFO: Sending: " + CRC.BytesToHexString(command) + Environment.NewLine + Environment.NewLine);
            OpenedPort.Write(command, 0, command.Length);
        }
        private static void DataReceived(object sender, EventArgs e)
        {
            if (!opened) return;
            SerialPort sp = (SerialPort)sender;
            try
            {
                while (sp.ReadByte() != -1)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] message = new byte[sp.BytesToRead];
                    for (int pos = 0; pos < sp.BytesToRead; pos++) message[pos] = (byte)sp.ReadByte();

                    Action action = () => tool.LOG(Hex.HexDump(message));
                    if (tool.LOGGE.InvokeRequired)
                        tool.LOGGE.Invoke(action);
                    else
                        action();
                }
            }
            catch { OpenedPort.Close(); opened = false; }
        }
        public static Byte[] BytesFromHexString(string strInput)
        {
            Byte[] bytArOutput = new Byte[] { };
            if (!string.IsNullOrEmpty(strInput) && strInput.Length % 2 == 0)
            {
                SoapHexBinary hexBinary = null;
                try
                {
                    hexBinary = SoapHexBinary.Parse(strInput);
                    if (hexBinary != null)
                        bytArOutput = hexBinary.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return bytArOutput;
        }
    }
}