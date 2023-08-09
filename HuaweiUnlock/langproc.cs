using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Witcher3_Multiplayer.ClientHost;

namespace Witcher3_Multiplayer
{
    public static class langproc
    {
        public static TextBox LOGGERB;
        public static bool IsHost = false,
            Dedicated = false,
            IsConnected = false,
            TESTMYCLIENT = false,
            debug = true;
        public static double VersionCur = 1.0;
        public static int SendDataDelay = 64;
        public static Main MForm;
        public static SimpleOverlayFORWINDOWEDMODE OverlForm;
        public static string MonstersPath = "characters\\npc_entities\\monsters\\";//+MONSTERNAME
        public static Dictionary<int, PlayerData> PlayerDataClient = new Dictionary<int, PlayerData>();
        public static Dictionary<int, PlayerData> PlayerDataServerDATAS = new Dictionary<int, PlayerData>();
        public static Dictionary<IPEndPoint, int> PlayerDataServer = new Dictionary<IPEndPoint, int>();
        public static void LOG(string s)
        {
            Action t = () => {
                LOGGERB.Text += ("[INFO] " + s + Environment.NewLine);
                LOGGERB.SelectionStart = LOGGERB.Text.Length;
                LOGGERB.ScrollToCaret(); 
            };
            if (LOGGERB.InvokeRequired)
                LOGGERB.Invoke(t);
            else
                t();
        }
        public static void ELOG(string s)
        {
            Action t = () => { LOGGERB.Text += "[ERROR] " + s + Environment.NewLine; };
            if (LOGGERB.InvokeRequired)
                LOGGERB.Invoke(t);
            else
                t();
        }
        public static byte[] ToByteArray(this object structure)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, structure);
            return ms.ToArray();
        }
        public static T ToStructure<T>(this byte[] arrBytes) where T : struct
        {
            MemoryStream memStream = new MemoryStream(arrBytes);
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
