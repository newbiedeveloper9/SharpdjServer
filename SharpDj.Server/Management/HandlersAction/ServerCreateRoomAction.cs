using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using SCPackets;
using SCPackets.CreateRoom;
using SCPackets.NotLoggedIn;
using SCPackets.RegisterPacket;
using SharpDj.Server.Entity;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;
using Result = SCPackets.CreateRoom.Container.Result;

namespace SharpDj.Server.Management.HandlersAction
{
    class ServerCreateRoomAction : ActionAbstract<CreateRoomRequest>
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
                var roomExist = _context.Rooms.Any(x => x.Name.Equals(req.RoomModel.Name));

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.AlreadyExist, roomExist);
                validation.Conditions.Add(Result.NameError, !DataValidation.LengthIsValid(req.RoomModel.Name, 2, 40));
                validation.Conditions.Add(Result.ImageError, !DataValidation.ImageIsValid(req.RoomModel.ImageUrl));

                validation.Conditions.Add(Result.LocalMessageError,
                    (!DataValidation.LengthIsValid(req.RoomModel.LocalEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomModel.LocalLeaveMessage, 0, 512)));

                validation.Conditions.Add(Result.PublicMessageError,
                    (!DataValidation.LengthIsValid(req.RoomModel.PublicEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomModel.PublicLeaveMessage, 0, 512)));

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@Result}", (Result)validate);
                    ext.SendPacket(new CreateRoomResponse((Result)validate, req));
                    return;
                }
                #endregion Validation

                var room = new Room();
                room.ImportByRoomModel(req.RoomModel, author.User);

                _context.Rooms.Add(room);
                _context.SaveChanges();
                RoomSingleton.Instance.RoomInstances.Add(room.ToRoomInstance());

                ext.SendPacket(
                    new CreateRoomResponse(Result.Success, req) {Room = room.ToRoomModel()});
                Log.Information("Room has been created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                ext.SendPacket(new CreateRoomResponse(Result.Error, req));
            }
        }
    }
}
