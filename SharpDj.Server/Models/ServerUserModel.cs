﻿using System.Collections.Generic;
using System.Linq;
using Network;
using SharpDj.Server.Entity;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Models
{
    public class ServerUserModel
    {
        private RoomUserConnection _activeRoom;
        public User User { get; set; }
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

        public ServerUserModel(User user, Connection connection)
        {
            Connections = new List<Connection>();

            User = user;
            Connections.Add(connection);
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

    public static class RoomUserConnectionHelper
    {
        public static RoomInstance GetActiveRoom(this RoomUserConnection roomUserConnection)
        {
            return RoomSingleton.Instance.RoomInstances.GetList()
                .FirstOrDefault(x => x.Id == roomUserConnection.RoomId);
        }
    }

    public static class ServerUserModelHelper
    {
        public static List<Connection> GetAllConnections(this IEnumerable<ServerUserModel> users)
        {
            var connections = new List<Connection>();
            foreach (var user in users)
                connections.AddRange(user.Connections);
            return connections;
        }
    }
}