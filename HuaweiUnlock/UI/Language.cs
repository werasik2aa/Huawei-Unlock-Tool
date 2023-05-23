using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiUnlocker.UI
{
    public class Language
    {
        private static Dictionary<string, string> LanguageStrs = new Dictionary<string, string>();
        public static string CURRENTlanguage = "English";
        public static bool ReadLngFile()
        {
            LanguageStrs = new Dictionary<string, string>();
            Stream ss = File.OpenRead("Languages\\" + CURRENTlanguage + ".ini");
            StreamReader readerL = new StreamReader(ss);
            string line = readerL.ReadLine();
            while ((line = readerL.ReadLine()) != null)
                if (!line.StartsWith("[") && !line.StartsWith("#")) LanguageStrs.Add(line.Split(',')[0], line.Split(',')[1]);
            readerL.Close();
            readerL.Dispose();
            ss.Close();
            ss.Dispose();
            return LanguageStrs.Count > 0;
        }
        public static string Get(string name)
        {
            return LanguageStrs[name];
        }
        public static bool isExist(string name)
        {
            return LanguageStrs.ContainsKey(name);
        }
    }
}
