using System.Collections.Generic;
using Network;
using SCPackets.Models;
using SCPackets.Packets.CreateRoomMessage;
using SCPackets.Packets.RoomNewMessageRequest;
using SharpDj.Common;
using SharpDj.Common.ListWrapper;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Extensions;

namespace SharpDj.Server.Models
{
    public class RoomHelper
    {
        private readonly ListWrapper<ServerUserModel> _users;

        public RoomHelper(ListWrapper<ServerUserModel> users)
        {
            _users = users;
        }

        public void MessageDistribute(CreateRoomMessageRequest request, UserClient author) //TODO fix this ugly code asap
        {
            var message = new RoomNewMessageRequest(request, author);
            foreach (var connection in GetConnections)
            {
                connection.Send(message);
            }
        }

        public IEnumerable<Connection> GetConnections => _users.ToReadonlyList().GetAllConnections();
    }
}
