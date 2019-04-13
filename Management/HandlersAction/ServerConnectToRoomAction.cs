using System;
using System.Linq;
using Network;
using SCPackets.ConnectToRoom;
using SCPackets.NotLoggedIn;
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

                var room = RoomSingleton.Instance.Rooms.FirstOrDefault(x => x.Id == request.RoomId);
                var connected = room?.Users.Contains(active);

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, room == null);
                validation.Conditions.Add(Result.AlreadyConnected, connected.GetValueOrDefault());

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new ConnectToRoomResponse((Result)result, request));
                    return;
                }

                active.RoomUserConnection.Add(new RoomUserConnection(room.Id, RoomConnectionType.Active));
                room.Users.Add(active);
                ext.SendPacket(new ConnectToRoomResponse(Result.Success, request));
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