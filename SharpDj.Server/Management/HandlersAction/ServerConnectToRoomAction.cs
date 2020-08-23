using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using SCPackets.Packets.ConnectToRoom;
using SharpDj.Server.Entity;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerConnectToRoomAction : ActionAbstract<ConnectToRoomRequest>
    {
        private readonly ServerContext _context;


        public ServerConnectToRoomAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(ConnectToRoomRequest request, Connection connection)
        {
            var ext = new ConnectionExtension(connection, this);
            try
            {
                var active = ConnectionExtension.GetClient(connection);
                if (ext.SendRequestOrIsNull(active)) return;

                var room = RoomSingleton.Instance.RoomInstances
                    .GetList()
                    .FirstOrDefault(x => x.Id == request.RoomId);

                var connected = room?.Users
                    .GetList()
                    .Any(x=>x.User.Id == active.User.Id);

                #region validation
                var validation = new DictionaryConditionsValidation<ConnectToRoomResult>();
                validation.Conditions.Add(ConnectToRoomResult.Error, room == null);
                validation.Conditions.Add(ConnectToRoomResult.AlreadyConnected, connected == true);

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@LoginResult}", (ConnectToRoomResult)validate);
                    ext.SendPacket(new ConnectToRoomResponse((ConnectToRoomResult)validate, request));
                    return;
                }
                #endregion validation

                active.ActiveRoom = new RoomUserConnection(room.Id);

                var userList = room.Users
                    .GetList()
                    .Select(serverUserModel => serverUserModel.User.ToUserClient());

                ext.SendPacket(new ConnectToRoomResponse(ConnectToRoomResult.Success,
                    room.ToRoomOutsideModel(), userList.ToList(), request));
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new ConnectToRoomResponse(ConnectToRoomResult.Error, request));
            }
        }
    }
}