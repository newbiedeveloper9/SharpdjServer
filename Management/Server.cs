using Network;
using Network.Enums;
using SCPackets.LoginPacket;
using System;
using System.IO;
using Communication.Shared;

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
            _packetsList = new ServerPacketsToHandleList();
            _connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(_config.Ip, _config.Port, 2048, false);

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
                connection.LogIntoStream(Stream.Null);
                Console.WriteLine(
                    $"{_connectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");
                _packetsList.RegisterPackets(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
