using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using NLog;
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

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();    

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
                    Logger.Info($"Validation failed. {(Result)validate}");
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
                Logger.Error(e);
                ext.SendPacket(new ConnectToRoomResponse(Result.Error, request));
            }
        }
    }
}