using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Witcher3_Multiplayer.Game;
using static Witcher3_Multiplayer.ClientHost.DataTypes;
using static Witcher3_Multiplayer.langproc;
namespace Witcher3_Multiplayer.ClientHost
{
    public class ClientV2
    {
        public static UdpClient UDP_CLIENT;
        private static PlayerData player_data;
        private static ServerInfo ConnectedInfo;
        private static IPEndPoint ConnectedEPOINT;
        private static int MyId;
        private static string MyName;
        private static bool HorseRiding = false;
        private static string MyCharacter;
        private static int CombatGuid;
        private static CombatTarget currentCombat;
        public async static void Connect(string nick, string chara, string address, int port, bool RCON, string Password = "")
        {
            LOG("[client] Verifying game satate");
            if (!SocketManager.IsConnected())
            {
                LOG("[client] Please start game first!");
                return;
            }
            if (GameManagerUI.IsInMenu() & GameManagerUI.IsGameStoped() & GameManagerUI.IsGamePaused())
            {
                LOG("YOU_CAN'T HOST GAME IN MAIN MENU! PLEASE load a save");
                //return;
            }
            if (GameManagerUI.IsGameStoped() || GameManagerUI.IsGamePaused())
                GameManagerUI.UnpauseGame();
            if (IsHost)
                LOG("[client] You Hoster of The Game!");
            if (RCON && string.IsNullOrEmpty(Password))
            {
                LOG("[client] NEED A PASSWORD!");
                return;
            }
            LOG("Connecting to " + address + ":" + port);
            ConnectedEPOINT = new IPEndPoint(IPAddress.Parse(address), port);
            UDP_CLIENT = new UdpClient(address, port);
            RCON = RCON ? AccessShell(Password) : RCON;
            IsConnected = true;
            MyName = nick;
            MyCharacter = chara;
            GameManagerMY.ExecConsoleCommand("LaunchCustomFramework");
            ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.RET_CONNECTED);
            while (true)
            {
                if (!SocketManager.GameSocket.Connected || !IsConnected)
                {
                    LOG("Game Or connection closed");
                    break;
                }
                var f = await UDP_CLIENT.ReceiveAsync();
                OperateWithData(f.Buffer);
            }
        }
        public static void SendMyData()
        {
            var a = Task.Run(() =>
            {
                while (true)
                {
                    if (IsConnected)
                    {
                        ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_PLAYERPOSITION, GameManagerMY.GetPlayerPosition());
                        var isonhorse = GameManagerMY.GetPlayerIsOnHorse();
                        if (isonhorse != HorseRiding)
                        {
                            if (debug) LOG("Sending data ImOnHorse: " + (HorseRiding = isonhorse));
                            ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_PLAYERONHORSE, BitConverter.GetBytes(HorseRiding));
                        }
                        var myCombat = GameManagerMY.GetCombatTargetGuid();
                        if (myCombat != 0)
                        {
                            if (myCombat != CombatGuid)
                            {
                                currentCombat = new CombatTarget()
                                {
                                    Guid = CombatGuid = GameManagerMY.GetCombatTargetGuid(),
                                    Template = GameManagerMY.GetCombatTargetName()
                                };
                                if (debug) LOG("[FightSystem] Player TargetInSight Send: " + CombatGuid);
                                ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_PLAYERCOMBATTARGET, currentCombat.ToByteArray());
                            }
                            else if (GameManagerMY.GetCombatTargetIsDead())
                            {
                                if (debug)
                                    LOG("[FightSystem] Player KILL entity send: " + CombatGuid);
                                ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_PLAYERCOMBATTARGETKILL, currentCombat.ToByteArray());
                                CombatGuid = 0;
                            }
                        }
                    }
                    Thread.Sleep(SendDataDelay);
                }
            });
        }
        public static bool AccessShell(string Password)
        {
            ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.RET_ACCESS, Password);
            byte[] data = ClientSender.GetSyncData(UDP_CLIENT, ConnectedEPOINT);
            if (data == null || data.Length < 3)
                return BitConverter.ToBoolean(data, 0);
            return false;
        }
        public static void OperateWithData(byte[] data)
        {
            if (data != null & data.Length >= 8)
            {
                byte[] header = data.Take(4).ToArray();
                byte[] header4 = data.Skip(4).Take(4).ToArray();
                int IDClient = PlayerDataClient.Count - BitConverter.ToInt16(header, 0) - 1;
                int ahead = BitConverter.ToInt16(header4, 0);
                byte[] recvdata = data.Skip(8).ToArray();
                RecvSendTypes head = (RecvSendTypes)ahead;
                switch (head)
                {
                    case RecvSendTypes.RCV_PLAYERINFO:
                        PlayerData iiii = recvdata.ToStructure<PlayerData>();
                        if (!PlayerDataClient.ContainsKey(iiii.ID) & (iiii.ID != player_data.ID || TESTMYCLIENT))
                        {
                            LOG("[client] ===INFO GET===");
                            LOG("[client] NickName: " + iiii.NickName);
                            LOG("[client] Player ID: " + iiii.ID);
                            PlayerDataClient.Add(iiii.ID, iiii);
                            GameManagerMY.Spawn_Player(iiii.NickName, iiii.ID, iiii.CharacterTemplate, iiii.PlayerPosition, iiii.HorsePosition);
                        }
                        break;
                    case RecvSendTypes.RCV_COMMANDRESPONSE:
                        LOG("[client-cmd] RESPONSE: " + Encoding.ASCII.GetString(recvdata));
                        break;
                    case RecvSendTypes.RCV_HOSTINFO:
                        INITDATA(recvdata);
                        break;
                    case RecvSendTypes.RCV_CHATMSG:
                        LOG(PlayerDataClient[IDClient].NickName + ": " + Encoding.UTF8.GetString(recvdata));
                        break;
                    case RecvSendTypes.RCV_PLAYERONHORSE:
                        var aaaa = BitConverter.ToBoolean(recvdata, 0);
                        GameManagerMY.SetPlayerIsOnHorse(IDClient, aaaa ? 1 : 0);
                        break;
                    case RecvSendTypes.RCV_PLAYERPOSITION:
                        GameManagerMY.SetPlayerMoveTo(IDClient, recvdata.ToStructure<Vector3>());
                        break;
                    case RecvSendTypes.RCV_PLAYERCOMBATTARGET:
                        CombatTarget combatstructs = recvdata.ToStructure<CombatTarget>();
                        if (debug) LOG("[client] ClientID: " + IDClient + "  Recieve target: " + combatstructs.Guid + ":" + combatstructs.Template);
                        GameManagerMY.CheckCombatTargetOrSpawnIt(combatstructs.Guid, combatstructs.Template, IDClient);
                        break;
                    case RecvSendTypes.RCV_PLAYERCOMBATTARGETKILL:
                        CombatTarget combatstruct = recvdata.ToStructure<CombatTarget>();
                        if (debug) LOG("[client] Entity: " + combatstruct.Guid + ":" + combatstruct.Template + " Killed By: " + IDClient);
                        GameManagerMY.KillEntityByGuid(combatstruct.Guid, combatstruct.Template, IDClient);
                        break;
                }
            }
        }
        public static void INITDATA(byte[] recvdata)
        {
            ServerInfo ServerResp = recvdata.ToStructure<ServerInfo>();
            if(debug) LOG("[pre] Verify the data");
            if (!string.IsNullOrEmpty(ServerResp.Name))
            {
                if (ServerResp.MaxPlayers > ServerResp.CurPlayers + 1)
                {
                    LOG("Joined to server: " + ServerResp.Name);
                    if (debug) LOG("[pre] Preparing Player Data");
                    player_data = new PlayerData()
                    {
                        ID = MyId = ServerResp.CurPlayers,
                        NickName = MyName,
                        HP = GameManagerMY.GetPlayerHP(),
                        LevelID = GameManagerMY.GetPlayerAreaID(),
                        Plevel = GameManagerMY.GetPlayerLevel(),
                        PlayerPosition = GameManagerMY.GetPlayerPosition(),
                        HorsePosition = GameManagerMY.GetPlayerHorsePosition(),
                        State = GameManagerMY.GetPlayerStateFightInt(),
                        CharacterTemplate = MyCharacter
                    };
                    if (debug) LOG("[pre] Sending Player Data to host LENGTH: " + player_data.ToByteArray().Length);
                    ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_PLAYERINFO, player_data.ToByteArray());
                    ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.RET_PLAYERDATAS);
                    IsConnected = true;
                    if(!TESTMYCLIENT)PlayerDataClient.Add(player_data.ID, player_data);
                    ConnectedInfo = ServerResp;
                    SendMyData();
                }
                else
                {
                    LOG("[pre] Server Already Full");
                    Disconnect();
                }
            }
            else
            {
                LOG("[pre] Server Connection wrong ServerName EMPTY");
                Disconnect();
            }
        }
        public static void Disconnect()
        {
            ClientSender.SendData(UDP_CLIENT, (int)RecvSendTypes.SND_DISCONNECTED);
            UDP_CLIENT.Close();
            IsConnected = false;
            if (!Dedicated & IsHost)
                ServerV2.StopServer();
        }
    }
}
