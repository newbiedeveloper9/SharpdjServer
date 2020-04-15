using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using SCPackets.ConnectToRoom;
using SCPackets.RegisterPacket;
using SharpDj.Server.Entity;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;
using Result = SCPackets.ConnectToRoom.Result;

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
                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, room == null);
                validation.Conditions.Add(Result.AlreadyConnected, connected == true);

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@Result}", (Result)validate);
                    ext.SendPacket(new ConnectToRoomResponse((Result)validate, request));
                    return;
                }
                #endregion validation

                active.ActiveRoom = new RoomUserConnection(room.Id);

                var userList = room.Users
                    .GetList()
                    .Select(serverUserModel => serverUserModel.User.ToUserClient());

                ext.SendPacket(new ConnectToRoomResponse(Result.Success,
                    room.ToRoomOutsideModel(), userList.ToList(), request));
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new ConnectToRoomResponse(Result.Error, request));
            }
        }
    }
}