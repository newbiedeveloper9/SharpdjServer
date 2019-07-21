using System;
using System.Data.Entity;
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
                if (ext.TrueAndLogoutIfObjIsNull(active)) return;

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

                var post = new RoomChatPost()
                {
                    Author = active.User,
                    Color = request.Color.ToString(),
                    Text = request.Message
                };

                var roomContext = _context.Rooms.Include(x => x.Posts).FirstOrDefault(x => x.Id == request.RoomId);
                roomContext.Posts.Add(post);
                _context.SaveChanges();

                roomInstance.ActionHelper.MessageDistribute(request, active.User.ToUserClient());
                conn.Send(new SendRoomChatMessageResponse(Result.Success));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conn.Send(new SendRoomChatMessageResponse(Result.Error));

            }
        }
    }
}
