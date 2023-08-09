using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HuaweiUnlocker.UI
{
    public class Language
    {
        private static Dictionary<string, string> LanguageStrs = new Dictionary<string, string>();
        public static string CURRENTlanguage = "English";
        public static bool ReadLngFile()
        {
            LanguageStrs = new Dictionary<string, string>();
            StreamReader readerL = new StreamReader(File.OpenRead("Languages\\" + CURRENTlanguage + ".ini"));
            string line;
            while ((line = readerL.ReadLine()) != null)
                if (!line.StartsWith("[") && !line.StartsWith("#")) LanguageStrs.Add(line.Split(',')[0], line.Split(',')[1]);
            readerL.Close();
            readerL.Dispose();
            return LanguageStrs.Count > 0;
        }
        public static string Get(string name)
        {
            if (isExist(name))
                return LanguageStrs[name];
            else return "LANG_ERROR";
        }
        public static bool isExist(string name)
        {
            return LanguageStrs.ContainsKey(name);
        }
    }
}
