using Network;
using SCPackets;
using SCPackets.CreateRoom;
using SCPackets.Disconnect;
using SCPackets.LoginPacket;
using SCPackets.NewRoomCreated;
using SCPackets.RegisterPacket;
using SCPackets.UpdateRoomData;
using Server.Management.HandlersAction;
using Server.Management.Singleton;
using Server.Models;
using System;
using System.Linq;

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


        public ServerPacketsToHandleList()
        {
            try
            {
                _context = new ServerContext();
                User user = _context.Users.FirstOrDefault(); //Will create entire EF structure at start
                InitializeRooms();

                RoomSingleton.Instance.Rooms.AfterInsert += RoomAfterCreationNewRoom;

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
        }

        private void RoomAfterCreationNewRoom(object sender, SpecialRoomList<RoomInstance>.SpecialRoomArgs e)
        {
            foreach (var user in ClientSingleton.Instance.Users)
            {
                user.Connection.Send(
                    new NewRoomCreatedRequest(e.Item.ToRoomOutsideModel()));
            }
        }

        public void InitializeRooms()
        {
            foreach (var room in _context.Rooms)
            {
                RoomSingleton.Instance.Rooms.Add(room.ToRoomInstance());
            }
        }
    }
}
