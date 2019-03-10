using Network;
using SCPackets.LoginPacket;
using Server.Models;
using Server.Security;
using System;
using System.Data.Entity;
using System.Linq;

namespace Server.Management.HandlersAction
{
    public class ServerLoginAction
    {
        private readonly ServerContext _context;

        public ServerLoginAction(ServerContext context)
        {
            _context = context;
        }

        public void Request(LoginRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var isActive = ConnectionExtension.GetClient(conn);
                if (isActive != null)
                {
                    ext.SendPacket(new LoginResponse(Result.AlreadyLogged, req));
                    return;
                }

                User user = _context.Users.Include(x=>x.UserAuth).
                    FirstOrDefault(x => (x.UserAuth.Login.Equals(req.Login)) || (x.Email.Equals(req.Login)));
                if (user == null)
                {
                    ext.SendPacket(new LoginResponse(Result.Error, req));
                    return;
                }

                string hashedPass = Scrypt.Hash(req.Password, user.UserAuth.Salt);
                if (user.UserAuth.Hash.Equals(hashedPass))
                {
                    ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));
                    ext.SendPacket(new LoginResponse(Result.Success, req));
                    Console.WriteLine("Login: {0}", user);
                }
                else
                {
                    ext.SendPacket(new LoginResponse(Result.Error, req));
                }
            }
            catch (Exception e)
            {
                ext.SendPacket(new LoginResponse(Result.Error, req));
                Console.WriteLine(e.Message);
            }
        }
    }
}
