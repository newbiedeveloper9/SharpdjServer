using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Packets.UpdateRoom;
using SharpDj.Common;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Application.Handlers
{
    public class ServerUpdateRoomAction : RequestHandler<UpdateRoomRequest>
    {
        private readonly ServerContext _context;
        private readonly RoomMapper _roomMapper;

        public ServerUpdateRoomAction(ServerContext context, RoomMapper roomMapper, IDictionaryConverter<IActionBag> dictionaryConverter)
            : base(dictionaryConverter)
        {
            _context = context;
            _roomMapper = roomMapper;
        }

        protected override async Task Action(UpdateRoomRequest req, Connection connection, List<IActionBag> actionBags)
        {
            var room = _context.Rooms
                .Include(x => x.Author)
                .FirstOrDefault(x => x.Id == req.RoomDetails.Id);

            var roomInstance = RoomSingleton.Instance.RoomInstances
                .FirstOrDefault(x => x.Id == req.RoomDetails.Id);

            if (!Validate(req, roomInstance, room?.Name, connection))
                return;

            var active = BagConverter.Get<ActiveUserBag>(actionBags).ActiveUser;

            if (room?.Author.Id == active.UserEntity.Id)
            {
                roomInstance = (RoomEntityInstance)_roomMapper.MapToEntity(req.RoomDetails, new RoomMapperBag(active.UserEntity));
                await _context.SaveChangesAsync();

                var response = new UpdateRoomResponse(UpdateRoomResult.Success, req)
                {
                    RoomDetails = _roomMapper.MapToDTO(roomInstance)
                };

                await connection.SendAsync<UpdateRoomResponse>(response);
                roomInstance.SendUpdateRequest();
            }
            else
            {
                var response = new UpdateRoomResponse(UpdateRoomResult.Error, req);
                await connection.SendAsync<UpdateRoomResponse>(response);
            }
        }

        private bool Validate(UpdateRoomRequest req, RoomEntityInstance roomEntityInstance, string roomName, Connection connection)
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
                connection.SendAsync<UpdateRoomResponse>(new UpdateRoomResponse((UpdateRoomResult)validate, req));
                return false;
            }

            return true;
        }
    }
}
