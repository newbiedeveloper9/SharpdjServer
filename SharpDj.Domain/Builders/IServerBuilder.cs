using Network;

namespace SharpDj.Domain.Builders
{
    public interface IServerBuilder
    {
        public ServerConnectionContainer Build();
    }
}