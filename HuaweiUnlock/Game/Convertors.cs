using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Witcher3_Multiplayer.Game
{
    public static class Convertors
    {
        public static string NsScriptDebugger = "ScriptDebugger";
        public static string NsScriptProfiler = "ScriptProfiler";
        public static string NsScriptCompiler = "ScriptCompiler";
        public static string NsScripts = "scripts";
        public static string NsRemote = "Remote";
        public static string NsUtility = "Utility";
        public static string NsConfig = "Config";

        public static string CmdBind = "BIND";

        public static byte[] PacketHead = { 0xDE, 0xAD }; // DEAD
        public static byte[] PacketTail = { 0xBE, 0xEF }; // BEEF

        public static string ScRootPath = "RootPath";

        public static string SdUnfilteredLocals = "UnfilteredLocals";
        public static string SdSortLocals = "SortLocals";
        public static string SdOpcodeRequest = "OpcodeBreakdownRequest";

        public static string SReload = "reload";
        public static string SPkgSync = "pkgSync";

        public static string CfgList = "list";

        public static byte[] TypeByte = { 0x81, 0x08 };
        public static byte[] TypeStringUtf8 = { 0xAC, 0x08 };
        public static byte[] TypeStringUtf16 = { 0x9C, 0x16 };
        public static byte[] TypeInt16 = { 0x81, 0x16 };
        public static byte[] TypeInt32 = { 0x81, 0x32 };
        public static byte[] TypeInt64 = { 0x81, 0x64 };
        public static byte[] TypeUint32 = { 0x71, 0x32 };
        public static List<byte[]> Init()
        {
            return new List<byte[]>()
            {
                Bind(NsScriptCompiler),
                Bind(NsScriptDebugger),
                Bind(NsScriptProfiler),
                Bind(NsScripts),
                Bind(NsUtility),
                Bind(NsRemote),
                Bind(NsConfig)
            };
        }

        /// <summary>
        /// Tells the game to send you any events in the specified namespace 
        /// </summary>
        /// <param name="nspace">Namespace to listen to</param>
        /// <returns></returns>
        public static byte[] Bind(string nspace)
        {
            return Initzero()
                    .AppendUtf8(CmdBind)
                    .AppendUtf8(nspace)
                    .End();
        }

        /// <summary>
        /// Reload game scripts
        /// </summary>
        /// <returns></returns>
        public static byte[] Reload()
        {
            return Initzero()
                    .AppendUtf8(NsScripts)
                    .AppendUtf8(SReload)
                    .End();
        }

        /// <summary>
        /// Get the root path for scripts
        /// </summary>
        /// <returns></returns>
        public static byte[] RootPath()
        {
            return Initzero()
                    .AppendUtf8(NsScriptCompiler)
                    .AppendUtf8(ScRootPath)
                    .End();
        }

        /// <summary>
        /// Searches for config variables 
        /// </summary>
        /// <param name="section">Section to search. If left empty, searches all sections</param>
        /// <param name="name">Only the variables containing this token will be returned. Leave empty for all variables</param>
        /// <returns></returns>
        public static byte[] Varlist(string section = "", string name = "")
        {
            return Initzero()
                    .AppendUtf8(NsConfig)
                    .AppendInt32(unchecked((Int32)0xCC00CC00))
                    .AppendUtf8(CfgList)
                    .AppendUtf8(section)
                    .AppendUtf8(name)
                    .End();
        }

        /// <summary>
        /// Runs an exec function from the game
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns></returns>
        public static byte[] Execute(string command)
        {
            //if(langproc.debug)
               // langproc.LOG("exec: " + command);
            return Initzero()
                    .AppendUtf8(NsRemote)
                    .AppendInt32(unchecked((Int32)0x12345678))
                    .AppendInt32(unchecked((Int32)0x81160008))
                    .AppendUtf8(command)
                    .End();
        }

        /// <summary>
        /// Gets the list of mods installed in game directory
        /// </summary>
        /// <returns></returns>
        public static byte[] Modlis()
        {
            return Initzero().AppendUtf8(NsScripts).AppendUtf8(SPkgSync).End();
        }

        public static byte[] SetVar(string section, string name, string value)
        {
            return Initzero()
                        .AppendUtf8("Config")
                        .AppendInt32(BitConverter.ToInt32(new[] { (byte)0xCC, (byte)0x00, (byte)0xCC, (byte)0x00 }, 0))
                        .AppendUtf8("set")
                        .AppendUtf8(section)
                        .AppendUtf8(name)
                        .AppendUtf16(value)
                        .End();
        }
        /// <summary>
        /// Gets the 0 bytes
        /// </summary>
        /// <returns></returns>
        public static byte[] Initzero()
        {
            return new byte[0];
        }
        /// <summary>
        /// Append
        /// </summary>
        /// <returns></returns>
        public static byte[] Append(this byte[] payload, byte[] data)
        {
            return payload.Concat(data).ToArray();
        }
        /// <summary>
        /// Append byte
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendByte(this byte[] payload, byte Value)
        {
            return payload.Append(TypeByte).Append(new byte[] { Value });
        }

        /// <summary>
        /// Append UTF8
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendUtf8(this byte[] payload, string text)
        {
            return payload.Append(TypeStringUtf8).AppendInt16((Int16)(text.Length)).Append(Encoding.UTF8.GetBytes(text));
        }
        /// <summary>
        /// Append UTF16
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendUtf16(this byte[] payload, string text)
        {
            return payload.Append(TypeStringUtf16).AppendInt16((Int16)(text.Length * 2)).Append(Encoding.BigEndianUnicode.GetBytes(text));
        }

        /// <summary>
        /// Append UTF64
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendInt64(this byte[] payload, Int64 value)
        {
            return payload.Append(TypeInt64).Append(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        /// <summary>
        /// Append INT64
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendInt16(this byte[] payload, Int16 value)
        {
            return payload.Append(TypeInt16).Append(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        /// <summary>
        /// Append INT16
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendUInt32(this byte[] payload, UInt32 value)
        {
            return payload.Append(TypeUint32).Append(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        /// <summary>
        /// Append INT32
        /// </summary>
        /// <returns></returns>
        public static byte[] AppendInt32(this byte[] payload, Int32 value)
        {
            return payload.Append(TypeInt32).Append(BitConverter.GetBytes(value).Reverse().ToArray());
        }
        /// <summary>
        /// Combine HEADER==<YOURCMD>===TAIL
        /// </summary>
        /// <returns></returns>
        public static byte[] End(this byte[] payload)
        {
            return PacketHead.Concat(BitConverter.GetBytes((short)(payload.Length + 6)).Reverse().Concat(payload)).Concat(PacketTail).ToArray();
        }
    }
}
