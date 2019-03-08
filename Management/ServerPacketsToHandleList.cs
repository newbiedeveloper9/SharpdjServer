using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using SCPackets;
using SCPackets.LoginPacket;
using SCPackets.RegisterPacket;
using Server.Models;

namespace Server.Management
{
    public class ServerPacketsToHandleList
    {
        private UserContext context;

        public HandlerModel<LoginRequest> Login { get; set; }
        public HandlerModel<RegisterRequest> Register { get; set; }

        public ServerPacketsToHandleList()
        {

                context = new UserContext();
                User user = context.Users.FirstOrDefault(); //Will create entire EF structure at start

                Login = new HandlerModel<LoginRequest> {Action = new HandlersAction.ServerLoginAction(context).Request};
                Register = new HandlerModel<RegisterRequest> {Action = new HandlersAction.ServerRegisterAction(context).Request};
        }

        public void RegisterPackets(Connection conn)
        {
            Login.RegisterPacket(conn);
            Register.RegisterPacket(conn);
        }
    }
}
