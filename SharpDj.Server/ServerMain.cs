using System;
using System.Threading.Tasks;
using Network;
using Network.Enums;
using SCPackets.Packets.Login;
using Serilog;
using SharpDj.Domain.Builders;
using SharpDj.Server.Application.Commands.Handlers;

namespace SharpDj.Server.Application
{
    public class ServerMain
    {
        private readonly IPacketRegistrator _packetRegistrator;
        private readonly ServerConnectionContainer _serverContainer;

        public ServerMain(IServerBuilder serverBuilder, IPacketRegistrator packetRegistrator)
        {
            _packetRegistrator = packetRegistrator;
            _serverContainer = serverBuilder.Build();

            _serverContainer.ConnectionLost += ServerConnectionLost;
            _serverContainer.ConnectionEstablished += ServerServerEstablished;
        }

        public void Start()
        {
            Task.Run(_serverContainer.StartTCPListener)
                .ContinueWith(x=> Log.Information("Server is running!"));
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

                if (/*_config.Logging*/true)
                {
                    connection.EnableLogging = true;
                    connection.LogIntoStream(Console.OpenStandardOutput());
                }

                _packetRegistrator.ConnectionPacketsRegister(connection);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while trying to establish connection");
            }
        }
    }
}
