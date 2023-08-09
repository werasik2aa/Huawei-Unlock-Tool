using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet;
using static HuaweiUnlocker.LangProc;
using System.Collections.ObjectModel;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static HuaweiUnlocker.TOOLS.Fastboot;
using System.Linq;
using System.Text;
using System.Threading;
using System;

namespace HuaweiUnlocker.TOOLS
{
    public class QCusb
    {
        public static UsbDevice MyUsbDevice;
        public static bool Connect()
        {
            // Dump all devices and descriptor information to console output.
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if (usbRegistry.Open(out MyUsbDevice))
                {
                    if(MyUsbDevice.Info.ManufacturerString.ToUpper().Contains("HUAWEI"))
                    {
                        LOG(0, "Found device: " + MyUsbDevice.Info.SerialString);
                        UsbConfigInfo configInfo = MyUsbDevice.Configs[0];
                        UsbInterfaceInfo interfaceInfo = configInfo.InterfaceInfoList[0];
                        LOG(0, "InterfaceLen: " + configInfo.InterfaceInfoList.Count);
                        ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                        for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
                        {
                            LOG(0, "ENDPOINT: " + endpointList[iEndpoint].ToString());
                        }
                        MyUsbDevice.Open();
                        if (MyUsbDevice == null)
                        {
                            LOG(2, "NoDEVICE");
                            return false;
                        }

                        var wDev = MyUsbDevice as IUsbDevice;

                        if (wDev is IUsbDevice)
                        {
                            wDev.SetConfiguration(3);
                            wDev.ClaimInterface(0);
                        }
                        return true;
                    }
                }
            }
            UsbDevice.Exit();
            return false;
        }
        public static string Command(byte[] command)
        {
            UsbEndpointWriter writeEndpoint = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep05);
            writeEndpoint.Write(command, 3000, out int WroteNum);
            if (WroteNum != command.Length)
                throw new Exception("Failed to write command! Transfered: " + WroteNum + "of" + command.Length + "bytes");

            UsbEndpointReader readEndpoint = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep05);
            string ASCI = "";
            byte[] buffer = new byte[1024];
            while (readEndpoint.Read(buffer, 3000, out int ReadNum) != ErrorCode.UnknownError)
            {
                ASCI = Encoding.ASCII.GetString(buffer);
                Thread.Sleep(1000);
                LOG(0, ASCI);
                if (string.IsNullOrEmpty(ASCI)) break;
            }
            return ASCI;
        }
    }
}
