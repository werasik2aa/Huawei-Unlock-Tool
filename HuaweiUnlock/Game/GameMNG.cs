using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Witcher3_Multiplayer.ClientHost;
using static Witcher3_Multiplayer.ClientHost.DataTypes;
using static Witcher3_Multiplayer.langproc;
namespace Witcher3_Multiplayer.Game
{
    public static class GameManagerUI
    {
        public static bool CheckData(string a)
        {
            a = a.ToLower().Trim();
            if (a.Contains("with error"))
            {
                if (IsHost) ServerV2.StopServer();
                if (IsConnected) ClientV2.Disconnect();
                IsHost = IsConnected = false;
                LOG("Version of this API whic has this mod outdated! And command return null! Please Update Client TO Match ModVersion");
            }
            if (string.IsNullOrEmpty(a) || !a.Contains("witcher-3mp")) return true;
            return false;
        }
        public static string SendChatNotify(string msg)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("NotifyChatMSG(" + msg + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string UnpauseGame()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("UnpauseGame"));
            return SocketManager.ReciveData_DoWork();
        }
        public static bool IsInMenu()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("IsInMenu"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata) || (!adata.ToLower().Contains("true") & !adata.ToLower().Contains("false")))
                return IsInMenu();
            return bool.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static bool IsGamePaused()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("IsGamePaused"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata) || (!adata.ToLower().Contains("true") & !adata.ToLower().Contains("false"))) return IsGamePaused();
            return bool.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static bool IsGameStoped()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("IsGameStoped"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata) || (!adata.ToLower().Contains("true") & !adata.ToLower().Contains("false")))
                return IsGameStoped();

            return bool.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static string SetDebugState(int i)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("SetHostDebugMode("+i+")"));
            string a = SocketManager.ReciveData_DoWork();
            return a;
        }
    }
    public static class GameManagerMY
    {
        public static bool CheckData(string a)
        {
            a = a.ToLower().Trim();
            if (a.Contains("with error"))
            {
                if (IsHost) ServerV2.StopServer();
                if (IsConnected) ClientV2.Disconnect();
                IsHost = IsConnected = false;
                LOG("Version of this API whic has this mod outdated! And command return null! Please Update Client TO Match ModVersion");
            }
            if (string.IsNullOrEmpty(a) || !a.Contains("witcher-3mp")) return true;
            return false;
        }
        public static string ExecConsoleCommand(string cmd)
        {
            if (debug) LOG("Executing: " + cmd);
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute(cmd));
            return SocketManager.ReciveData_DoWork();
        }
        public static string SetCurrentPlayers(int icount)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("SetCurPlayers(" + icount + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string KillEntityByGuid(int Guid, string Template, int idclien)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("KillActorByGuidHash(" + Guid + ", " + Template + ", " + idclien + ")"));
            string adata = SocketManager.ReciveData_DoWork();
            return adata;
        }
        public static string CheckCombatTargetOrSpawnIt(int Guid, string Template, int idclien)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("CheckActorByGuidHash(" + Guid + ", " + Template + ", " + idclien + ")"));
            string adata = SocketManager.ReciveData_DoWork();
            return adata;
        }
        public static int GetCombatTargetGuid()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCombatTargetGuid"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetCombatTargetGuid();
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static string GetCombatTargetName()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCombatTargetName"));
            string adata = SocketManager.ReciveData_DoWork();
            return adata;
        }
        public static bool GetCombatTargetIsDead()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("IsCombatTargetDead"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata) || (!adata.ToLower().Contains("true") & !adata.ToLower().Contains("false"))) return GetCombatTargetIsDead();
            return bool.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static int GetPlayerHP()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentHP"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerHP();
            return int.Parse(adata.Replace("WITCHER-3MP ", "").Trim().Split('.')[0]);
        }
        public static int GetPlayerLevel()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentPLevel"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerLevel();
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static int GetPlayerAreaID()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentLevelId"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerLevel();
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static int GetPlayerStateFightInt()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetStateAnimInt"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerStateFightInt();
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static int GetPlayerStateFightName()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetStateAnimName"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerStateFightInt();
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static string GetPlayerStateName()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetStateAnimName"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetPlayerStateName();
            return adata;
        }
        public static Vector3 GetPlayerPosition()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentPosition"));
            string raw = SocketManager.ReciveData_DoWork();
            if (CheckData(raw) || !raw.Contains(".") || !raw.Contains("playerAP")) GetPlayerPosition();
            string[] splitted = raw.Replace("WITCHER-3MP ", "").Replace("playerAP", "").Trim().Replace('.', ',').Split(' ');
            Vector3 f = new Vector3()
            {
                x = float.Parse(splitted[0]),
                y = float.Parse(splitted[1]),
                z = float.Parse(splitted[2])
            };
            return f;
        }
        public static Quaternion GetCurrentRotation()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentRotation"));
            string raw = SocketManager.ReciveData_DoWork();
            if (CheckData(raw) || !raw.Contains(".")) GetCurrentRotation();
            string[] splitted = raw.Replace("WITCHER-3MP ", "").Trim().Replace('.', ',').Split(' ');
            Quaternion f = new Quaternion()
            {
                x = float.Parse(splitted[0]),
                y = float.Parse(splitted[1]),
                z = float.Parse(splitted[2])
            };
            return f;
        }
        public static Vector3 GetPlayerHorsePosition()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentHorsePosition"));
            string raw = SocketManager.ReciveData_DoWork();
            if (CheckData(raw) || !raw.Contains(".") || !raw.Contains("horseAP")) GetPlayerPosition();
            string[] splitted = raw.Replace("WITCHER-3MP ", "").Replace("horseAP", "").Trim().Replace('.', ',').Split(' ');
            Vector3 f = new Vector3()
            {
                x = float.Parse(splitted[0]),
                y = float.Parse(splitted[1]),
                z = float.Parse(splitted[2])
            };
            return f;
        }
        public static Quaternion GetPlayerRotationWorld()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentRotation"));
            string raw = SocketManager.ReciveData_DoWork();
            if (CheckData(raw) || !raw.Contains(".")) return GetPlayerRotationWorld();
            string[] splitted = raw.Replace("WITCHER-3MP ", "").Replace('.', ',').Split(' ');
            Quaternion f = new Quaternion()
            {
                x = float.Parse(splitted[0]),
                y = float.Parse(splitted[1]),
                z = float.Parse(splitted[2])
            };
            return f;
        }
        public static bool GetPlayerIsOnHorse()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("IsOnHorse"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata) || (!adata.ToLower().Contains("true") & !adata.ToLower().Contains("false"))) return GetPlayerIsOnHorse();
            return bool.Parse(adata.Replace("WITCHER-3MP ", "").Trim());
        }
        public static string SetPlayerIsOnHorse(int client, int state)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("MountHorse(" + client + ", " + state +")"));
            string adata = SocketManager.ReciveData_DoWork();
            return adata;
        }
        public static int GetCurrentPlayers()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("GetCurrentPlayers"));
            string adata = SocketManager.ReciveData_DoWork();
            if (CheckData(adata)) return GetCurrentPlayers();
            LOG("WTF: " + adata);
            return int.Parse(adata.Replace("WITCHER-3MP ", ""));
        }
        public static string TeleportPlayer(int id, Vector3 pos)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("Teleport_Player(" + id + ", " + pos.ToString() + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string SetPlayerMoveTo(int id, Vector3 pos)
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("SetMoveTo_Player(" + id + ", " + pos.ToString() + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string Spawn_Player(string nick, int clientid, string data, Vector3 posp, Vector3 pos2h)
        {
            string donestr = nick + ", " + clientid + ", \"" + data + "\", " + posp.ToString() + ", " + pos2h.ToString();
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("Spawn_Player(" + donestr + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string Spawn_CORPSE(string data, Vector3 pos)
        {
            string donestr = data + "\", " + pos.ToString();
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("Spawn_CORPSE(" + donestr + ")"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string GetPlayerNameFromGame()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("headname"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string GetTime()
        {
            SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("telltime"));
            return SocketManager.ReciveData_DoWork();
        }
        public static string SetWeather(int state)
        {
            WeatherState id = (WeatherState)state;
            switch (id)
            {
                case WeatherState.STORM:
                    SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("makeitrain"));
                    break;
                case WeatherState.RAINI:
                    SocketManager.Send(SocketManager.GameSocket, Convertors.Execute("stoprain"));
                    break;
            }
            return SocketManager.ReciveData_DoWork();
        }
        public static void RunGame(string path)
        {
            path += "\\bin\\x64\\witcher3.exe";
            Process p = new Process();
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "-net -debugscripts";
            p.Start();
        }
    }
}
