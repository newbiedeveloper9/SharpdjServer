﻿using Network;
using SCPackets;
using SCPackets.CreateRoom;
using SCPackets.CreateRoom.Container;
using Server.Models;
using System;

namespace Server.Management.HandlersAction
{
    class ServerCreateRoomAction
    {
        private readonly ServerContext _context;
        public ServerCreateRoomAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(CreateRoomRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                var author = ConnectionExtension.GetClient(conn);

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, author == null);
                validation.Conditions.Add(Result.NameError, !DataValidation.LengthIsValid(req.RoomModel.Name, 2, 40));
                validation.Conditions.Add(Result.ImageError, !DataValidation.ImageIsValid(req.RoomModel.ImageUrl));

                validation.Conditions.Add(Result.LocalMessageError,
                    (!DataValidation.LengthIsValid(req.RoomModel.LocalEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomModel.LocalLeaveMessage, 0, 512)));

                validation.Conditions.Add(Result.PublicMessageError,
                    (!DataValidation.LengthIsValid(req.RoomModel.PublicEnterMessage, 0, 512) ||
                     !DataValidation.LengthIsValid(req.RoomModel.PublicLeaveMessage, 0, 512)));

                var result = validation.Validate(); 
                if (result != null)
                {
                    ext.SendPacket(new CreateRoomResponse((Result)result, req));
                    return;
                }

                var room = new Room();
                room = room.ToServerModel(req.RoomModel, author.User);

                _context.Rooms.Add(room);
                _context.SaveChanges();

                ext.SendPacket(new CreateRoomResponse(Result.Success, req));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ext.SendPacket(new CreateRoomResponse(Result.Error, req));
            }
        }
    }
}