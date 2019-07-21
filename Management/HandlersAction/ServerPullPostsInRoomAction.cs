using System;
using System.Data.Entity;
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

        public void Action(PullPostsInRoomRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.TrueAndLogoutIfObjIsNull(active)) return;

                var userIsInRoom = active.ActiveRoom.RoomId == request.RoomId;
                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == request.RoomId);

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.NotInRoom, !userIsInRoom);
                validation.Conditions.Add(Result.Error, roomInstance == null);

                var any = validation.Validate();
                if (any != null)
                {
                    ext.SendPacket(new PullPostsInRoomResponse((Result)any, request));
                    return;
                }

                var roomContext = _context.Rooms.Include(x => x.Posts).FirstOrDefault(x => x.Id == request.RoomId);
                var postsServer = roomContext.Posts.Skip(request.MessageCount).Take(30).ToList();
                if (!postsServer.Any())
                {
                    ext.SendPacket(new PullPostsInRoomResponse(Result.EOF, request));
                    return;
                }

                var response = new PullPostsInRoomResponse(Result.Success, request);
                response.Posts = postsServer.Select(x => x.ToOutsideModel()).ToList();

                conn.Send(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}