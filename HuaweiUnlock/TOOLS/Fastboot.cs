using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.TOOLS
{
    public class Fastboot
    {
        private const int USB_VID = 0x18D1;
        private const int USB_PID = 0xD00D;
        private const int HEADER_SIZE = 4;
        private const int BLOCK_SIZE = 512 * 1024;

        public int DefaultRWTimeout = 500000;
        public int DefaultTimeoutWait = 100;
        public UsbDevice device;
        UsbEndpointWriter WriteEp;
        UsbEndpointReader ReadEp;

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
        public bool Connect(int waittime = 100)
        {
            if (waittime == 0) waittime = DefaultTimeoutWait;
            if (device != null) Disconnect();
            var counter = 0;
            while (true)
            {
                var allDevices = UsbDevice.AllDevices;
                if (allDevices.Any(x => x.Vid == USB_VID & x.Pid == USB_PID))
                    break;
                if (counter == DefaultTimeoutWait)
                {
                    LOG(2, "TimeoutWait error.");
                    return false;
                }

                Thread.Sleep(500);
                counter++;
                Application.DoEvents();
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
            WriteEp = device.OpenEndpointWriter(WriteEndpointID.Ep01);
            ReadEp = device.OpenEndpointReader(ReadEndpointID.Ep01);
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

            WriteEp.Write(command, DefaultRWTimeout, out int WroteNum);

            if (WroteNum != command.Length)
                throw new Exception("Failed to write command! Transfered: " + WroteNum + "of" + command.Length + "bytes");

            //READ point
            FastbootStatus status;
            StringBuilder response = new StringBuilder();
            string ASCI;
            while (true)
            {
                byte[] buffer = new byte[64];
                ReadEp.Read(buffer, DefaultRWTimeout, out int ReadNum);
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

        private bool SendDataCommand(long size)
        {
            var res = Command($"download:{size:X8}");
            if (res.Status != FastbootStatus.Data)
                throw new Exception($"Invalid response from device! (data size: {size})");
            if (res.Payload.Contains("too large"))
                LOG(2, "EwPE", "Partition size Too Large");
            return !res.Payload.Contains("too large");
        }
        private void TransferBlock(FileStream stream, byte[] buffer, int size)
        {
            stream.Read(buffer, 0, size);
            WriteEp.Write(buffer, DefaultRWTimeout, out int wroteSize);

            if (wroteSize != size)
                throw new Exception("Failed to transfer block (sent " + wroteSize + " of " + size + ")");
        }
        public bool UploadData(string path, string partname)
        {
            //WRITE_D
            FileStream stream = new FileStream(path, FileMode.Open);
            string madx = Command("getvar:max-download-size").Payload;
            int MAX_DWN_SIZE = (int)new Int32Converter().ConvertFromString(madx);
            Progress(0, (int)stream.Length);
            int i = 1;
            long totalenremain = stream.Length;
            if (SendDataCommand(totalenremain))
            {
                while (totalenremain > 0)
                {
                    long curlenremain = totalenremain >= MAX_DWN_SIZE ? MAX_DWN_SIZE : totalenremain;
                    LOG(0, "[Fastboot] Sending: ", curlenremain == MAX_DWN_SIZE ? partname + " Part: " + (i++) : partname);
                    while (curlenremain > 0)
                    {
                        if (curlenremain < BLOCK_SIZE)
                        {
                            TransferBlock(stream, new byte[curlenremain], (int)curlenremain);
                            totalenremain -= curlenremain;
                            curlenremain = 0;
                        }
                        else
                        {
                            TransferBlock(stream, new byte[BLOCK_SIZE], BLOCK_SIZE);
                            totalenremain -= BLOCK_SIZE;
                            curlenremain -= BLOCK_SIZE;
                        }
                        Progress((int)((stream.Length - totalenremain) / stream.Length) * 100);
                    }
                    //READ_ED
                    var resBuffer = new byte[64];
                    ReadEp.Read(resBuffer, DefaultRWTimeout, out _);
                    var strBuffer = Encoding.ASCII.GetString(resBuffer);
                    if (strBuffer.Length < HEADER_SIZE)
                        throw new Exception("Invalid response from device: " + strBuffer);
                    if (GetStatus(new string(strBuffer.Take(HEADER_SIZE).ToArray())) != FastbootStatus.Ok)
                        throw new Exception("Invalid status: " + strBuffer);
                    else
                    {
                        LOG(0, "[Fastboot] Writing: ", partname);
                        if (partname.Equals("gpt")) partname = "partition";
                        var res = Command("flash:" + partname).Payload;
                        if (res.Contains("table doesn't exist"))
                        {
                            LOG(1, "Skip Partition. ", res);
                            break;
                        }
                    }
                }
            }
            else
            {
                stream.Close();
                stream.Dispose();
                return false;
            }
            stream.Close();
            stream.Dispose();
            return true;
        }
        public Response Command(string command)
        {
            return Command(Encoding.ASCII.GetBytes(command));
        }
    }
}