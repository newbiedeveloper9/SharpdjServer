/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SCPackets.Packets.CreateRoomMessage;
using Serilog;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Domain.Factory;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;
using ActiveUserBag = SharpDj.Server.Application.Commands.Bags.ActiveUserBag;

namespace SharpDj.Server.Application.Commands.Handlers.Room
{
    public class ServerSendRoomChatMessageAction : RequestHandler<CreateRoomMessageRequest>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IChatMessageFactory _chatMessageFactory;

        public ServerSendRoomChatMessageAction(IRoomRepository roomRepository, 
            IChatMessageFactory chatMessageFactory, 
            IDictionaryConverter<IActionBag> bagsConverter, IServiceProvider serviceProvider)
            : base(bagsConverter, serviceProvider)
        {
            _roomRepository = roomRepository;
            _chatMessageFactory = chatMessageFactory;
        }

        protected override async Task Action(CreateRoomMessageRequest request, Connection connection,
            List<IActionBag> actionBags)
        {
            var currentUser = BagConverter.Get<ActiveUserBag>(actionBags).ActiveUser;

            var activeRoomId = currentUser.ActiveRoomId;
            var activeRoomInstance = RoomSingleton.Instance.RoomInstances.FirstOrDefault(x => x.Id == activeRoomId);

            if (!RoomExists(activeRoomId, activeRoomInstance))
            {
                await connection.SendAsync<CreateRoomMessageResponse>(
                    new CreateRoomMessageResponse(CreateRoomMessageResult.Error, request));
                return;
            }

            var room = await _roomRepository.GetRoomByIdAsync(activeRoomId);

            var chatMessage = _chatMessageFactory.CreateChatMessage(currentUser.UserEntity, request.Post.Color.RGB, request.Post.Message);
            room.Posts.Add(chatMessage);
            await _roomRepository.UpdateRoom(room);

            activeRoomInstance.TemporaryRoomHelper.MessageDistribute(request, currentUser.UserEntity.ToUserClient());

            await connection.SendAsync<CreateRoomMessageResponse>(
                new CreateRoomMessageResponse(CreateRoomMessageResult.Success, request));

            Log.Information("UserEntity {@UserEntity} send a message to roomDetails {@RoomDetails}",
                currentUser.UserEntity,
                activeRoomInstance.Name);
        }

        private bool RoomExists(int roomId, RoomEntityInstance roomInstance)
        {
            if (roomInstance == null)
            {
                Log.Information("RoomDetails with id @RoomId doesn't exist", roomId);
                return false;
            }

            return true;
        }
    }
}*/