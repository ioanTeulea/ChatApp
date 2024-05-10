using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ChatServer
{
    class Program
    {
        static List<Client> _clients;
       static  TcpListener _listener;
        static void Main(string[] args)
        {
            _clients = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            while (true)
            {
                var client =new Client( _listener.AcceptTcpClient());
                _clients.Add(client);

                BroadcastConnection(); 
            }
           
        }
        static void BroadcastConnection()
        {
            foreach(var client in _clients) 
            {
                foreach(var cln in _clients)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteString(cln.Username);
                    broadcastPacket.WriteString(cln.UID.ToString());
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        public static void Broadcastmessage(string message)
        {
            foreach (var client in _clients)
            {
                var msgPacket=new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteString(message);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public static void BroadcastDissconnect(string uid)
        {
            var dissconnectedUser = _clients.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _clients.Remove(dissconnectedUser);
           
            foreach (var client in _clients)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteString(uid);
                client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            Broadcastmessage($"[{dissconnectedUser.Username}] Dissconnected!");
        }
    }

}

