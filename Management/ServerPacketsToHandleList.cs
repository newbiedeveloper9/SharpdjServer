using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using SCPackets;
using SCPackets.CreateRoom;
using SCPackets.LoginPacket;
using SCPackets.RegisterPacket;
using Server.Management.HandlersAction;
using Server.Models;

namespace Server.Management
{
    public class ServerPacketsToHandleList
    {
        private readonly ServerContext _context;

        public HandlerModel<LoginRequest> Login { get; set; }
        public HandlerModel<RegisterRequest> Register { get; set; }
        public HandlerModel<CreateRoomRequest> CreateRoom { get; set; }

        public ServerPacketsToHandleList()
        {
            try
            {
                _context = new ServerContext();
                User user = _context.Users.FirstOrDefault(); //Will create entire EF structure at start
                InitializeRooms();

                Login = new HandlerModel<LoginRequest> {Action = new ServerLoginAction(_context).Request};
                Register = new HandlerModel<RegisterRequest> {Action = new ServerRegisterAction(_context).Request};
                CreateRoom = new HandlerModel<CreateRoomRequest>()
                    {Action = new ServerCreateRoomAction(_context).Action};
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
