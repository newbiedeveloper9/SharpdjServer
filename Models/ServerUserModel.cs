using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Network;
using SCPackets;
using Server.Models;

namespace Server.Management
{
    public class ServerUserModel
    {
        private RoomUserConnection _activeRoom;
        public User User { get; set; }
        public Connection Connection { get; set; }

        public RoomUserConnection ActiveRoom
        {
            get => _activeRoom;
            set
            {
                if (value == _activeRoom) return;

                var room = RoomSingleton.Instance.RoomInstances.GetList().
                    FirstOrDefault(x => x.Users.GetList().Contains(this));
                room?.Users.Remove(this);

                var destinationRoom = RoomSingleton.Instance.RoomInstances.GetList()
                    .FirstOrDefault(x => x.Id == value.RoomId);
                destinationRoom?.Users.Add(this);

                _activeRoom = value;
            }
        }

        public ServerUserModel(User user, Connection connection)
        {
            User = user;
            Connection = connection;
        }
    }

    public class RoomUserConnection
    {
        public RoomUserConnection(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; set; }
    }
}
