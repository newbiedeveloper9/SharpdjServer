using System.Collections.Generic;
using Network;
using Server.Models;

namespace Server.Management
{
    public class ServerUserModel
    {
        public ServerUserModel(User user, Connection connection)
        {
            RoomUserConnection = new List<RoomUserConnection>();

            User = user;
            Connection = connection;
        }

        public User User { get; set; }
        public Connection Connection { get; set; }
        public List<RoomUserConnection> RoomUserConnection { get; set; }
    }

    public class RoomUserConnection
    {
        public RoomUserConnection(int roomId, RoomConnectionType roomConnectionType)
        {
            RoomId = roomId;
            RoomConnectionType = roomConnectionType;
        }

        public int RoomId { get; set; }
        public RoomConnectionType RoomConnectionType { get; set; }
    }

    public enum RoomConnectionType
    {
        Active,
        Listening,
        Sleep,
    }
}
