using System;
using System.Linq;
using Network;
using SCPackets.ConnectToRoom;
using SCPackets.NotLoggedIn;
using SCPackets.Ping;
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
                var connected = room?.Users.Contains(active);

                if (connected.GetValueOrDefault())
                {
                    ext.SendPacket(new ConnectToRoomResponse(Result.Success, room?.ToRoomOutsideModel(), request));
                    return;
                }

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, room == null);

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new ConnectToRoomResponse((Result)result, request));
                    return;
                }

                active.AddIfNotExist(new RoomUserConnection(room.Id));

                if (!room.Users.Contains(active))
                    room.Users.Add(active);

                ext.SendPacket(new ConnectToRoomResponse(Result.Success, room.ToRoomOutsideModel(), request));
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