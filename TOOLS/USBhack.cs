using System;
using System.Runtime.InteropServices;
using System.Windows;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.TOOLS
{
    public class USBhack
    {
        [DllImport("hwbc.dll", CallingConvention = CallingConvention.ThisCall)]
        public static extern int ConnectPhone(int e);
        [DllImport("hwbc.dll", CallingConvention = CallingConvention.ThisCall)]
        public static extern int GetAuthorizationData(int e);

        public static void TRY()
        {
            try
            {
                GetAuthorizationData(41);
            }
            catch (Exception e)
            {
                LOG(e.ToString());
            }
        }
    }
}
