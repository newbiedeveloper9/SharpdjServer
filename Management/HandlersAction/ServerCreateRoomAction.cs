using Network;
using SCPackets;
using SCPackets.CreateRoom;
using SCPackets.CreateRoom.Container;
using SCPackets.NotLoggedIn;
using Server.Models;
using System;
using System.Linq;
using NLog;
using Server.Management.Singleton;

namespace Server.Management.HandlersAction
{
    class ServerCreateRoomAction
    {
        private readonly ServerContext _context;
        public ServerCreateRoomAction(ServerContext context)
        {
            _context = context;
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Action(CreateRoomRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                var author = ConnectionExtension.GetClient(conn);
                if (author == null)
                {
                    conn.Send(new NotLoggedInRequest());
                    return;
                }

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

                var validate = validation.Validate();
                if (validate != null)
                {
                    Logger.Info($"Validation failed. {(Result)validate}");
                    ext.SendPacket(new CreateRoomResponse((Result)validate, req));
                    return;
                }
                #endregion Validation

                var room = new Room();
                room.ImportByRoomModel(req.RoomModel, author.User);

                _context.Rooms.Add(room);
                _context.SaveChanges();
                RoomSingleton.Instance.RoomInstances.Add(room.ToRoomInstance());

                ext.SendPacket(new CreateRoomResponse(Result.Success, req) { Room = room.ToRoomModel() });
                Logger.Info("Success");
            }
            catch (Exception ex)
            { 
                Logger.Error(ex);
                ext.SendPacket(new CreateRoomResponse(Result.Error, req));
            }
        }
    }
}
