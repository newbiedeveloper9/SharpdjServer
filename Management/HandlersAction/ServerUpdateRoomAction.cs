using Network;
using SCPackets;
using SCPackets.NotLoggedIn;
using SCPackets.UpdateRoomData;
using Server.Models;
using System;
using System.Data.Entity;
using System.Linq;
using SCPackets.RoomOutsideUpdate;
using Server.Management.Singleton;

namespace Server.Management.HandlersAction
{
    public class ServerUpdateRoomAction
    {
        private readonly ServerContext _context;

        public ServerUpdateRoomAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(UpdateRoomDataRequest req, Connection connection)
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

                var room = _context.Rooms.Include(x => x.Author)
                    .FirstOrDefault(x => x.Id == req.Room.Id);
                var roomInstance = RoomSingleton.Instance.Rooms.FirstOrDefault(x => x.Id == req.Room.Id);

                var roomExist = _context.Rooms.Any(x => 
                    (x.Name.Equals(req.Room.Name) && x.Name != room.Name));


                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, roomInstance == null);
                validation.Conditions.Add(Result.AlreadyExist, roomExist);
                validation.Conditions.Add(Result.NameError, !DataValidation.LengthIsValid(req.Room.Name, 2, 40));
                validation.Conditions.Add(Result.ImageError, !DataValidation.ImageIsValid(req.Room.ImageUrl));

                validation.Conditions.Add(Result.LocalMessageError,
                    (!DataValidation.LengthIsValid(req.Room.LocalEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.Room.LocalLeaveMessage, 0, 512)));

                validation.Conditions.Add(Result.PublicMessageError,
                    (!DataValidation.LengthIsValid(req.Room.PublicEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.Room.PublicLeaveMessage, 0, 512)));

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new UpdateRoomDataResponse((Result)result, req));
                    return;
                }

                if (room?.Author.Id == active.User.Id)
                {
                    roomInstance.ImportByRoomModel(req.Room, active.User);
                    room.ImportByRoomModel(req.Room, active.User);

                    _context.SaveChanges();

                    var response = new UpdateRoomDataResponse(Result.Success, req) { Room = room.ToRoomModel() };
                    ext.SendPacket(response);
                    var roomOutside = roomInstance.ToRoomOutsideModel();

                    foreach (var user in ClientSingleton.Instance.Users)
                    {
                        user.Connection.Send(new RoomOutsideUpdateRequest(roomOutside));
                    }
                }
                else
                {
                    ext.SendPacket(new UpdateRoomDataResponse(Result.Error, req));
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ext.SendPacket(new UpdateRoomDataResponse(Result.Error, req));
            }
        }
    }
}
