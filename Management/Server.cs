using Network;
using Network.Enums;
using SCPackets.LoginPacket;
using System;
using System.IO;
using NLog;
using SCPackets.Disconnect;
using SCPackets.SendRoomChatMessage;
using Server.Management.HandlersAction;
using Server.Models;
using ConnectionType = Network.Enums.ConnectionType;

namespace Server.Management
{
    class Server
    {
        private readonly ServerConfig _config;
        private readonly ServerConnectionContainer _connectionContainer;
        private readonly ServerPacketsToHandleList _packetsList;
        private ServerContext _context;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Server(ServerConfig config)
        {
            _context = new ServerContext();
            _config = config;

            Logger.Info($"Starting server on socket {_config.Ip}:{_config.Port}...");
            _packetsList = new ServerPacketsToHandleList(_context);
            _connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(config.Ip, config.Port, config.RSAKeySize, false);

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
            Logger.Info("Server is running!");
        }

        private void ServerConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            new ServerDisconnectAction(_context).Action(new DisconnectRequest(), connection, true);

            Logger.Warn($"{connection.IPRemoteEndPoint} connection lost");
        }

        private void ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Logger.Info(
                    $"{_connectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

                connection.EnableLogging = false;
                connection.LogIntoStream(Console.OpenStandardOutput());
                _packetsList.RegisterPackets(connection);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
