using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using SCPackets;
using SCPackets.Packets.CreateRoom;
using SharpDj.Common;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerCreateRoomAction : ActionAbstract<CreateRoomRequest>
    {
        private readonly ServerContext _context;
        private readonly RoomMapper _roomMapper;

        public ServerCreateRoomAction(ServerContext context, RoomMapper roomMapper)
        {
            _context = context;
            _roomMapper = roomMapper;
        }

        public override async Task Action(CreateRoomRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                var author = ConnectionExtension.GetClient(conn);
                if (ext.SendRequestOrIsNull(author) || !Validate(req, ext))
                    return;

                var room = _roomMapper.MapToEntity(req.RoomDetailsModel,
                    new RoomMapper.RoomMapperBag(author.UserEntity));

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                RoomSingleton.Instance.RoomInstances.Add((RoomEntityInstance)room);

                ext.SendPacket(
                    new CreateRoomResponse(CreateRoomResult.Success, req)
                    {
                        RoomDetails = req.RoomDetailsModel
                    });

                Log.Information("RoomDetails has been created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                ext.SendPacket(new CreateRoomResponse(CreateRoomResult.Error, req));
            }
        }

        private bool Validate(CreateRoomRequest request, ConnectionExtension ext)
        {
            var roomExist = _context.Rooms.Any(x => x.Name.Equals(request.RoomDetailsModel.Name));

            var validation = new DictionaryConditionsValidation<CreateRoomResult>();
            validation.Conditions.Add(CreateRoomResult.AlreadyExist, roomExist);
            validation.Conditions.Add(CreateRoomResult.NameError, !DataValidation.LengthIsValid(request.RoomDetailsModel.Name, 2, 40));
            validation.Conditions.Add(CreateRoomResult.ImageError, !DataValidation.ImageIsValid(request.RoomDetailsModel.ImageUrl));

            var roomConfig = request.RoomDetailsModel.RoomConfigDTO;
            validation.Conditions.Add(CreateRoomResult.LocalMessageError,
                (!DataValidation.LengthIsValid(roomConfig.LocalEnterMessage, 0, 512) ||
                 !DataValidation.LengthIsValid(roomConfig.LocalLeaveMessage, 0, 512)));

            validation.Conditions.Add(CreateRoomResult.PublicMessageError,
                (!DataValidation.LengthIsValid(roomConfig.PublicEnterMessage, 0, 512) ||
                 !DataValidation.LengthIsValid(roomConfig.PublicLeaveMessage, 0, 512)));

            var validate = validation.AnyError();
            if (validate != null)
            {
                Log.Information("Validation has failed. {@LoginResult}", (CreateRoomResult)validate);
                ext.SendPacket(new CreateRoomResponse((CreateRoomResult)validate, request));
                return false;
            }

            return true;
        }
    }
}
