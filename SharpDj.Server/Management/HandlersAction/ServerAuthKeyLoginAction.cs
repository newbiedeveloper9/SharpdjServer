using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using Network.Interfaces;
using SCPackets.AuthKeyLogin;
using SCPackets.RegisterPacket;
using SharpDj.Server.Entity;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;
using Result = SCPackets.AuthKeyLogin.Result;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerAuthKeyLoginAction : ActionAbstract<AuthKeyLoginRequest>
    {
        private ServerContext _context;

        public ServerAuthKeyLoginAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(AuthKeyLoginRequest request, Connection conn)
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

                var user = _context.Users
                    .Include(x => x.UserAuth).
                    FirstOrDefault(x => x.UserAuth.AuthenticationKey.Equals(request.AuthenticationKey));

                active = ClientSingleton.Instance.Users
                    .GetList()
                    .FirstOrDefault(x => x.User.Id == user?.Id);

                var expiration = user?.UserAuth.AuthenticationExpiration;

                #region Validation
                var validation = new DictionaryConditionsValidation<Result>();
                validation.Conditions.Add(Result.Error, user == null);
               // validation.Conditions.Add(Result.AlreadyLogged, active != null);
                validation.Conditions.Add(Result.Expired, expiration < DateTime.Now);

                var validate = validation.AnyError();
                if (validate != null)
                {
                    Log.Information("Validation has failed. {@Result}",  (Result)validate);
                    ext.SendPacket(new AuthKeyLoginResponse((Result)validate, request));
                    return;
                }
                #endregion Validation

                if (active != null)
                {
                    ClientSingleton.Instance.Users
                        .GetList()
                        .FirstOrDefault(x => x.User.Id == user.Id)
                        .Connections.Add(conn);
                }
                else
                {
                    ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));
                }

                var response = new AuthKeyLoginResponse(Result.Success, request);
                response.Data.FillData(user, _context);

                ext.SendPacket(response);
                Log.Information("Success login by authKey");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new AuthKeyLoginResponse(Result.Error, request));
            }
        }
    }
}