using Network;

namespace SharpDj.Server.Application.Handlers
{
    public interface IAction
    {
        void RegisterPacket(Connection conn);
    }
}