using Network;

namespace SharpDj.Common.Handlers.Base
{
    public interface IPacketRegister
    {
        void RegisterPacket(Connection conn);
    }
}