using Network;
using SCPackets;
using SCPackets.RegisterPacket;
using Server.Models;
using Server.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Server.Management.HandlersAction
{
    class ServerRegisterAction
    {
        private readonly ServerContext _context;
        public ServerRegisterAction(ServerContext context)
        {
            _context = context;
        }

        public void Request(RegisterRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.PasswordError, (req.Password.Length < 6 || req.Password.Length > 48));
                validation.Conditions.Add(Result.EmailError, !DataValidation.EmailIsValid(req.Email));
                validation.Conditions.Add(Result.LoginError, !DataValidation.LengthIsValid(req.Login, 2, 32));
                validation.Conditions.Add(Result.UsernameError, !DataValidation.LengthIsValid(req.Username, 2, 32));
                validation.Conditions.Add(Result.AlreadyExist, AccountExist(req.Login, req.Email));

                var result = validation.Validate();
                if (result != null)
                {
                    ext.SendPacket(new RegisterResponse((Result) result, req));
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

                ext.SendPacket(new RegisterResponse(Result.Success, req));
                Console.WriteLine("Register: {0}", user);
            }
            catch (Exception e)
            {
                ext.SendPacket(new RegisterResponse(Result.Error, req));
                Console.WriteLine(e.Message);
            }
        }

        private bool AccountExist(string login, string email)
        {
            return _context.Users.Include(x => x.UserAuth)
                        .Any(x => x.UserAuth.Login.Equals(login)) ||
                    _context.Users.Any(x => x.Email.Equals(email));
        }

    }
}
