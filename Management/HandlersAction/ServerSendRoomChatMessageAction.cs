using System;
using System.Linq;
using Network;
using SCPackets.SendRoomChatMessage;
using Server.Models;

namespace Server.Management.HandlersAction
{
    public class ServerSendRoomChatMessageAction
    {
        private ServerContext _context;

        public ServerSendRoomChatMessageAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(SendRoomChatMessageRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.LogoutIfObjIsNull(active)) return;

                var userIsInRoom = active.ActiveRoom.RoomId == request.RoomId;
                if (!userIsInRoom)
                {
                    conn.Send(new SendRoomChatMessageResponse(Result.NotInRoom));
                    return;
                }

                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == request.RoomId);
                if (roomInstance == null)
                {
                    conn.Send(new SendRoomChatMessageResponse(Result.Error));
                    return;
                }

                conn.Send(new SendRoomChatMessageResponse(Result.Success));
                roomInstance.ActionHelper.MessageDistribute(request, active.User.ToUserClient());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conn.Send(new SendRoomChatMessageResponse(Result.Error));

            }
        }
    }
}
