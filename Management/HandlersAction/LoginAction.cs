using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SCPackets.LoginPacket;

namespace Server.Management.HandlersAction
{
    public class LoginAction 
    {
        public void Action(LoginRequest req, Connection conn)
        {
            Console.WriteLine(req.Password);
            Console.WriteLine(req.Login);
            conn.Send(new LoginResponse(Result.Success, req), this);
        }
    }
}
