using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Network;
using NLog;
using SCPackets.Models;
using SCPackets.PullPostsInRoom;
using Server.Models;

namespace Server.Management.HandlersAction
{
    public class ServerPullPostsInRoomAction
    {
        private ServerContext _context;

        public ServerPullPostsInRoomAction(ServerContext context)
        {
            _context = context;
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    Logger.Info("Room with given id doesn't exist");
                    conn.Send(new PullPostsInRoomResponse(Result.Error));
                    return;
                }

                var roomContext = _context.Rooms
                    .Include(x => x.Posts)
                    .FirstOrDefaultAsync(x => x.Id == request.RoomId)
                    .Result;

                //Get 50 newer posts
                var postsServer = roomContext.Posts
                    .Reverse()
                    .Skip(request.MessageCount)
                    .Take(50)
                    .Reverse();

                var roomChatPosts = postsServer.ToList();
                if (!roomChatPosts.Any())
                {
                    Logger.Info("There is no more posts in room");
                    conn.Send(new PullPostsInRoomResponse(Result.EOF));
                    return;
                }

                var response = new PullPostsInRoomResponse(Result.Success)
                {
                    Posts = roomChatPosts
                        .Select(x => x.ToOutsideModel())
                        .ToList()
                };

                conn.Send(response);
                Logger.Info("Success");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                conn.Send(new PullPostsInRoomResponse(Result.Error));
            }
        }
    }
}