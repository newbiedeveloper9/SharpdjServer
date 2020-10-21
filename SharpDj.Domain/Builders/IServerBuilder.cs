using Network;

namespace SharpDj.Domain.Builders
{
    public interface IServerBuilder
    {
        public IServerBuilder ConfigureServer(string ip, short port);
        public IServerBuilder SetRSA(short? rsaKeySize);
        public ServerConnectionContainer Build();
    }
}