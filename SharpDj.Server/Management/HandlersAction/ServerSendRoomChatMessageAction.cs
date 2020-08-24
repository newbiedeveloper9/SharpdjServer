using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Packets.CreateRoomMessage;
using SharpDj.Domain.Entity;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerSendRoomChatMessageAction : ActionAbstract<CreateRoomMessageRequest>
    {
        private readonly ServerContext _context;

        public ServerSendRoomChatMessageAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(CreateRoomMessageRequest request, Connection conn)
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
                    Log.Information("RoomDetails with id @RoomId doesn't exist.", roomId);
                    conn.Send(new CreateRoomMessageResponse(CreateRoomMessageResult.Error));
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
                conn.Send(new CreateRoomMessageResponse(CreateRoomMessageResult.Success));

                Log.Information("User {@User} send a message to roomDetails {@RoomDetails}", active.User, roomInstance.Name);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                conn.Send(new CreateRoomMessageResponse(CreateRoomMessageResult.Error));
            }
        }
    }
}
