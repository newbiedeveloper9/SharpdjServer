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
                if (active == null)
                {
                    connection.Send(new NotLoggedInRequest());
                    return;
                }

                var room = RoomSingleton.Instance.RoomInstances.FirstOrDefault(x => x.Id == request.RoomId);
                var connected = room?.Users.GetList().Contains(active);

                if (connected == true)
                {
                    ext.SendPacket(new ConnectToRoomResponse(Result.AlreadyConnected, request));
                    return;
                }

                #region validation
                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, room == null);

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new ConnectToRoomResponse((Result)result, request));
                    return;
                }
                #endregion validation

                active.AddIfNotExist(new RoomUserConnection(room.Id));

                if (!room.Users.GetList().Contains(active))
                    room.Users.Add(active);

                var userList = new List<UserClientModel>();
                foreach (var serverUserModel in room.Users.GetList())
                    userList.Add(serverUserModel.User.ToUserClient());

                ext.SendPacket(new ConnectToRoomResponse(Result.Success, room.ToRoomOutsideModel(), userList, request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ext.SendPacket(new ConnectToRoomResponse(Result.Error, request));
                return;
            }
        }
    }
}