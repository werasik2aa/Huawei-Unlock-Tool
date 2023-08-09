using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Witcher3_Multiplayer.Game
{
    public class SocketManager
    {
        public static Socket GameSocket;
        public static ManualResetEvent ConnectDone = new ManualResetEvent(false);
        public static ManualResetEvent SendDone = new ManualResetEvent(false);
        public static ManualResetEvent ReceiveDone = new ManualResetEvent(false);
        public static Response.Data Response;
        public static IPEndPoint DebugProtoclAdress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 37001);
        public static bool ConnectToGame()
        {
            GameSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            if (!GameSocket.Connected)
            {
                GameSocket.BeginConnect(DebugProtoclAdress, ConnectCallback, GameSocket);
                ConnectDone.WaitOne();
            }
            return GameSocket.Connected;
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Convertors.Init().ForEach(x => Send(GameSocket, x));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            ConnectDone.Set();
        }
        public static void Send(Socket client, byte[] data)
        {
            if (!GameSocket.Connected) return;
            client.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;
                var bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            SendDone.Set();
        }
        public static string ReciveData_DoWork()
        {
            byte[] dataIn = new byte[8192 * 32];
            string returnns = "";
            if (GameSocket.Connected)
            {
                try
                {
                    int bytesRead = GameSocket.Receive(dataIn, dataIn.Length, SocketFlags.None);
                    if (bytesRead != -1)
                    {
                        Response = new Response.Data(dataIn);
                        returnns = Response.Params.Last().ToString();
                    }
                    else
                        langproc.LOG("Connection to game CLOSED:");
                }
                catch
                {
                    langproc.LOG("ERROR: Socket or Game closed?");
                }
            }
            return returnns;
        }
        public static bool IsConnected()
        {
            return GameSocket != null && GameSocket.Connected;
        }
    }
}
