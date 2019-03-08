using Network;
using SCPackets.RegisterPacket;
using Server.Models;
using Server.Security;
using System;
using System.Data.Entity;
using System.Linq;

namespace Server.Management.HandlersAction
{
    class ServerRegisterAction
    {
        private readonly UserContext _context;
        public ServerRegisterAction(UserContext context)
        {
            _context = context;
        }

        public void Request(RegisterRequest req, Connection conn)
        {
            Console.WriteLine(req.Password);

            try
            {
                if (_context.Users.Include(x=>x.UserAuth)
                        .Any(x => x.UserAuth.Login.Equals(req.Login)) || _context.Users.Any(x => x.Email.Equals(req.Email)))
                {
                    conn.Send(new RegisterResponse(Result.AlreadyExist, req));
                    return;
                }

                if (req.Password.Length < 6 || req.Password.Length > 48)
                {
                    ConnectionExtension.SendPacket(conn, new RegisterResponse(Result.PasswordError, req), this);
                    return;
                }

                string salt = Scrypt.GenerateSalt();
                var user = new User()
                {
                    Email = req.Email,
                    UserAuth = new UserAuth()
                    {
                        Salt = salt,
                        Hash = Scrypt.Hash(req.Password, salt),
                        Login = req.Login,
                    },
                    Username = req.Username
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                ConnectionExtension.SendPacket(conn, new RegisterResponse(Result.Success, req), this);
                Console.WriteLine("Register: {0}", user);
            }
            catch (Exception e)
            {
                ConnectionExtension.SendPacket(conn, new RegisterResponse(Result.Error, req), this);
                Console.WriteLine(e.Message);
            }
        }
    }
}
