using Network;
using Network.Enums;
using SCPackets.LoginPacket;
using System;

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
            _connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(_config.Ip, _config.Port, _config.RSAKeySize, false);

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
            Console.WriteLine("lost");
        }

        private void ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            Console.WriteLine(connection.IPRemoteEndPoint);
            _packetsList.Register(connection);
        }
    }
}
