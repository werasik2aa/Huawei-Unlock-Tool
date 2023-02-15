using System;
using System.Runtime.InteropServices;

namespace HuaweiUnlocker.TOOLS
{
    public class UpgradeMode
    {
        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int IsDiagPortOK();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern char CheckUpdateFile();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init(int a1, int a2);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitMultApp(int a1, int a2, int a3);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegisteAuthCallback(int a1);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegisteProcessCallback(int a1);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegisteStatusCallback(int a1);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetRepairPort(int a1);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetUpdateAuth();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetZipPackages(int a1, int a2, int a3);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern char SparseNewRecoveryImg();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern char SparseRecoveryImg();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseFile();

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.ThisCall)]
        public static extern int StartSearchPort(IntPtr ptr);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SystemUpdateAuth(int a1, int a2);

        [DllImport("FirmwareUpdate.dll", CallingConvention = CallingConvention.ThisCall)]
        public static extern char UpdateFirmware(IntPtr ptr);

    }
}
