using System.Linq;
using Network;
using Server.Management.Singleton;

namespace Server.Management
{
    public class ConnectionExtension
    {
        private readonly Connection _conn;
        private readonly object _anyClass;

        public ConnectionExtension(Connection conn, object anyClass)
        {
            _conn = conn;
            _anyClass = anyClass;
        }

        public void SendPacket<TPacket>(TPacket packet) where TPacket : Packet
        {
            _conn.Send(packet, _anyClass);
        }

        public static ServerUserModel GetClient(Connection conn)
        {
            return ClientSingleton.Instance.Users.FirstOrDefault(x => x.Connection.Equals(conn));
        }
    }
}
