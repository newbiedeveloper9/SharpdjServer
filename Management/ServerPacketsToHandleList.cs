using Network;
using SCPackets;
using SCPackets.ConnectToRoom;
using SCPackets.CreateRoom;
using SCPackets.Disconnect;
using SCPackets.LoginPacket;
using SCPackets.NewRoomCreated;
using SCPackets.RegisterPacket;
using SCPackets.SendRoomChatMessage;
using SCPackets.UpdateRoomData;
using Server.Management.HandlersAction;
using Server.Management.Singleton;
using Server.Models;
using System;
using System.Linq;
using SCPackets.Buffers;

namespace Server.Management
{
    public class ServerPacketsToHandleList
    {
        private readonly ServerContext _context;

        public HandlerModel<LoginRequest> Login { get; set; }
        public HandlerModel<RegisterRequest> Register { get; set; }
        public HandlerModel<CreateRoomRequest> CreateRoom { get; set; }
        public HandlerModel<UpdateRoomDataRequest> UpdateRoom { get; set; }
        public HandlerModel<DisconnectRequest> Disconnect { get; set; }
        public HandlerModel<SendRoomChatMessageRequest> SendRoomChatMessage { get; set; }
        public HandlerModel<ConnectToRoomRequest> ConnectToRoom { get; set; }


        public ServerPacketsToHandleList()
        {
            try
            {
                _context = new ServerContext();
                User user = _context.Users.FirstOrDefault(); //Will create entire EF structure at start
                InitializeRooms();

                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomAfterCreationNewRoom;

                Login = new HandlerModel<LoginRequest>
                { Action = new ServerLoginAction(_context).Request };
                Register = new HandlerModel<RegisterRequest>
                { Action = new ServerRegisterAction(_context).Request };
                CreateRoom = new HandlerModel<CreateRoomRequest>()
                { Action = new ServerCreateRoomAction(_context).Action };
                UpdateRoom = new HandlerModel<UpdateRoomDataRequest>
                { Action = new ServerUpdateRoomAction(_context).Action };
                Disconnect = new HandlerModel<DisconnectRequest>
                { Action = new ServerDisconnectAction(_context).Action };
                SendRoomChatMessage = new HandlerModel<SendRoomChatMessageRequest>
                { Action = new ServerSendRoomChatMessageAction(_context).Action };
                ConnectToRoom = new HandlerModel<ConnectToRoomRequest>
                { Action = new ServerConnectToRoomAction(_context).Action };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RegisterPackets(Connection conn)
        {
            Login.RegisterPacket(conn);
            Register.RegisterPacket(conn);
            CreateRoom.RegisterPacket(conn);
            UpdateRoom.RegisterPacket(conn);
            Disconnect.RegisterPacket(conn);
            SendRoomChatMessage.RegisterPacket(conn);
            ConnectToRoom.RegisterPacket(conn);
        }

        private void RoomAfterCreationNewRoom(object sender, ListWrapper<RoomInstance>.AfterAddEventArgs<RoomInstance> e)
        {
            if (!(e.Item is RoomInstance))
                throw new Exception("item is not of room type");
            var roomInstance = (RoomInstance) e.Item;

            BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(roomInstance.Id);

            foreach (var user in ClientSingleton.Instance.Users.GetList())
            {
                user.Connection.Send(
                    new NewRoomCreatedRequest(roomInstance.ToRoomOutsideModel()));
            }
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
