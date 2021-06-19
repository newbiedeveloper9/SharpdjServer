using Microsoft.Extensions.Options;
using Network;
using Serilog;
using SharpDj.Domain.Options;

namespace SharpDj.Domain.Builders
{
    public class ServerBuilder : IServerBuilder
    {
        private readonly SettingsOptions _settings;

        public ServerBuilder(IOptions<SettingsOptions> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        public ServerConnectionContainer Build()
        {
            ServerConnectionContainer serverConnection;

            if (_settings.RSAKeySize.HasValue && _settings.RSAKeySize > 0)
            {
                serverConnection = ConnectionFactory.CreateSecureServerConnectionContainer(_settings.Ip, _settings.Port, _settings.RSAKeySize.Value, false);
                Log.Information("Server will run {@Secure}...", "secured");
            }
            else
            {
                serverConnection = ConnectionFactory.CreateServerConnectionContainer(_settings.Ip, _settings.Port, false);
                Log.Information("Server will run {@Secure}...", "unsecured");
            }

            return serverConnection;
        }
    }
}
