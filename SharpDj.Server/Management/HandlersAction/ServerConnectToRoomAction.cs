using System;
using System.Linq;
using Network;
using SCPackets.ConnectToRoom;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models;
using SharpDj.Server.Models.EF;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerConnectToRoomAction
    {
        private readonly ServerContext _context;

        public ServerConnectToRoomAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(ConnectToRoomRequest request, Connection connection)
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

                var validate = validation.Validate();
                if (validate != null)
                {
                    Log.Information("Validation failed. {@Result}", (Result)validate);
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