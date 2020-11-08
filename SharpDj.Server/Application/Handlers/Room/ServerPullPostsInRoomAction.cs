// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Network;
// using SCPackets.Packets.PullRoomChat;
// using SharpDj.Domain.Mapper;
// using SharpDj.Infrastructure;
// using SharpDj.Server.Singleton;
// using Log = Serilog.Log;
//
// namespace SharpDj.Server.Management.HandlersAction
// {
//     public class ServerPullPostsInRoomAction : RequestHandler<PullRoomChatRequest>
//     {
//         private readonly ServerContext _context;
//         private readonly ChatMessageMapper _chatMessageMapper;
//
//         public ServerPullPostsInRoomAction(ServerContext context, ChatMessageMapper chatMessageMapper)
//         {
//             _context = context;
//             _chatMessageMapper = chatMessageMapper;
//         }
//
//         public override async Task Action(PullRoomChatRequest chatRequest, Connection conn, List<IActionBag> actionBags)
//         {
//             var ext = new ConnectionExtension(conn, this);
//             try
//             {
//                 var active = ConnectionExtension.GetClient(conn);
//                 if (ext.IsUserLoggedIn(active)) return;
//
//                 var roomInstance = RoomSingleton.Instance.RoomInstances
//                     .GetList()
//                     .FirstOrDefault(x => x.Id == active.ActiveRoomId.RoomId);
//                 if (roomInstance == null)
//                 {
//                     Log.Information("RoomEntity with given id doesn't exist");
//                     conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
//                     return;
//                 }
//
//                 var room = _context.Rooms
//                     .Include(x => x.Posts)
//                     .FirstOrDefaultAsync(x => x.Id == chatRequest.RoomId)
//                     .Result;
//                 if (room == null)
//                 {
//                     Log.Error("RoomEntity with id{@RoomId} not found.", chatRequest.RoomId);
//                     conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
//                     return;
//                 }
//
//                 //Get 50 newer posts
//                 var postsServer = room.Posts
//                     .Reverse()
//                     .Skip(chatRequest.MessageCount)
//                     .Take(50)
//                     .Reverse();
//
//                 var roomChatPosts = postsServer.ToList();
//                 if (!roomChatPosts.Any())
//                 {
//                     Log.Information("There is no more posts in roomDetails");
//                     conn.Send(new PullRoomChatResponse(PullRoomChatResult.EOF));
//                     return;
//                 }
//
//                 var response = new PullRoomChatResponse(PullRoomChatResult.Success);
//                 response.Posts = roomChatPosts
//                     .Select(x => _chatMessageMapper.MapToDto(x))
//                     .ToList();
//
//                 conn.Send(response);
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e.StackTrace);
//                 conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
//             }
//         }
//     }
// }