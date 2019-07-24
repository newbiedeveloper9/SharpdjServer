using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Network;
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

        public async void Action(PullPostsInRoomRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.TrueAndLogoutIfObjIsNull(active)) return;

                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == active.ActiveRoom.RoomId);
                if (roomInstance == null)
                {
                    conn.Send(new PullPostsInRoomResponse(Result.Error));
                    return;
                }

                var roomContext = _context.Rooms.Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == request.RoomId).Result;
                var postsServer = roomContext.Posts.Reverse().Skip(request.MessageCount).Take(30).Reverse();
               
                if (!postsServer.Any())
                {
                    conn.Send(new PullPostsInRoomResponse(Result.EOF));
                    return;
                }

                var response = new PullPostsInRoomResponse(Result.Success);
                response.Posts = postsServer.Select(x => x.ToOutsideModel()).ToList();

                conn.Send(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conn.Send(new PullPostsInRoomResponse(Result.Error));
            }
        }
    }
}