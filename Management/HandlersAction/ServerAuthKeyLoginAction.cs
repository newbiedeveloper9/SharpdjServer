using System;
using System.Data.Entity;
using System.Linq;
using Network;
using SCPackets.AuthKeyLogin;
using Server.Management.Singleton;
using Server.Models;

namespace Server.Management.HandlersAction
{
    public class ServerAuthKeyLoginAction
    {
        private ServerContext _context;

        public ServerAuthKeyLoginAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(AuthKeyLoginRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (active != null)
                {
                    ext.SendPacket(new AuthKeyLoginResponse(Result.AlreadyLogged, request));
                    return;
                }

                User user = _context.Users.Include(x => x.UserAuth).
                    FirstOrDefault(x => x.UserAuth.AuthenticationKey.Equals(request.AuthenticationKey));

                var tmp = _context.Users.Include(x=>x.UserAuth).FirstOrDefault(x=>x.Username=="qwerty");
                Console.WriteLine(tmp.UserAuth.AuthenticationKey);

                active = ClientSingleton.Instance.Users.GetList().FirstOrDefault(x => x.User.Id == user?.Id);

                var expiration = user?.UserAuth.AuthenticationExpiration;

                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, user == null);
                validation.Conditions.Add(Result.AlreadyLogged, active != null);
                validation.Conditions.Add(Result.Expired, expiration < DateTime.Now);

                var validate = validation.Validate();
                if (validate != null)
                {
                    ext.SendPacket(new AuthKeyLoginResponse((Result)validate, request));
                    return;
                }

                //Login Success, filling data
                var response = new AuthKeyLoginResponse(Result.Success, request);
                response.Data.FillData(user, _context);

                ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));

                ext.SendPacket(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ext.SendPacket(new AuthKeyLoginResponse(Result.Error, request));
            }
        }
    }
}