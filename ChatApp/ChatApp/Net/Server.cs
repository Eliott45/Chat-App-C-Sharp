using ChatClient.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    internal class Server
    {
        public PacketReader PacketReader;
        public Action ConnectedEvent;
        public Action MessageReceivedEvent;
        public Action UserDisconnectEvent;

        private readonly TcpClient _client;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (_client.Connected) return;
            _client.Connect("127.0.0.1", 7891);
            PacketReader = new PacketReader(_client.GetStream());

            if(!string.IsNullOrEmpty(username))
            {
                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(username);
                _client.Client.Send(connectPacket.GetPacketBytes());
            }

            ReadPackets();
        }

        public void SendMessageToServer(string message)
        {
            var messagePaket = new PacketBuilder();
            messagePaket.WriteOpCode(5);
            messagePaket.WriteMessage(message);
            _client.Client.Send(messagePaket.GetPacketBytes());
        }

        private void ReadPackets()
        {
            Task.Run(() => {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;
                        case 5:
                            MessageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Something went wrong...");
                            break;
                    }
                }
            });
        }

    }
}
