using System.Collections.Generic;
using System.Linq;
using Network;
using SharpDj.Domain.Entity;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Models
{
    public class ServerUserModel
    {
        public UserEntity UserEntity { get; set; }
        public List<Connection> Connections { get; set; }

        public ServerUserModel(UserEntity userEntity, Connection connection)
        {
            Connections = new List<Connection>();

            UserEntity = userEntity;
            Connections.Add(connection);
        }

        private int _activeRoomId;
        public int ActiveRoomId
        {
            get => _activeRoomId;
            set
            {
                if (value == _activeRoomId) return;

                var room = RoomSingleton.Instance.RoomInstances.ToReadonlyList().
                    FirstOrDefault(x => x.Users.ToReadonlyList().Contains(this));
                room?.Users.Remove(this);

                var destinationRoom = RoomSingleton.Instance.RoomInstances.ToReadonlyList()
                    .FirstOrDefault(x => x.Id == value);
                destinationRoom?.Users.Add(this);

                _activeRoomId = value;
            }
        }
    }
}
