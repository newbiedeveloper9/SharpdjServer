using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using SCPackets;
using SCPackets.Packets.CreateRoom;
using SharpDj.Server.Entity;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    class ServerCreateRoomAction : ActionAbstract<CreateRoomRequest>, 
    {
        private readonly ServerContext _context;

        public ServerCreateRoomAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(CreateRoomRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                var author = ConnectionExtension.GetClient(conn);
                if (ext.SendRequestOrIsNull(author)) return;

                #region Validation
                var roomExist = _context.Rooms.Any(x => x.Name.Equals(req.RoomDetailsModel.Name));

                var validation = new DictionaryConditionsValidation<CreateRoomResult>();
                validation.Conditions.Add(CreateRoomResult.AlreadyExist, roomExist);
                validation.Conditions.Add(CreateRoomResult.NameError, !DataValidation.LengthIsValid(req.RoomDetailsModel.Name, 2, 40));
                validation.Conditions.Add(CreateRoomResult.ImageError, !DataValidation.ImageIsValid(req.RoomDetailsModel.ImageUrl));

                validation.Conditions.Add(CreateRoomResult.LocalMessageError,
                    (!DataValidation.LengthIsValid(req.RoomDetailsModel.LocalEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomDetailsModel.LocalLeaveMessage, 0, 512)));

                validation.Conditions.Add(CreateRoomResult.PublicMessageError,
                    (!DataValidation.LengthIsValid(req.RoomDetailsModel.PublicEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomDetailsModel.PublicLeaveMessage, 0, 512)));

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@LoginResult}", (CreateRoomResult)validate);
                    ext.SendPacket(new CreateRoomResponse((CreateRoomResult)validate, req));
                    return;
                }
                #endregion Validation

                var room = new Room();
                room.ImportByRoomModel(req.RoomDetailsModel, author.User);

                _context.Rooms.Add(room);
                _context.SaveChanges();
                RoomSingleton.Instance.RoomInstances.Add(room.ToRoomInstance());

                ext.SendPacket(
                    new CreateRoomResponse(CreateRoomResult.Success, req) {RoomDetails = room.ToRoomModel()});
                Log.Information("RoomDetails has been created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                ext.SendPacket(new CreateRoomResponse(CreateRoomResult.Error, req));
            }
        }
    }
}
