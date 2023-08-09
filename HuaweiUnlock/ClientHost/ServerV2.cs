using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Witcher3_Multiplayer.Game;
using static Witcher3_Multiplayer.ClientHost.DataTypes;
using static Witcher3_Multiplayer.langproc;
namespace Witcher3_Multiplayer.ClientHost
{
    public class ServerV2
    {
        private static UdpClient UDP_SERVER;
        private static ServerInfo host_data;
        private static string ServerName = "EBalo";
        private static string Password = "NET";
        private static bool CommandShell = false;
        private static int MaxPlayers = 1;
        public async static void CreateServer(int port, string Name, int MaxP, bool RCON, string password = "")
        {
            if (!SocketManager.GameSocket.Connected)
            {
                LOG("[host] Please start game first!");
                return;
            }
            if (IsHost)
            {
                LOG("[host] Already Server Runned!");
                return;
            }
            if (IsConnected)
            {
                LOG("[host] Please Disconnect from server, before create new server. OR MANY BUGS!");
                return;
            }
            if (RCON && string.IsNullOrEmpty(password))
            {
                LOG("[client] NEED A PASSWORD!");
                return;
            }
            LOG("Creating Server");
            MaxPlayers = MaxP;
            CommandShell = RCON;
            Password = password;
            ServerName = Name;
            IsHost = true;
            UDP_SERVER = new UdpClient(port);
            LOG("Server Created!");
            SendPlayersData();
            while (true)
            {
                if (!SocketManager.GameSocket.Connected)
                {
                    LOG("Game closed");
                    break;
                }
                var result = await UDP_SERVER.ReceiveAsync();
                OperateWithData(result.Buffer, result.RemoteEndPoint);
            }
        }
        public static void SendPlayersData()
        {
            var a = Task.Run(() =>
            {
                while (true)
                {
                    foreach (var item in PlayerDataServer)
                    {
                        HostSender.SendDataToAllExceptOne(UDP_SERVER, item.Key, (int)RecvSendTypes.RCV_PLAYERPOSITION, PlayerDataServerDATAS[item.Value].PlayerPosition.ToByteArray());
                        HostSender.SendDataToAllExceptOne(UDP_SERVER, item.Key, (int)RecvSendTypes.RCV_PLAYERONHORSE, PlayerDataServerDATAS[item.Value].HorsePosition.ToByteArray());
                    }
                    Thread.Sleep(SendDataDelay/2);
                }  
            });
        }
        public static void OperateWithData(byte[] data, IPEndPoint fromclie)
        {
            if (data != null & data.Length >= 4)
            {
                byte[] header4 = data.Take(4).ToArray();
                int ahead = BitConverter.ToInt16(header4, 0);
                byte[] recvdata = data.Skip(4).ToArray();
                int IDClient = PlayerDataServer.ContainsKey(fromclie)? PlayerDataServer[fromclie] : -1;
                RecvSendTypes head = (RecvSendTypes)ahead;
                switch (head)
                {
                    case RecvSendTypes.RET_ACCESS:
                        HostSender.SendDataHost(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_ACCESSSHELL, BitConverter.GetBytes(CommandShell && Password == Encoding.UTF8.GetString(recvdata)));
                        break;
                    case RecvSendTypes.SND_PLAYERINFO:
                        if (debug)
                            LOG("[host] Preparing PlayerData OF new Client and adding to list");
                        PlayerData iiii = recvdata.ToStructure<PlayerData>();
                        if (!PlayerDataServer.ContainsKey(fromclie))
                        {
                            LOG("[host] ===Joined===");
                            LOG("[host] Player ID: " + iiii.ID);
                            LOG("[host] NickName: " + iiii.NickName);
                            LOG("[host] HP: " + iiii.HP);
                            LOG("[host] LevelID: " + iiii.LevelID);
                            LOG("[host] Plevel: " + iiii.Plevel);
                            PlayerDataServer.Add(fromclie, iiii.ID);
                            PlayerDataServerDATAS.Add(iiii.ID, iiii);
                            LOG("[host] CurrentPlayers: " + PlayerDataServer.Count);
                            if (debug) LOG("[host] Resend Connected Player INFO to All");
                        }
                        HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_PLAYERINFO, iiii.ToByteArray());
                        break;
                    case RecvSendTypes.SND_COMMAND:
                        if (CommandShell)
                            HostSender.SendDataHost(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_COMMANDRESPONSE, GameManagerMY.ExecConsoleCommand(Encoding.UTF8.GetString(recvdata)));
                        break;
                    case RecvSendTypes.RET_CONNECTED:
                        if (debug) LOG("[host] Preparing Server INFO For new client");
                        host_data = new ServerInfo()
                        {
                            Name = ServerName,
                            MaxPlayers = MaxPlayers,
                            CurPlayers = PlayerDataServer.Count,
                            Version = VersionCur
                        };
                        if (debug) LOG("[host] Sending Current ServerINFO to new client ");
                        HostSender.SendDataHost(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_HOSTINFO, host_data.ToByteArray());
                        break;
                    case RecvSendTypes.SND_DISCONNECTED:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            PlayerDataServerDATAS.Remove(PlayerDataServer[fromclie]);
                            PlayerDataServer.Remove(fromclie);
                            LOG("===Disconnected===");
                            LOG("PlayerID: " + IDClient);
                            LOG("PlayerName: " + PlayerDataServerDATAS[IDClient].NickName);
                            //ACTION REMOVE PLAYER FROM GAME
                        }
                        HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_DISCONNECTED);
                        break;
                    case RecvSendTypes.RET_PLAYERDATAS:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            if (debug) LOG("[host] Client ret All player Dats: ");
                            foreach (var o in PlayerDataServerDATAS)
                            {
                                if (debug) LOG("[host] Sending data of Player: " + o.Value.NickName + " : " + o.Value.ID + " To PlayerID: " + IDClient);
                                if (o.Value.ID != PlayerDataServer[fromclie])
                                    HostSender.SendDataHost(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_PLAYERINFO, o.Value.ToByteArray());
                            }
                        }
                        break;
                    case RecvSendTypes.SND_PLAYERPOSITION:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            var a = PlayerDataServerDATAS[PlayerDataServer[fromclie]];
                            a.PlayerPosition = recvdata.ToStructure<Vector3>();
                            PlayerDataServerDATAS[PlayerDataServer[fromclie]] = a;
                        }
                        break;
                    case RecvSendTypes.SND_PLAYERHORSEPOSITION:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            var aa = PlayerDataServerDATAS[PlayerDataServer[fromclie]];
                            aa.HorsePosition = recvdata.ToStructure<Vector3>();
                            PlayerDataServerDATAS[PlayerDataServer[fromclie]] = aa;
                        }
                        break;
                    case RecvSendTypes.SND_PLAYERONHORSE:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            if (debug) LOG("[host] Resend Horse State of playerID" + IDClient);
                            HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_PLAYERONHORSE, recvdata);
                        }
                        break;
                    case RecvSendTypes.SND_PLAYERCOMBATTARGET:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_PLAYERCOMBATTARGET, recvdata);
                        }
                        break;
                    case RecvSendTypes.SND_PLAYERCOMBATTARGETKILL:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            if (debug) LOG("[host] PlayerID " + IDClient + " kill the entity");
                            HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_PLAYERCOMBATTARGETKILL, recvdata);
                        }
                        break;
                    case RecvSendTypes.SND_CHATMSG:
                        if (PlayerDataServer.ContainsKey(fromclie))
                        {
                            LOG("Recieve PlayerID " + IDClient + " MSG:" + Encoding.UTF8.GetString(recvdata));
                            HostSender.SendDataToAllExceptOne(UDP_SERVER, fromclie, (int)RecvSendTypes.RCV_CHATMSG, recvdata);
                        }
                        break;
                }
            }
        }
        public static void StopServer()
        {
            UDP_SERVER.Close();
            IsHost = false;
        }
    }
}
