// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Network;
// using SCPackets.Models;
// using SCPackets.Packets.Login;
// using SharpDj.Domain.Entity;
// using SharpDj.Infrastructure;
// using SharpDj.Server.Models;
// using SharpDj.Server.Security;
// using SharpDj.Server.Singleton;
// using Log = Serilog.Log;
//
// namespace SharpDj.Server.Management.HandlersAction
// {
//     public class ServerLoginAction : RequestHandler<LoginRequest>
//     {
//         private readonly ServerContext _context;
//
//         public ServerLoginAction(ServerContext context)
//         {
//             _context = context;
//         }
//
//         
//     }
//
//     public static class LoginHelper
//     {
//         public static void FillData(this PreviewLogin data, UserEntity userEntity, ServerContext _context)
//         {
//             data.User = userEntity.ToUserClient();
//
//             //Pull all rooms
//             foreach (var roomModel in RoomSingleton.Instance.RoomInstances.GetList())
//             {
//                 data.RoomOutsideModelList.Add(roomModel.ToRoomOutsideModel());
//             }
//
//             //Pull his rooms
//             var userRooms = _context.Rooms
//                 .Include(x => x.ConfigEntity)
//                 .Where(x => x.User.Id.Equals(userEntity.Id));
//
//             foreach (var room in userRooms)
//             {
//                 //TODO data.UserRoomList.Add(room.ToRoomModel());
//             }
//         }
//     }
// }
