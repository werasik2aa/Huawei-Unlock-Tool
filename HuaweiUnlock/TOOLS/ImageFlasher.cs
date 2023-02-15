using HuaweiUnlocker.DIAGNOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace HuaweiUnlocker.TOOLS
{
    public class ImageFlasher
    {
        private const int BAUDRATE = 115200;
        private const int MAX_DATA_LEN = 0x400;
        private readonly static byte[] headframe = new byte[] { 0xFE, 0x00, 0xFF, 0x01 };
        private readonly static byte[] dataframe = new byte[] { 0xDA };
        private readonly static byte[] tailframe = new byte[] { 0xED };

        public void Write(string path, int address, Action<int> reportProgress = null)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var length = (int)stream.Length;
            var nframes = length / (MAX_DATA_LEN + (length % MAX_DATA_LEN > 0 ? 1 : 0));
            var n = 0;
            var buffer = new byte[MAX_DATA_LEN];

            SendHeadFrame(length, address);

            while (length > MAX_DATA_LEN)
            {
                stream.Read(buffer, 0, MAX_DATA_LEN);
                SendDataFrame(n + 1, buffer);

                n++;
                length -= MAX_DATA_LEN;

                if (n % (nframes > 250 ? 10 : 3) == 0)
                    reportProgress?.Invoke((int)(100f * n / nframes));
            }

            if (length > 0)
            {
                buffer = new byte[length];
                stream.Read(buffer, 0, length);
                SendDataFrame(n + 1, buffer);
            }

            reportProgress?.Invoke(100);
            SendTailFrame(n + 2);
        }

        private void SendHeadFrame(int length, int address)
        {
            var data = new List<byte>(headframe);

            data.AddRange(BitConverter.GetBytes(length).Reverse());
            data.AddRange(BitConverter.GetBytes(address).Reverse());

            SendFrame(data.ToArray());
        }

        private void SendDataFrame(int n, byte[] data)
        {
            var frame = new List<byte>(dataframe)
            {
                (byte)(n & 0xFF),
                (byte)((~n) & 0xFF)
            };

            frame.AddRange(data);

            SendFrame(frame.ToArray());
        }

        private void SendTailFrame(int n)
        {
            var frame = new List<byte>(tailframe)
            {
                (byte)(n & 0xFF),
                (byte)((~n) & 0xFF)
            };

            SendFrame(frame.ToArray());
        }

        private void SendFrame(byte[] data)
        {
            var crc = CRC.GetChecksum(data);

            var frameList = new List<byte>(data)
            {
                (byte)((crc >> 8) & 0xFF),
                (byte)(crc & 0xFF)
            };

            var count = frameList.Count();
            var frame = frameList.ToArray();

            SerialManager.Port.Write(frame, 0, count);

            byte _ack = (byte)SerialManager.Port.ReadByte();

            SerialManager.Port.DiscardInBuffer();
            SerialManager.Port.DiscardOutBuffer();

            if (_ack != 0xAA)
                throw new Exception(string.Format("ACK is invalid! ACK={0:X2}; Excepted={1:X2}", _ack, 0xAA));
        }
    }
}