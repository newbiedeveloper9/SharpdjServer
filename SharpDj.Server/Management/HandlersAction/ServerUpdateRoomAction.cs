using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets;
using SCPackets.NotLoggedIn;
using SCPackets.UpdateRoomData;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models.EF;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
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
                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == req.Room.Id);

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

                var validate = validation.Validate();
                if (validate != null)
                {
                    Log.Information("Validation failed. {@Result}", (Result)validate);
                    ext.SendPacket(new UpdateRoomDataResponse((Result)validate, req));
                    return;
                }

                if (room?.Author.Id == active.User.Id)
                {
                    roomInstance.ImportByRoomModel(req.Room, active.User);
                    room.ImportByRoomModel(req.Room, active.User);

                    _context.SaveChanges();

                    var response = new UpdateRoomDataResponse(Result.Success, req) { Room = room.ToRoomModel() };
                    ext.SendPacket(response);

                    roomInstance.SendUpdateRequest();
                }
                else
                {
                    ext.SendPacket(new UpdateRoomDataResponse(Result.Error, req));
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new UpdateRoomDataResponse(Result.Error, req));
            }
        }
    }
}
