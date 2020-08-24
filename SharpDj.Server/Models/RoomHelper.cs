﻿using System.Collections.Generic;
using Network;
using SCPackets.Models;
using SCPackets.Packets.CreateRoomMessage;
using SCPackets.Packets.RoomNewMessageRequest;

namespace SharpDj.Server.Models
{
    public class RoomHelper
    {
        private readonly IReadOnlyCollection<ServerUserModel> _users;

        public RoomHelper(IReadOnlyCollection<ServerUserModel> users)
        {
            _users = users;
        }

        public void MessageDistribute(CreateRoomMessageRequest request, UserClient author)
        {
            var message = new RoomNewMessageRequest(request, author);
            foreach (var connection in GetConnections)
                connection.Send(message);
        }

        public IEnumerable<Connection> GetConnections => _users.GetAllConnections();
    }
}