using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiUnlocker.TOOLS
{
    public class FlashToolLib
    {
        [DllImport("FlashToolLibEx.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int flashtool_startup();
    }
}
