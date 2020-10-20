using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Network;
using Network.Enums;
using SCPackets.Packets.Disconnect;
using SharpDj.Infrastructure;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using ConnectionType = Network.Enums.ConnectionType;
using Log = Serilog.Log;

namespace SharpDj.Server.Application
{
    class ServerApp
    {
        private readonly IServerConfig _config;
        private readonly ServerContext _context;

        private readonly ServerConnectionContainer _connectionContainer;
        private readonly IEnumerable<IAction> _actionRegisterList;

        public ServerApp(IServerConfig config, ServerContext context, IComponentContext componentContext)
        {
            _config = config;
            _context = context;

            Log.Information("Starting server on socket {@IP}:{@Port}...",
                _config.Ip, _config.Port);

            _connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(config.Ip, config.Port, config.RSAKeySize, false);
            _connectionContainer.ConnectionLost += async (connection, type, closeReason) =>
                    await ServerConnectionLost(connection, type, closeReason);
            _connectionContainer.ConnectionEstablished += ServerConnectionEstablished;

            _actionRegisterList = componentContext.Resolve<IEnumerable<IAction>>();
        }

        public void Start()
        {
            _connectionContainer.Start();
            Log.Information("Server is running!");
        }

        private async Task ServerConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            await new ServerDisconnectAction(_context).Action(new DisconnectRequest(), connection, true);

            Log.Warning("{@IP} connection lost", connection.IPRemoteEndPoint);
        }

        private void ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Log.Information(
                    "{@Count} {@TypeEntity} connected on port {@IP}",
                    _connectionContainer.Count, connection.GetType(), connection.IPRemoteEndPoint.Port);

                connection.EnableLogging = _config.Logging;
                connection.LogIntoStream(Console.OpenStandardOutput());

                foreach (var action in _actionRegisterList)
                {
                    action.RegisterPacket(connection);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while trying to establish connection");
            }
        }
    }
}
