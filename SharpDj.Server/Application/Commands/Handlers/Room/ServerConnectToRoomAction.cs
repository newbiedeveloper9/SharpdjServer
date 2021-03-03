/*using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Network;
using SCPackets.Packets.ConnectToRoom;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Commands.Handlers;
using SharpDj.Server.Application.Management;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Models;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerConnectToRoomAction : RequestHandler<ConnectToRoomRequest>
    {
        private readonly ServerContext _context;


        public ServerConnectToRoomAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(ConnectToRoomRequest request, Connection connection, List<IActionBag> actionBags)
        {
            if (!Validate(room, active, request))
                return;

            active.ActiveRoomId = new RoomUserConnection(room.Id);

            var userList = room.Users
                .ToReadonlyList()
                .Select(serverUserModel => serverUserModel.UserEntity.ToUserClient());

            ext.SendPacket(new ConnectToRoomResponse(ConnectToRoomResult.Success,
                room.ToRoomOutsideModel(), userList.ToList(), request));
        }

        private bool Validate(RoomEntityInstance roomEntity, ServerUserModel loggedInUser, ConnectToRoomRequest request)
        {
            var connected = roomEntity?.Users
                .ToReadonlyList()
                .Any(x => x.UserEntity.Id == loggedInUser.UserEntity.Id);

            var validation = new DictionaryConditionsValidation<ConnectToRoomResult>();
            validation.Conditions.Add(ConnectToRoomResult.Error, roomEntity == null);
            validation.Conditions.Add(ConnectToRoomResult.AlreadyConnected, connected == true);

            var validate = validation.AnyError();
            if (validate != null)
            {
                Log.Information("Validation has failed. {@LoginResult}", (ConnectToRoomResult)validate);
                ext.SendPacket(new ConnectToRoomResponse((ConnectToRoomResult)validate, request));
                return false;
            }

            return true;
        }
    }
}*/