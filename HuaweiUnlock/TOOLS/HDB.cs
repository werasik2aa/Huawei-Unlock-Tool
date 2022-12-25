using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace HuaweiUnlocker.TOOLS
{
    public class HDB
    {
        private const int USB_VID = 0x12D1;
        private const int USB_PID = 0x107D;
        public static UsbDevice MyUsbDevice;
        public void ALLdev()
        {
            ErrorCode ec = ErrorCode.None;
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            LangProc.LOG("=================================================================");
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if (usbRegistry.Open(out MyUsbDevice))
                {
                    UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep05);
                    UsbEndpointWriter writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep05);

                    ec = writer.Write(Encoding.ASCII.GetBytes("26720056027E"), 2000, out int bytesWritten);
                    if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);
                    byte[] readBuffer = new byte[1024];

                    while (ec == ErrorCode.None)
                    {
                        int bytesRead;
                        ec = reader.Read(readBuffer, 100, out bytesRead);
                        LangProc.LOG("FUCK FUCK FUCK: "+Encoding.Default.GetString(readBuffer, 0, bytesRead));
                    }
                }
            }
            LangProc.LOG("=================================================================");
        }
        public void Mafin()
        {

            
        }
    }
}