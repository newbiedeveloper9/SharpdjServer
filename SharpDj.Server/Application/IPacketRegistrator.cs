using Network;

namespace SharpDj.Server.Application
{
    public interface IPacketRegistrator
    {
        void ConnectionPacketsRegister(Connection connection);
    }
}