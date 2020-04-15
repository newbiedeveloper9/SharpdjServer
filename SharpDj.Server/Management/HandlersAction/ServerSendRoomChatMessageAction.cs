﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.SendRoomChatMessage;
using SharpDj.Server.Entity;
using SharpDj.Server.Models;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerSendRoomChatMessageAction : ActionAbstract<SendRoomChatMessageRequest>
    {
        private readonly ServerContext _context;

        public ServerSendRoomChatMessageAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(SendRoomChatMessageRequest request, Connection conn)
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
                    Log.Information("Room with given id doesn't exist");
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
                _context.SaveChanges();

                roomInstance
                    .ActionHelper
                    .MessageDistribute(request, active.User.ToUserClient());
                conn.Send(new SendRoomChatMessageResponse(Result.Success));

                Log.Information("User {@user} send a message to room {@room}", active.User, roomInstance.Name);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                conn.Send(new SendRoomChatMessageResponse(Result.Error));
            }
        }
    }
}