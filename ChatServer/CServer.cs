using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ChatServer
{
    class CServer
    {
        class Client
        {
            public long SessionId { get; set; }
            public string Username { get; set; }
            public SocketContainer SocketContainer { get; set; }
        }

        readonly List<SocketContainer> _clients = new List<SocketContainer>();
        readonly Dictionary<long, Client> _acceptedClients = new Dictionary<long, Client>();
        readonly Socket _listenerSocket;
        private const int ServerVersion = 1;

        public CServer()
        {
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, 4040));
            _listenerSocket.Listen(1024);
            BeginAccept();
        }

        private void BeginAccept()
        {
            _listenerSocket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var client = _listenerSocket.EndAccept(ar);
            BeginReceive(client);
            BeginAccept();
        }

        private void BeginReceive(Socket client)
        {
            var sc = new SocketContainer
            {
                Socket = client,
                RawBuffer = new byte[SocketContainer.RawBufferSize],
            };
            sc.MessageArrived = delegate (byte[] bytes)
            {
                Console.WriteLine("Got {0} bytes.", bytes.Length);
                ThreadPool.QueueUserWorkItem(delegate (object state)
                {
                    var stateObj = (dynamic)state;
                    HandleReceivedMessage(stateObj.sc, stateObj.bytes, stateObj.bytes.Length);
                }, new { bytes, sc });
            };
            _clients.Add(sc);
            Console.WriteLine("New Client Connected, now {0} clients are connected to server", _clients.Count);
            client.BeginReceive(sc.RawBuffer, 0, SocketContainer.RawBufferSize, SocketFlags.None, EndReceive, sc);
        }

        private void HandleReceivedMessage(SocketContainer sc, byte[] bytes, int length)
        {
            var msg = fastJSON.JSON.ToObject<IMessage>(Encoding.UTF8.GetString(bytes));
            switch (msg.MessageType)
            {
                case MessageType.RequestSession:
                    if (_acceptedClients.Any(t => t.Value.SocketContainer == sc))
                    {
                        SendMessage(sc, new Error { ErrorMessage = "Currently Accepted" });
                        CloseCliet(sc);
                    }
                    else
                    {
                        var aMsg = (RequestSession)msg;

                        if (aMsg.Version != ServerVersion)
                        {
                            SendMessage(sc, new RejectSession { Reason = "Invalid Client Version , Please Update your client!" });
                            CloseCliet(sc);
                            break;
                        }
                        if (FakeRepository.Instance.CheckUsernamePassword(aMsg.Username, aMsg.Password))
                        {
                            var sessionId = DateTime.Now.Ticks;
                            _acceptedClients[sessionId] = new Client { SessionId = sessionId, Username = aMsg.Username, SocketContainer = sc };
                            SendMessage(sc, new AcceptSession
                            {
                                SessionId = sessionId,
                                WellcomeMessage = string.Format("{0} , Wellcome.", aMsg.Username)
                            });
                        }
                        else
                        {
                            SendMessage(sc, new RejectSession { Reason = "Invalid Username or password" });
                        }
                    }
                    break;
                case MessageType.Broadcast:
                    var bMsg = (Broadcast)msg;
                    Client client;
                    if (_acceptedClients.TryGetValue(bMsg.SessionId,out client))
                    {
                        ThreadPool.QueueUserWorkItem(delegate (object state)
                        {
                            var msgStr = (string)state;
                            Parallel.ForEach(_acceptedClients, delegate (KeyValuePair<long, Client> aClient)
                            {
                                SendMessage(aClient.Value.SocketContainer, new Broadcast
                                {
                                    Message = client.Username + " :" +  msgStr
                                });
                            });
                        }, bMsg.Message);
                    }
                    else
                    {
                        SendMessage(sc, new Error { ErrorMessage = "RequesSession" });
                        CloseCliet(sc);
                    }
                    break;
                default:
                    SendMessage(sc, new Error { ErrorMessage = "Invalid Message Type" });
                    CloseCliet(sc);
                    break;
            }
        }

        private void CloseCliet(SocketContainer sc)
        {
            try
            {
                sc.Socket.Shutdown(SocketShutdown.Both);
                sc.Socket.Close();
            }
            catch
            {
                //ignore
            }
        }

        private void SendMessage(SocketContainer sc, IMessage msg)
        {
            SendMessage(sc, Encoding.UTF8.GetBytes(fastJSON.JSON.ToJSON(msg)));
        }

        private void SendMessage(SocketContainer sc, byte[] msgBytes)
        {
            var data = sc.WrapMessage(msgBytes);
            sc.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, sc);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var sc = (SocketContainer)ar.AsyncState;
            sc.Socket.EndSend(ar);
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
                _clients.Remove(sc);
                Console.WriteLine("Client disconnected , {0} Clients connected", _clients.Count);
            }
        }
    }
}