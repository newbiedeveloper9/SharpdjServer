using System.Collections.Generic;
using Network;
using SCPackets.Models;
using SCPackets.RoomChatNewMessageClient;
using SCPackets.SendRoomChatMessage;

namespace SharpDj.Server.Models.InstanceHelpers
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
            foreach (var connection in GetConnections)
                connection.Send(message);
        }

        public IEnumerable<Connection> GetConnections => _users.GetAllConnections();
    }
}
