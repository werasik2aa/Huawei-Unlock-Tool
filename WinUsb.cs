using MadWizard.WinUSBNet;

namespace HuaweiUnlocker
{
    public class WinUsb
    {
        public static USBDevice device;
        public static USBDevice GetDevice(string guid)
        {
            return USBDevice.GetSingleDevice(guid);
        }
        public static byte[] Read()
        {
            //return device.ControlIn(requestType, request, value, 0, 512);
            return null;
        }
    }
}
