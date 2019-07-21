using System.Linq;
using Network;
using Network.Packets;
using SCPackets.NotLoggedIn;
using Server.Management.Singleton;

namespace Server.Management
{
    public class ConnectionExtension
    {
        private readonly Connection _conn;
        private readonly object _sender;

        public ConnectionExtension(Connection conn, object sender)
        {
            _conn = conn;
            _sender = sender;
        }

        public void SendPacket<TPacket>(TPacket packet) where TPacket : Packet
        {
            _conn.Send(packet, _sender);
        }

        public bool TrueAndLogoutIfObjIsNull(object conditionToCheck)
        {
            if (conditionToCheck != null) return false;

            _conn.Send(new NotLoggedInRequest());
            return true;
        }

        public static ServerUserModel GetClient(Connection conn)
        {
            return ClientSingleton.Instance.Users.GetList().FirstOrDefault(x => x.Connection.Equals(conn));
        }
    }
}
