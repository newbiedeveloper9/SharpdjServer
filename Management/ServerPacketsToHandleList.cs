using Network;
using SCPackets;
using SCPackets.ConnectToRoom;
using SCPackets.CreateRoom;
using SCPackets.Disconnect;
using SCPackets.LoginPacket;
using SCPackets.RegisterPacket;
using SCPackets.SendRoomChatMessage;
using SCPackets.UpdateRoomData;
using Server.Management.HandlersAction;
using Server.Management.Singleton;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Network.Packets;
using SCPackets.Buffers;

namespace Server.Management
{
    public class ServerPacketsToHandleList
    {
        private readonly ServerContext _context;
        private List<IHandlerModel> Handlers { get; set; } = new List<IHandlerModel>();

        public ServerPacketsToHandleList()
        {
            try
            {
                _context = new ServerContext();
                var user = _context.Users.FirstOrDefault(); //Will create entire EF structure at start
                InitializeRooms();

                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomAfterCreationNewRoom;

                var login = new ServerLoginAction(_context);
                var register = new ServerRegisterAction(_context);
                var createRoom = new ServerCreateRoomAction(_context);
                var updateRoom = new ServerUpdateRoomAction(_context);
                var disconnect = new ServerDisconnectAction(_context);
                var sendRoomChatMessage = new ServerSendRoomChatMessageAction(_context);
                var connectToRoom = new ServerConnectToRoomAction(_context);

                Handlers.Add(new HandlerModel<LoginRequest>(login.Action));
                Handlers.Add(new HandlerModel<RegisterRequest>(register.Action));
                Handlers.Add(new HandlerModel<CreateRoomRequest>(createRoom.Action));
                Handlers.Add(new HandlerModel<UpdateRoomDataRequest>(updateRoom.Action));
                Handlers.Add(new HandlerModel<DisconnectRequest>(disconnect.Action));
                Handlers.Add(new HandlerModel<SendRoomChatMessageRequest>(sendRoomChatMessage.Action));
                Handlers.Add(new HandlerModel<ConnectToRoomRequest>(connectToRoom.Action));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RegisterPackets(Connection conn)
        {
            Handlers.ForEach(x=>x.RegisterPacket(conn));
        }

        private void RoomAfterCreationNewRoom(object sender, ListWrapper<RoomInstance>.AfterAddEventArgs e)
        {
            BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(e.Item.Id);

            var squareRoomBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareRoomBuffer.InsertRooms.Add(e.Item.ToRoomOutsideModel());
        }

        public void InitializeRooms()
        {
            foreach (var room in _context.Rooms)
            {
                RoomSingleton.Instance.RoomInstances.Add(room.ToRoomInstance());
                BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(room.Id);
            }
        }
    }
}
