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
        private ServerContext context;

        public HandlerModel<LoginRequest> Login { get; set; }
        public HandlerModel<RegisterRequest> Register { get; set; }
        public HandlerModel<CreateRoomRequest> CreateRoom { get; set; }

        public ServerPacketsToHandleList()
        {

                context = new ServerContext();
                User user = context.Users.FirstOrDefault(); //Will create entire EF structure at start

                Login = new HandlerModel<LoginRequest> {Action = new ServerLoginAction(context).Request};
                Register = new HandlerModel<RegisterRequest> {Action = new ServerRegisterAction(context).Request};
                CreateRoom = new HandlerModel<CreateRoomRequest>(){Action = new ServerCreateRoomAction(context).Action};
        }

        public void RegisterPackets(Connection conn)
        {
            Login.RegisterPacket(conn);
            Register.RegisterPacket(conn);
            CreateRoom.RegisterPacket(conn);
        }
    }
}
