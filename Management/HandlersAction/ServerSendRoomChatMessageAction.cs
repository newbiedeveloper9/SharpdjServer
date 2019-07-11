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
                if (ext.CheckObjIsNullAndSendLogoutPacket(active)) return;


                var userIsInRoom = active.RoomList.Any(x => x.RoomId == request.RoomId);
                if (!userIsInRoom)
                {
                    conn.Send(new SendRoomChatMessageResponse(Result.NotInRoom));
                    return;
                }

                var roomInstance = RoomSingleton.Instance.RoomInstances.FirstOrDefault(x => x.Id == request.RoomId);
                if (roomInstance == null)
                {
                    conn.Send(new SendRoomChatMessageResponse(Result.Error));
                    return;
                }

                conn.Send(new SendRoomChatMessageResponse(Result.Success));
                roomInstance.ActionHelper.MessageDistribute(request, active.User.ToUserClient()); //send message to all users
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conn.Send(new SendRoomChatMessageResponse(Result.Error));

            }
        }
    }
}
