using System;
using System.Collections.Generic;
using Autofac;
using Network;
using Network.Enums;
using SharpDj.Domain.Builders;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Handlers;
using SharpDj.Server.Application.Handlers.Base;
using SharpDj.Server.Application.Handlers.CoR;
using SharpDj.Server.Application.Management.Config;
using Log = Serilog.Log;

namespace SharpDj.Server.Application
{
    class App
    {
        private readonly IConfig _config;
        private readonly ServerContext _context;

        private readonly ServerConnectionContainer _serverContainer;
        private readonly IEnumerable<IAction> _actionRegisterList;

        public App(IConfig config, ServerContext context, IComponentContext componentContext)
        {
            _config = config;
            _context = context;

            Log.Information("Starting server on socket {@IP}:{@ServerPort}...",
                _config.Ip, _config.Port);

            _serverContainer = new ServerBuilder()
                .ConfigureServer(_config.Ip, _config.Port)
                .SetRSA(config.RSAKeySize)
                .Build();

            _serverContainer.ConnectionLost += ServerConnectionLost;
            _serverContainer.ConnectionEstablished += ServerServerEstablished;

            var tmp = componentContext.Resolve(typeof(IEnumerable<IAction>));
        }

        public void Start()
        {
            _serverContainer.Start();
            Log.Information("Server is running!");
        }

        private void ServerConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            //todo await new ServerDisconnectAction(_context).Action(new DisconnectRequest(), connection, true);

            Log.Warning("[{@Reason}]: {@IP} connection lost.", connection.IPRemoteEndPoint, closeReason);
        }

        private void ServerServerEstablished(Connection connection, ConnectionType connectionType)
        {
            try
            {
                Log.Information(
                    "{@Count} {@TypeEntity} connected on port {@IP}",
                    _serverContainer.Count, connection.GetType(), connection.IPRemoteEndPoint.Port);

                if (_config.Logging)
                {
                    connection.EnableLogging = true;
                    connection.LogIntoStream(Console.OpenStandardOutput());
                }

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
