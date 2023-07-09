using System;
using System.IO;
using System.Reflection;

namespace HuaweiUnlocker
{
    public class ResourcesMNG
    {
        public static byte[] GetResource(string ResourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
        public static Stream GetResourceStream(string ResourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
        }
        public static void SaveResources(string ResourceName, string SavePath, string SaveName = "")
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
            string myspace = typeof(ResourcesMNG).Namespace;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(myspace + "." + ResourceName);
            FileStream filewritter = new FileStream(SavePath + "\\" + (string.IsNullOrEmpty(SaveName) ? ResourceName : SaveName), FileMode.CreateNew);
            for (int i = 0; i < stream.Length; i++) filewritter.WriteByte((byte)stream.ReadByte());
            filewritter.Close();
            filewritter.Dispose();
        }
    }
}
