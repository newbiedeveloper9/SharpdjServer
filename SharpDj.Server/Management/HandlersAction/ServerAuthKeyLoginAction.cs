using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.AuthKeyLogin;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models;
using SharpDj.Server.Models.EF;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
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

                var validate = validation.Validate();
                if (validate != null)
                {
                    Log.Information("Validation failed. {@Result}",  (Result)validate);
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