using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets;
using SCPackets.Packets.UpdateRoom;
using SharpDj.Common;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerUpdateRoomAction : ActionAbstract<UpdateRoomRequest>
    {
        private readonly ServerContext _context;
        private readonly RoomMapperService _roomMapperService;

        public ServerUpdateRoomAction(ServerContext context, RoomMapperService roomMapperService)
        {
            _context = context;
            _roomMapperService = roomMapperService;
        }
        public override async Task Action(UpdateRoomRequest req, Connection connection)
        {
            var ext = new ConnectionExtension(connection, this);
            try
            {
                var active = ConnectionExtension.GetClient(connection);
                if (ext.SendRequestOrIsNull(active)) return;

                var room = _context.Rooms
                    .Include(x => x.Author)
                    .FirstOrDefault(x => x.Id == req.RoomDetails.Id);

                var roomInstance = RoomSingleton.Instance.RoomInstances
                    .GetList()
                    .FirstOrDefault(x => x.Id == req.RoomDetails.Id);

                if (!Validate(req, roomInstance, ext, room?.Name))
                    return;

                if (room?.Author.Id == active.UserEntity.Id)
                {
                    roomInstance = (RoomEntityInstance)_roomMapperService
                        .MapToEntity(req.RoomDetails, new RoomMapperService.RoomMapperBag(active.UserEntity));
                    room = _roomMapperService
                        .MapToEntity(req.RoomDetails, new RoomMapperService.RoomMapperBag(active.UserEntity));

                    await _context.SaveChangesAsync();

                    var response = new UpdateRoomResponse(UpdateRoomResult.Success, req)
                    {
                        RoomDetails = _roomMapperService.MapToDTO(room)
                    };
                    ext.SendPacket(response);

                    roomInstance.SendUpdateRequest();
                }
                else
                {
                    ext.SendPacket(new UpdateRoomResponse(UpdateRoomResult.Error, req));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new UpdateRoomResponse(UpdateRoomResult.Error, req));
            }
        }

        private bool Validate(UpdateRoomRequest req, RoomEntityInstance roomEntityInstance, ConnectionExtension ext, string roomName)
        {
            var roomExist = _context.Rooms.Any(x =>
                       (x.Name.Equals(req.RoomDetails.Name) && x.Name != roomName));

            var roomConfig = req.RoomDetails.RoomConfigDTO;

            var validation = new DictionaryConditionsValidation<UpdateRoomResult>();
            validation.Conditions.Add(UpdateRoomResult.Error, roomEntityInstance == null);
            validation.Conditions.Add(UpdateRoomResult.AlreadyExist, roomExist);
            validation.Conditions.Add(UpdateRoomResult.NameError, !DataValidation.LengthIsValid(req.RoomDetails.Name, 2, 40));
            validation.Conditions.Add(UpdateRoomResult.ImageError, !DataValidation.ImageIsValid(req.RoomDetails.ImageUrl));

            validation.Conditions.Add(UpdateRoomResult.LocalMessageError,
                (!DataValidation.LengthIsValid(roomConfig.LocalEnterMessage, 0, 512) ||
                 !DataValidation.LengthIsValid(roomConfig.LocalLeaveMessage, 0, 512)));

            validation.Conditions.Add(UpdateRoomResult.PublicMessageError,
                (!DataValidation.LengthIsValid(roomConfig.PublicEnterMessage, 0, 512) ||
                 !DataValidation.LengthIsValid(roomConfig.PublicLeaveMessage, 0, 512)));

            var validate = validation.AnyError();
            if (validate != null)
            {
                Log.Information("Validation has failed. {@LoginResult}", (UpdateRoomResult)validate);
                ext.SendPacket(new UpdateRoomResponse((UpdateRoomResult)validate, req));
                return false;
            }

            return true;
        }
    }
}
