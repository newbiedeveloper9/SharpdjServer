﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using SCPackets.Models;
using SCPackets.Ping;
using SCPackets.RoomChatNewMessageClient;
using SCPackets.SendRoomChatMessage;
using Server.Management;

namespace Server.Models.InstanceHelpers
{
    public class RoomHelper
    {
        private readonly IReadOnlyCollection<ServerUserModel> _users;

        public RoomHelper(IReadOnlyCollection<ServerUserModel> users)
        {
            _users = users;
        }

        public void MessageDistribute(SendRoomChatMessageRequest request, UserClientModel author)
        {
            var message = new RoomChatNewMessageRequest(request, author);
            foreach (var user in _users)
            {
                user.Connection.Send(message);
            }
        }

        public List<Connection> GetConnections
        {
            get
            {
                return _users.Select(serverUserModel => serverUserModel.Connection).ToList();
            }
        }
    }
}