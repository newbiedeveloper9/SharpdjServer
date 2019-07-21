using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using SCPackets.ConnectToRoom;
using SCPackets.Models;
using SCPackets.NotLoggedIn;
using SCPackets.Ping;
using Server.Management.Singleton;
using Server.Models;

namespace Server.Management.HandlersAction
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
                if (ext.TrueAndLogoutIfObjIsNull(active)) return;

                var room = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == request.RoomId);
                var connected = room?.Users.GetList().Contains(active);

                #region validation
                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, room == null);
                validation.Conditions.Add(Result.AlreadyConnected, connected == true);

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new ConnectToRoomResponse((Result)result, request));
                    return;
                }
                #endregion validation

                active.ActiveRoom = new RoomUserConnection(room.Id);

                var userList = room.Users.GetList().Select(serverUserModel => serverUserModel.User.ToUserClient());

                ext.SendPacket(new ConnectToRoomResponse(Result.Success,
                    room.ToRoomOutsideModel(), userList.ToList(), request));
            }
            catch (Exception e)
            {
                ext.SendPacket(new ConnectToRoomResponse(Result.Error, request));
                throw e;
            }
        }
    }
}