using Network;

namespace SharpDj.Server.Application.Handlers.Base
{
    public interface IAction
    {
        void RegisterPacket(Connection conn);
    }
}