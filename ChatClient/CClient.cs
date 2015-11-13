using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ChatServer;
using Common;

namespace ChatClient
{
    public class CClient
    {
        public bool IsConnected { get; private set; }
        public event Action<IMessage> DataMessageArrived;

        SocketContainer socketContainer;
        public event Action Connected;
        public CClient()
        {
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socketContainer = new SocketContainer
            {
                Socket = clientSocket
            };
            clientSocket.BeginConnect("localhost", 4040, ConnectCallback, null);
            socketContainer.MessageArrived += MessageArrived;
        }

        private void MessageArrived(byte[] bytes)
        {
            var msg = fastJSON.JSON.ToObject<IMessage>(Encoding.UTF8.GetString(bytes));
            if (msg != null)
            {
                OnDataMessageArrived(msg);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            socketContainer.Socket.EndConnect(ar);
            Console.WriteLine(@"Connected to server");
            IsConnected = true;
            OnConnected();
            BeginReceive(socketContainer);
        }

        private void BeginReceive(SocketContainer sc)
        {
            sc.ResetBuffer();
            sc.Socket.BeginReceive(sc.RawBuffer, 0, SocketContainer.RawBufferSize, SocketFlags.None, EndReceive, sc);
        }

        private void EndReceive(IAsyncResult ar)
        {
            var sc = (SocketContainer)ar.AsyncState;
            try
            {
                var bytesReceived = sc.Socket.EndReceive(ar);
                if (bytesReceived == 0)
                {
                    Console.WriteLine("Client disconnected");
                }
                else
                {
                    var data = sc.RawBuffer.Take(bytesReceived).ToArray();
                    sc.DataReceived(data);
                    BeginReceive(sc);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Client disconnected");
            }
        }

        protected virtual void OnConnected()
        {
            var handler = Connected;
            if (handler != null) handler();
        }

        private void SendMessage(byte[] msgBytes)
        {
            var data = socketContainer.WrapMessage(msgBytes);
            socketContainer.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
        }

        public void SendMessage(IMessage msg)
        {
            var msgJson = fastJSON.JSON.ToJSON(msg);
            SendMessage(Encoding.UTF8.GetBytes(msgJson));
        }

        private void SendCallback(IAsyncResult ar)
        {
            socketContainer.Socket.EndSend(ar);
        }

        protected virtual void OnDataMessageArrived(IMessage obj)
        {
            var handler = DataMessageArrived;
            if (handler != null) handler(obj);
        }

        public void Dispose()
        {
            try
            {
                socketContainer.Socket.Close();
                socketContainer.Socket = null;
            }
            catch
            {
                //ignored
            }
        }
    }
}