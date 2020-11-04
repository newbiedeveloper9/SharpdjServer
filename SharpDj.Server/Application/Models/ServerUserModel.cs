using System.Collections.Generic;
using System.Linq;
using Network;
using SharpDj.Domain.Entity;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Models
{
    public class ServerUserModel
    {
        private RoomUserConnection _activeRoom;
        public UserEntity UserEntity { get; set; }
        public IList<Connection> Connections { get; set; }

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

        public ServerUserModel(UserEntity userEntity, Connection connection)
        {
            Connections = new List<Connection>();

            UserEntity = userEntity;
            Connections.Add(connection);
        }
    }
}
