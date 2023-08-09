using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Witcher3_Multiplayer.ClientHost;
using Witcher3_Multiplayer.Game;
using static Witcher3_Multiplayer.ClientHost.DataTypes;
using static Witcher3_Multiplayer.langproc;
namespace Witcher3_Multiplayer
{
    public partial class SimpleOverlayFORWINDOWEDMODE : Form
    {
        private bool enaMY;
        public SimpleOverlayFORWINDOWEDMODE()
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            Text = "";
            InitializeComponent();
            this.KeyPreview = true;
            this.Opacity = 0.7f;
            this.TransparencyKey = Color.Magenta;
            this.BackColor = Color.Magenta;
            this.StartPosition = FormStartPosition.Manual;
            this.Left = 0;
            this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height;
            SHOW();
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public void SHOW()
        {
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x00);
        }
        public void HIDE()
        {
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x10);
        }
        private async void ConnectBTN_Click(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                TESTMYCLIENT = DebugTestClient.Checked;
                int port = int.Parse(Regex.Replace(ServerPORTTEXT.Text, "[^0-9]", ""));
                string charact = MForm.CharacterSelectorTEXT.Text;
                var a = Task.Run(() =>
                {
                    if (!SocketManager.IsConnected())
                    {
                        if (SocketManager.ConnectToGame())
                        {
                            var c = Task.Run(() =>
                            {
                                ClientV2.Connect(MForm.NickNameTEXT.Text, charact, ServerIPCONTEXT.Text, port, false);
                            });
                        }
                    }
                    else
                    {
                        var c = Task.Run(() =>
                        {
                            ClientV2.Connect(MForm.NickNameTEXT.Text, charact, ServerIPCONTEXT.Text, port, false);
                        });
                    }
                });
                await a;
            }
        }
        private async void HostBtn_Click(object sender, EventArgs e)
        {
            debug = DebugCheck.Checked;
            Dedicated = DedicatedFlag.Checked;
            TESTMYCLIENT = DebugTestClient.Checked;
            int serverport = int.Parse(Regex.Replace(ServerPORTTEXT.Text, "[^0-9]", ""));
            int serverMAXP = int.Parse(Regex.Replace(MaxPlTEXT.Text, "[^0-9]", ""));
            if (!IsHost)
            {
                var player = MForm.CharacterSelectorTEXT.Text;
                var a = Task.Run(() =>
                {
                    if (!SocketManager.IsConnected())
                    {
                        if (SocketManager.ConnectToGame())
                        {
                            var b = Task.Run(() =>
                            {
                                ServerV2.CreateServer(serverport, ServerNAMETEXT.Text, serverMAXP, ConsoleRCON.Checked, PasswordServ.Text);
                            });
                            Thread.Sleep(200);
                            var c = Task.Run(() =>
                            {
                                ClientV2.Connect(MForm.NickNameTEXT.Text, player, "127.0.0.1", serverport, ConsoleRCON.Checked);
                            });
                        }
                    }
                    else
                    {
                        var b = Task.Run(() =>
                        {
                            ServerV2.CreateServer(serverport, ServerNAMETEXT.Text, serverMAXP, ConsoleRCON.Checked, PasswordServ.Text);
                        });
                        Thread.Sleep(200);
                        var c = Task.Run(() =>
                        {
                            ClientV2.Connect(MForm.NickNameTEXT.Text, player, "127.0.0.1", serverport, ConsoleRCON.Checked);
                        });
                    }
                });
                await a;
            }
        }
        private void SimpleOverlay_KeyDown(object sender, KeyEventArgs e)
        {
            var press = e.KeyCode;
            if (press == Keys.NumPad5)
            {
                enaMY = !enaMY;
                if (enaMY)
                    SHOW();
                else
                    HIDE();
            }
            if (press == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(ChatCMDBOX.Text) & TCONTROL.SelectedIndex == 0)
                {
                    if (ChatCMDBOX.Text.StartsWith("/msg "))
                    {
                        if (ChatCMDBOX.Text.Split(' ').Length > 1 & IsConnected)
                        {
                            LOG(MForm.NickNameTEXT.Text + ": " + ChatCMDBOX.Text.Replace("/msg ", ""));
                            ClientSender.SendData(ClientV2.UDP_CLIENT, (int)RecvSendTypes.SND_CHATMSG, Encoding.UTF8.GetBytes(ChatCMDBOX.Text.Replace("/msg ", "")));
                        }
                    }
                    else
                    {
                        if (!SocketManager.IsConnected())
                        {
                            if (SocketManager.ConnectToGame())
                                LOG(GameManagerMY.ExecConsoleCommand(ChatCMDBOX.Text.Trim()));
                        }
                        else
                            LOG(GameManagerMY.ExecConsoleCommand(ChatCMDBOX.Text.Trim()));
                    }
                    ChatCMDBOX.Text = "";
                }
            }
        }
    }
}