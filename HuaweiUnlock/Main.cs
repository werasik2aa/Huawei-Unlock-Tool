using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Witcher3_Multiplayer.Game;
using static Witcher3_Multiplayer.langproc;
using Witcher3_Multiplayer.ClientHost;
using System.Threading;
using System.Text.RegularExpressions;
using static Witcher3_Multiplayer.ClientHost.DataTypes;

namespace Witcher3_Multiplayer
{
    public partial class Main : Form
    {
        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (IsConnected)
                ClientSender.SendData(ClientV2.UDP_CLIENT, (int)RecvSendTypes.SND_DISCONNECTED);
        }
        public Main()
        {
            InitializeComponent();
            LOGGERB = LogBoxman;
            MForm = this;
            OverlForm = new SimpleOverlayFORWINDOWEDMODE();
            foreach(var a in DataTypes.NpcsPlayer)
                CharacterSelectorTEXT.Items.Add(a);
            CharacterSelectorTEXT.SelectedIndex = 1;
        }

        private async void RunGame_Click(object sender, EventArgs e)
        {
            LogBoxman.Text = "";
            MForm.Hide();
            var a = Task.Run(() =>
            {
                //ServerV2.CreateServer(33299, "COCKFUCK", false);
            });
            await a;
            OverlForm.Show();
            LOGGERB = OverlForm.ChatOut;
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                TESTMYCLIENT = DebugTestClient.Checked;
                int port = int.Parse(Regex.Replace(ServerCONPORTtext.Text, "[^0-9]", ""));
                string charact = CharacterSelectorTEXT.Text;
                var a = Task.Run(() =>
                {
                    if (!SocketManager.IsConnected())
                    {
                        if (SocketManager.ConnectToGame())
                        {
                            var c = Task.Run(() =>
                            {
                                ClientV2.Connect(NickNameTEXT.Text, charact, ServerIPCONTEXT.Text, port, false);
                            });
                        }
                    }
                    else
                    {
                        var c = Task.Run(() =>
                        {
                            ClientV2.Connect(NickNameTEXT.Text, charact, ServerIPCONTEXT.Text, port, false);
                        });
                    }
                });
                await a;
            }
        }

        private async void HostBTN_Click(object sender, EventArgs e)
        {
            debug = DebugCheck.Checked;
            Dedicated = DedicatedFlag.Checked;
            TESTMYCLIENT = DebugTestClient.Checked;
            int serverport = int.Parse(Regex.Replace(ServerPORTTEXT.Text, "[^0-9]", ""));
            int serverMAXP = int.Parse(Regex.Replace(MaxPlTEXT.Text, "[^0-9]", ""));
            if (!IsHost)
            {
                var player = CharacterSelectorTEXT.Text;
                var a = Task.Run(() =>
                {
                    if (!SocketManager.IsConnected())
                    {
                        if (SocketManager.ConnectToGame())
                        {
                            var b = Task.Run(() =>
                            {
                                ServerV2.CreateServer(serverport, ServerNAMETEXT.Text, serverMAXP, ConsoleRCON.Checked, ConsolePASSWDText.Text);
                            });
                            Thread.Sleep(200);
                            var c = Task.Run(() =>
                            {
                                ClientV2.Connect(NickNameTEXT.Text, player, "127.0.0.1", serverport, ConsoleRCON.Checked);
                            });
                        }
                    }
                    else
                    {
                        var b = Task.Run(() =>
                        {
                            ServerV2.CreateServer(serverport, ServerNAMETEXT.Text, serverMAXP, ConsoleRCON.Checked, ConsolePASSWDText.Text);
                        });
                        Thread.Sleep(200);
                        var c = Task.Run(() =>
                        {
                            ClientV2.Connect(NickNameTEXT.Text, player, "127.0.0.1", serverport, ConsoleRCON.Checked);
                        });
                    }
                });
                await a;
            }
        }

        private async void CmdBTN_Click(object sender, EventArgs e)
        {
            var a = Task.Run(() =>
            {
                if (!SocketManager.IsConnected())
                {
                    if (SocketManager.ConnectToGame())
                        LOG(GameManagerMY.ExecConsoleCommand(CMDb.Text));
                }
                else
                    LOG(GameManagerMY.ExecConsoleCommand(CMDb.Text));
            });
            await a;
        }

        private void SettingsBTN_Click(object sender, EventArgs e)
        {
            SettingsLBOX.Visible = !SettingsLBOX.Visible;
        }
    }
}
