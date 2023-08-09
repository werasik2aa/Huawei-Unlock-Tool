using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.TOOLS
{
    public class Fastboot
    {
        private const int USB_VID = 0x18D1;
        private const int USB_PID = 0xD00D;
        private const int HEADER_SIZE = 4;
        private const int BLOCK_SIZE = 512 * 1024; // 512 KB

        public int Timeout = 3000;
        public int TimeoutWait = 50;
        private UsbDevice device;

        public enum FastbootStatus
        {
            Failed,
            Ok,
            Data,
            Info,
            Unknown
        }

        public class Response
        {
            public FastbootStatus Status;
            public string Payload;
            public byte[] RawData;

            public Response(FastbootStatus status, string payload)
            {
                Status = status;
                Payload = payload;
            }
        }

        private FastbootStatus GetStatus(string header)
        {
            switch (header)
            {
                case "INFO":
                    return FastbootStatus.Info;
                case "OKAY":
                    return FastbootStatus.Ok;
                case "DATA":
                    return FastbootStatus.Data;
                case "FAIL":
                    return FastbootStatus.Failed;
                default:
                    return FastbootStatus.Unknown;
            }
        }
        public bool Connect()
        {
            if (device != null) Disconnect();
            var counter = 0;
            while (true)
            {
                var allDevices = UsbDevice.AllDevices;
                if (allDevices.Any(x => x.Vid == USB_VID & x.Pid == USB_PID))
                    break;
                if (counter == TimeoutWait)
                {
                    LOG(2, "TimeoutWait error.");
                    return false;
                }

                Thread.Sleep(500);
                counter++;
            }

            UsbDeviceFinder finder;
            finder = new UsbDeviceFinder(USB_VID, USB_PID);

            device = UsbDevice.OpenUsbDevice(finder);

            if (device == null)
            {
                LOG(2, "NoDEVICE");
                return false;
            }

            var wDev = device as IUsbDevice;

            if (wDev is IUsbDevice)
            {
                wDev.SetConfiguration(1);
                wDev.ClaimInterface(0);
            }
            return device != null;
        }
        public void Disconnect()
        {
            device.Close();
        }
        public string GetSerialNumber()
        {
            return device.Info.SerialString;
        }
        public Response Command(byte[] command)
        {
            var writeEndpoint = device.OpenEndpointWriter(WriteEndpointID.Ep01);

            writeEndpoint.Write(command, Timeout, out int WroteNum);

            if (WroteNum != command.Length)
                throw new Exception("Failed to write command! Transfered: " + WroteNum + "of" + command.Length + "bytes");

            //READ point
            FastbootStatus status;
            StringBuilder response = new StringBuilder();
            UsbEndpointReader readEndpoint = device.OpenEndpointReader(ReadEndpointID.Ep01);
            string ASCI;
            while (true)
            {
                byte[] buffer = new byte[64];
                readEndpoint.Read(buffer, Timeout, out int ReadNum);
                ASCI = Encoding.ASCII.GetString(buffer);

                if (ASCI.Length < HEADER_SIZE)
                    status = FastbootStatus.Unknown;
                else
                    status = GetStatus(new string(ASCI.Take(HEADER_SIZE).ToArray()));

                response.Append(ASCI.Skip(HEADER_SIZE).Take(ReadNum - HEADER_SIZE).ToArray());

                response.Append("\n");

                if (status != FastbootStatus.Info)
                    break;
            }

            return new Response(status, response.ToString().Replace("\r", string.Empty).Replace("\0", string.Empty))
            {
                RawData = Encoding.ASCII.GetBytes(ASCI)
            };
        }

        private void SendDataCommand(long size)
        {
            if (Command($"download:{size:X8}").Status != FastbootStatus.Data)
                throw new Exception($"Invalid response from device! (data size: {size})");
        }
        private void TransferBlock(FileStream stream, UsbEndpointWriter writeEndpoint, byte[] buffer, int size)
        {
            stream.Read(buffer, 0, size);
            writeEndpoint.Write(buffer, Timeout, out int wroteSize);

            if (wroteSize != size)
                throw new Exception("Failed to transfer block (sent " + wroteSize + " of " + size + ")");
        }
        public void UploadData(FileStream stream)
        {
            //WRITE_D
            var writeEp = device.OpenEndpointWriter(WriteEndpointID.Ep01);
            var length = stream.Length;
            var buffer = new byte[BLOCK_SIZE];

            SendDataCommand(length);

            while (length >= BLOCK_SIZE)
            {
                TransferBlock(stream, writeEp, buffer, BLOCK_SIZE);
                length -= BLOCK_SIZE;
            }

            if (length > 0)
            {
                buffer = new byte[length];
                TransferBlock(stream, writeEp, buffer, (int)length);
            }

            //READ_ED
            var resBuffer = new byte[64];
            ErrorCode ErrorEr = device.OpenEndpointReader(ReadEndpointID.Ep01).Read(resBuffer, Timeout, out _);
            var strBuffer = Encoding.ASCII.GetString(resBuffer);
            if (strBuffer.Length < HEADER_SIZE)
                throw new Exception("Invalid response from device: " + strBuffer);
            if (GetStatus(new string(strBuffer.Take(HEADER_SIZE).ToArray())) != FastbootStatus.Ok)
                throw new Exception("Invalid status: " + strBuffer);
        }
        public void UploadData(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            UploadData(stream);
        }
        public Response Command(string command)
        {
            return Command(Encoding.ASCII.GetBytes(command));
        }
    }
}