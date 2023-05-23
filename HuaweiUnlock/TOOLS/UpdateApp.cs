using HuaweiUpdateLibrary.Core;
using HuaweiUpdateLibrary;
using Microsoft.VisualBasic.Logging;

namespace HuaweiUnlocker.TOOLS
{
    public class UpdateApp
    {
        public static void UnpackAPP(string path)
        {
            foreach (var o in UpdateFile.Open("1.APP"))
            {
                LangProc.LOG(-1, o.FileType);
            }
        }
    }
}
