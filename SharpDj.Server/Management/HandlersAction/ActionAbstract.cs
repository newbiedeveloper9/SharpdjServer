using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using Network.Packets;

namespace SharpDj.Server.Management.HandlersAction
{
    internal interface IAction
    {
        void RegisterPacket(Connection conn);
    }

    public abstract class ActionAbstract<TReq> : IAction where TReq : RequestPacket
    {
        protected readonly PacketReceivedHandler<TReq> PacketHandler;

        protected ActionAbstract()
        {
            PacketHandler = async (packet, connection) =>
                await Action(packet, connection).ConfigureAwait(false);
        }

        public abstract Task Action(TReq req, Connection conn);

        public void RegisterPacket(Connection conn)
        {
            conn.RegisterStaticPacketHandler(PacketHandler);
        }
    }
}