using Network;
using SCPackets.Packets.UpdateRoom;
using Serilog;
using SharpDj.Common;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Mapper;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Application.Management;

namespace SharpDj.Server.Application.Handlers
{
    public class ServerUpdateRoomAction : RequestHandler<UpdateRoomRequest>
    {
        private readonly IRoomMapper _roomMapper;
        private readonly IRoomRepository _roomRepository;

        public ServerUpdateRoomAction(IRoomMapper roomMapper, 
            IRoomRepository roomRepository, 
            IDictionaryConverter<IActionBag> dictionaryConverter)
            : base(dictionaryConverter)
        {
            _roomMapper = roomMapper;
            _roomRepository = roomRepository;
        }

        protected override async Task Action(UpdateRoomRequest req, Connection connection, List<IActionBag> actionBags)
        {
            var room = await _roomRepository.GetRoomByIdAsync(req.RoomDetails.Id);

            var validate = await Validate(req, room, room?.Name, connection);
            if (!validate)
                return;

            var activeUser = BagConverter.Get<ActiveUserBag>(actionBags).ActiveUser;

            UpdateRoomResponse response;
            if (!IsRoomCreator(room, activeUser))
            {
                response = new UpdateRoomResponse(UpdateRoomResult.Error, req);
                await connection.SendAsync<UpdateRoomResponse>(response);
                return;
            }

            room = _roomMapper.MapToEntity(req.RoomDetails, new RoomMapperBag(activeUser.UserEntity));
            await _roomRepository.UpdateRoom(room);
            await _roomRepository.UnitOfWork.SaveChangesAsync();

            response = new UpdateRoomResponse(UpdateRoomResult.Success, _roomMapper.MapToDto(room), req);
            await connection.SendAsync<UpdateRoomResponse>(response);
            ((RoomEntityInstance)room)?.SendUpdateRequest();
        }

        private bool IsRoomCreator(RoomEntity room, ServerUserModel active)
        {
            return room?.Author.Id == active.UserEntity.Id;
        }

        private async Task<bool> Validate(UpdateRoomRequest req, RoomEntity room, string roomName, Connection connection)
        {
            var roomNameExist = _roomRepository.AnyRoomContainsName(req.RoomDetails.Name);

            var validation = new DictionaryConditionsValidation<UpdateRoomResult>();
            validation.Conditions.Add(UpdateRoomResult.Error, room == null);
            validation.Conditions.Add(UpdateRoomResult.AlreadyExist, roomNameExist);
            validation.Conditions.Add(UpdateRoomResult.NameError, !DataValidation.LengthIsValid(req.RoomDetails.Name, 2, 40));
            validation.Conditions.Add(UpdateRoomResult.ImageError, !DataValidation.ImageIsValid(req.RoomDetails.ImageUrl));

            var roomConfig = req.RoomDetails.RoomConfigDTO;
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
                await connection.SendAsync<UpdateRoomResponse>(new UpdateRoomResponse((UpdateRoomResult)validate, req));
                return false;
            }

            return true;
        }
    }
}
