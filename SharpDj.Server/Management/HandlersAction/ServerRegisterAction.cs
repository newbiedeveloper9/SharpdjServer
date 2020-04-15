using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets;
using SCPackets.RegisterPacket;
using SharpDj.Server.Entity;
using SharpDj.Server.Security;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    class ServerRegisterAction : ActionAbstract<RegisterRequest>
    {
        private readonly ServerContext _context;

        public ServerRegisterAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(RegisterRequest req, Connection conn)
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

                var result = validation.AnyError();
                if (result != null)
                {
                    Log.Information("User with given credentials already exist");
                    ext.SendPacket(new RegisterResponse((Result)result, req));
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
                await _context.SaveChangesAsync();

                ext.SendPacket(new RegisterResponse(Result.Success, req));
                Log.Information("Success register: {@User}", user.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new RegisterResponse(Result.Error, req));
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
