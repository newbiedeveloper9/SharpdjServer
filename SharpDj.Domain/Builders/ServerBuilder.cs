using Network;
using Network.RSA;
using Serilog;

namespace SharpDj.Domain.Builders
{
    public class ServerBuilder : IServerBuilder
    {
        private ServerConnectionContainer _serverConnection;

        private string _ip;
        private short _port;
        private short? _rsaKeySize;

        public IServerBuilder ConfigureServer(string ip, short port)
        {
            _ip = ip;
            _port = port;

            return this;
        }

        public IServerBuilder SetRSA(short? rsaKeySize)
        {
            _rsaKeySize = rsaKeySize;

            return this;
        }

        public ServerConnectionContainer Build()
        {
            if (_rsaKeySize.HasValue)
            {
                _serverConnection = ConnectionFactory.CreateSecureServerConnectionContainer(_ip, _port, _rsaKeySize.Value, false);
                Log.Information("Server will run {@Secure}...", "secured");
            }
            else
            {
                _serverConnection = ConnectionFactory.CreateServerConnectionContainer(_ip, _port, false);
                Log.Information("Server will run {@Secure}...", "unsecured");
            }


            return _serverConnection;
        }
    }
}
