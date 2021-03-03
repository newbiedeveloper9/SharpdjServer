/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Network;
using SCPackets.Packets.CreateRoom;
using SharpDj.Common;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Commands.Handlers;
using SharpDj.Server.Application.Management;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerCreateRoomAction : RequestHandler<CreateRoomRequest>
    {
        private readonly ServerContext _context;
        private readonly RoomMapper _roomMapper;

        public ServerCreateRoomAction(ServerContext context, RoomMapper roomMapper)
        {
            _context = context;
            _roomMapper = roomMapper;
        }

        public override async Task Action(CreateRoomRequest req, Connection conn, List<IActionBag> actionBags)
        {
            var logger = Log.ForContext("RoomName", req.RoomDetailsModel.Name);
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var author = ConnectionExtension.GetClient(conn);
                if (ext.IsUserLoggedIn(author) || Validate(req, ext) == false)
                {
                    return;
                }

                var mappedRoomFromRequest = _roomMapper.MapToEntity(req.RoomDetailsModel,
                    new RoomMapperBag(author.UserEntity));

                await _context.Rooms.AddAsync(mappedRoomFromRequest);
                await _context.SaveChangesAsync();

                RoomSingleton.Instance.RoomInstances.Add((RoomEntityInstance)mappedRoomFromRequest);

                var response = new CreateRoomResponse(CreateRoomResult.Success, req)
                {
                    RoomDetails = req.RoomDetailsModel
                };
                ext.SendPacket(response);

                Log.Information("RoomDetails has been created");
            }
            catch (Exception)
            {
                ext.SendPacket(new CreateRoomResponse(CreateRoomResult.Error, req));
                throw;
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
*/
