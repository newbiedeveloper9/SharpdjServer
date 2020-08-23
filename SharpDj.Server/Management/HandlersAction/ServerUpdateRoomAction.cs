using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets;
using SCPackets.Packets.UpdateRoom;
using SharpDj.Server.Entity;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerUpdateRoomAction : ActionAbstract<UpdateRoomRequest>
    {
        private readonly ServerContext _context;

        public ServerUpdateRoomAction(ServerContext context)
        {
            _context = context;
        }
        public override async Task Action(UpdateRoomRequest req, Connection connection)
        {
            var ext = new ConnectionExtension(connection, this);
            try
            {
                var active = ConnectionExtension.GetClient(connection);
                if (ext.SendRequestOrIsNull(active)) return;

                var room = _context.Rooms.Include(x => x.Author)
                    .FirstOrDefault(x => x.Id == req.RoomDetails.Id);
                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == req.RoomDetails.Id);

                var roomExist = _context.Rooms.Any(x =>
                    (x.Name.Equals(req.RoomDetails.Name) && x.Name != room.Name));


                var validation = new DictionaryConditionsValidation<UpdateRoomResult>();
                validation.Conditions.Add(UpdateRoomResult.Error, roomInstance == null);
                validation.Conditions.Add(UpdateRoomResult.AlreadyExist, roomExist);
                validation.Conditions.Add(UpdateRoomResult.NameError, !DataValidation.LengthIsValid(req.RoomDetails.Name, 2, 40));
                validation.Conditions.Add(UpdateRoomResult.ImageError, !DataValidation.ImageIsValid(req.RoomDetails.ImageUrl));

                validation.Conditions.Add(UpdateRoomResult.LocalMessageError,
                    (!DataValidation.LengthIsValid(req.RoomDetails.LocalEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomDetails.LocalLeaveMessage, 0, 512)));

                validation.Conditions.Add(UpdateRoomResult.PublicMessageError,
                    (!DataValidation.LengthIsValid(req.RoomDetails.PublicEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomDetails.PublicLeaveMessage, 0, 512)));

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@LoginResult}", (UpdateRoomResult)validate);
                    ext.SendPacket(new UpdateRoomResponse((UpdateRoomResult)validate, req));
                    return;
                }

                if (room?.Author.Id == active.User.Id)
                {
                    roomInstance.ImportByRoomModel(req.RoomDetails, active.User);
                    room.ImportByRoomModel(req.RoomDetails, active.User);

                    _context.SaveChanges();

                    var response = new UpdateRoomResponse(UpdateRoomResult.Success, req) { RoomDetails = room.ToRoomModel() };
                    ext.SendPacket(response);

                    roomInstance.SendUpdateRequest();
                }
                else
                {
                    ext.SendPacket(new UpdateRoomResponse(UpdateRoomResult.Error, req));
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new UpdateRoomResponse(UpdateRoomResult.Error, req));
            }
        }
    }
}
