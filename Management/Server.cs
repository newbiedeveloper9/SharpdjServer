using Network;
using Network.Enums;
using SCPackets.LoginPacket;
using System;
using System.IO;
using SCPackets.SendRoomChatMessage;

namespace Server.Management
{
    class Server
    {
        private readonly ServerConfig _config;
        private readonly ServerConnectionContainer _connectionContainer;
        private readonly ServerPacketsToHandleList _packetsList;


        public Server(ServerConfig config)
        {
            _config = config;
            Console.WriteLine($"Starting server on socket {_config.Ip}:{_config.Port}");
            _packetsList = new ServerPacketsToHandleList();
            _connectionContainer = ConnectionFactory.CreateServerConnectionContainer("192.168.0.103", 5666, false);

            Initialize();
        }

        private void Initialize()
        {
            _connectionContainer.ConnectionLost += ServerConnectionLost;
            _connectionContainer.ConnectionEstablished += ServerConnectionEstablished;
        }

        public void Start()
        {
            _connectionContainer.Start();
        }

        private void ServerConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            Console.WriteLine($"{connection.IPRemoteEndPoint} connection lost");
        }

        private void ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Console.WriteLine(
                    $"{_connectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");
                connection.EnableLogging = true;
                connection.LogIntoStream(Console.OpenStandardOutput());
                _packetsList.RegisterPackets(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
