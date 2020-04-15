using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Network;
using Network.Enums;
using SCPackets.Disconnect;
using SharpDj.Server.Entity;
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
        private readonly IComponentContext _componentContext;

        private readonly ServerConnectionContainer _connectionContainer;
        private readonly IEnumerable<IAction> _actionRegisterList;

        public ServerApp(IServerConfig config, ServerContext context, IComponentContext componentContext)
        {
            _config = config;
            _context = context;
            _componentContext = componentContext;

            Log.Information("Starting server on socket {@IP}:{@Port}...",
                _config.Ip, _config.Port);

            _connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(config.Ip, config.Port, config.RSAKeySize, false);

            _connectionContainer.ConnectionLost += async (connection, type, closeReason) =>
                    await ServerConnectionLost(connection, type, closeReason);
            _connectionContainer.ConnectionEstablished += async (connection, type) =>
                    await ServerConnectionEstablished(connection, type);

            _actionRegisterList = _componentContext.Resolve<IEnumerable<IAction>>();

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

        private async Task ServerConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Log.Information(
                    "{@Count} {@Type} connected on port {@IP}",
                    _connectionContainer.Count, connection.GetType(), connection.IPRemoteEndPoint.Port);

                connection.EnableLogging = _config.Logging;
                connection.LogIntoStream(Console.OpenStandardOutput());

                foreach (var action in _actionRegisterList)
                {
                    action.RegisterPacket(connection);
                }
                //_packetsList.RegisterPackets(connection);
                //_packetsList.

            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
        }
    }
}
