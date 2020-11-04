using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Models;
using SCPackets.Packets.CreateRoomMessage;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Application.Handlers
{
    public class ServerSendRoomChatMessageAction : RequestHandler<CreateRoomMessageRequest>
    {
        private readonly IRoomRepository _roomRepository;

        public ServerSendRoomChatMessageAction(IRoomRepository roomRepository, IDictionaryConverter<IActionBag> bagsConverter)
            : base(bagsConverter)
        {
            _roomRepository = roomRepository;
        }

        protected override async Task Action(CreateRoomMessageRequest request, Connection connection,
            List<IActionBag> actionBags)
        {
            var currentUser = BagConverter.Get<ActiveUserBag>(actionBags).ActiveUser;

            var activeRoom = currentUser.ActiveRoom;
            var activeRoomInstance = RoomSingleton.Instance.RoomInstances.FirstOrDefault(x => x.Id == activeRoom.RoomId);

            if (!RoomExists(activeRoom, activeRoomInstance))
            {
                await connection.SendAsync<CreateRoomMessageResponse>(
                    new CreateRoomMessageResponse(CreateRoomMessageResult.Error, request));
                return;
            }

            var room = await _roomRepository.GetRoomByIdAsync(activeRoom.RoomId);
            room.Posts.Add(GetRoomChatMessageEntity(request.Post, currentUser));
            await _roomRepository.UpdateRoom(room);

            activeRoomInstance.ActionHelper.MessageDistribute(request, currentUser.UserEntity.ToUserClient());

            await connection.SendAsync<CreateRoomMessageResponse>(
                new CreateRoomMessageResponse(CreateRoomMessageResult.Success, request));

            Log.Information("UserEntity {@UserEntity} send a message to roomDetails {@RoomDetails}",
                currentUser.UserEntity,
                activeRoomInstance.Name);
        }

        private bool RoomExists(RoomUserConnection room, RoomEntityInstance roomInstance)
        {
            if (roomInstance == null)
            {
                Log.Information("RoomDetails with id @RoomId doesn't exist", room.RoomId);
                return false;
            }

            return true;
        }

        private RoomChatMessageEntity GetRoomChatMessageEntity(ChatMessage chatMessage, ServerUserModel currentUser)
        {
            return new RoomChatMessageEntity()
            {
                User = currentUser.UserEntity,
                Color = chatMessage.Color.RGB,
                Text = chatMessage.Message
            };
        }
    }
}
