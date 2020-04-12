using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.PullPostsInRoom;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models.EF;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerPullPostsInRoomAction
    {
        private ServerContext _context;

        public ServerPullPostsInRoomAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(PullPostsInRoomRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.SendRequestOrIsNull(active)) return;

                var roomInstance = RoomSingleton.Instance.RoomInstances
                    .GetList()
                    .FirstOrDefault(x => x.Id == active.ActiveRoom.RoomId);
                if (roomInstance == null)
                {
                    Log.Information("Room with given id doesn't exist");
                    conn.Send(new PullPostsInRoomResponse(Result.Error));
                    return;
                }

                var room = _context.Rooms
                    .Include(x => x.Posts)
                    .FirstOrDefaultAsync(x => x.Id == request.RoomId)
                    .Result;
                if (room == null)
                {
                    Log.Error("roomId not found. {@Room}", room);
                    conn.Send(new PullPostsInRoomResponse(Result.Error));
                    return;
                }

                //Get 50 newer posts
                var postsServer = room.Posts
                    .Reverse()
                    .Skip(request.MessageCount)
                    .Take(50)
                    .Reverse();

                var roomChatPosts = postsServer.ToList();
                if (!roomChatPosts.Any())
                {
                    Log.Information("There is no more posts in room");
                    conn.Send(new PullPostsInRoomResponse(Result.EOF));
                    return;
                }

                var response = new PullPostsInRoomResponse(Result.Success);
                response.Posts = roomChatPosts
                    .Select(x => x.ToOutsideModel())
                    .ToList();

                conn.Send(response);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                conn.Send(new PullPostsInRoomResponse(Result.Error));
            }
        }
    }
}