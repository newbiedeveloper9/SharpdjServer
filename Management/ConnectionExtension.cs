using Network;

namespace Server.Management
{
    public class ConnectionExtension
    {
        public static void SendPacket<TPacket>(Connection conn, TPacket packet, object any) where TPacket:Packet
        {
            conn.Send(packet, any);
        }
    }
}
