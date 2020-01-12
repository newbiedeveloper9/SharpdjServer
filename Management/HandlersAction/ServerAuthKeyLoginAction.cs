using System;
using System.Data.Entity;
using System.Linq;
using Network;
using NLog;
using NLog.Fluent;
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

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    Logger.Info($"Validation failed. {(Result)validate}");
                    ext.SendPacket(new AuthKeyLoginResponse((Result)validate, request));
                    return;
                }
                #endregion Validation

                //todo if active then edit object, else create new like below. same for login
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

                //Login Success, filling data
                var response = new AuthKeyLoginResponse(Result.Success, request);
                response.Data.FillData(user, _context);

                ext.SendPacket(response);
                Logger.Info("Success");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ext.SendPacket(new AuthKeyLoginResponse(Result.Error, request));
            }
        }
    }
}