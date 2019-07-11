using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Network;
using Server.Models;

namespace Server.Management
{
    public class ServerUserModel
    {
        public User User { get; set; }
        public Connection Connection { get; set; }
        private readonly List<RoomUserConnection> _roomUserConnectionList;
        public ReadOnlyCollection<RoomUserConnection> RoomList => new ReadOnlyCollection<RoomUserConnection>(_roomUserConnectionList);

        public ServerUserModel(User user, Connection connection)
        {
            _roomUserConnectionList = new List<RoomUserConnection>();

            User = user;
            Connection = connection;
        }

        public void AddIfNotExist(RoomUserConnection room)
        {
            var exists = _roomUserConnectionList.FirstOrDefault(x => x.RoomId == room.RoomId);
            if (exists == null)
            {
                _roomUserConnectionList.Add(room);
            }
        }

        public void RemoveRoom(RoomUserConnection room)
        {
            _roomUserConnectionList.Remove(room);
        }
    }

    public class RoomUserConnection
    {
        public RoomUserConnection(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; set; }
        public bool Listening { get; set; }
    }
}
