using System;
using Network;
using Network.Enums;
using SCPackets.Disconnect;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Models.EF;
using ConnectionType = Network.Enums.ConnectionType;
using Log = Serilog.Log;

namespace SharpDj.Server.Management
{
    class Server
    {
        private readonly ServerConfig _config;
        private readonly ServerConnectionContainer _connectionContainer;
        private readonly ServerPacketsToHandleList _packetsList;
        private readonly ServerContext _context;

        public Server(ServerConfig config)
        {
            _context = new ServerContext();
            _config = config;

            Log.Information("Starting server on socket {@IP}:{@Port}...",
                _config.Ip, _config.Port);

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
            Log.Information("Server is running!");
        }

        private void ServerConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            new ServerDisconnectAction(_context).Action(new DisconnectRequest(), connection, true);

            Log.Warning("{@IP} connection lost", connection.IPRemoteEndPoint);
        }

        private void ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Log.Information(
                    "{@Count} {@Type} connected on port {@IP}",
                    _connectionContainer.Count, connection.GetType(), connection.IPRemoteEndPoint.Port);

                connection.EnableLogging = false;
                connection.LogIntoStream(Console.OpenStandardOutput());
                _packetsList.RegisterPackets(connection);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
        }
    }
}
