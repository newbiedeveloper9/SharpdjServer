using System;
using System.Data.Entity;
using System.Linq;
using Network;
using NLog;
using SCPackets.SendRoomChatMessage;
using Server.Models;

namespace Server.Management.HandlersAction
{
    public class ServerSendRoomChatMessageAction
    {
        private readonly ServerContext _context;

        public ServerSendRoomChatMessageAction(ServerContext context)
        {
            _context = context;
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async void Action(SendRoomChatMessageRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.SendRequestOrIsNull(active)) return;

                var roomId = active.ActiveRoom.RoomId;

                var roomInstance = active.ActiveRoom.GetActiveRoom();
                if (roomInstance == null)
                {
                    Logger.Info("Room with given id doesn't exist");
                    conn.Send(new SendRoomChatMessageResponse(Result.Error));
                    return;
                }

                var post = new RoomChatPost()
                {
                    Author = active.User,
                    Color = request.Post.Color.ToString(),
                    Text = request.Post.Message
                };

                var roomContext = _context.Rooms
                    .Include(x => x.Posts)
                    .FirstOrDefault(x => x.Id == roomId);

                roomContext.Posts.Add(post);
                await _context.SaveChangesAsync();

                roomInstance
                    .ActionHelper
                    .MessageDistribute(request, active.User.ToUserClient());
                conn.Send(new SendRoomChatMessageResponse(Result.Success));

                Logger.Info("Success");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                conn.Send(new SendRoomChatMessageResponse(Result.Error));
            }
        }
    }
}
