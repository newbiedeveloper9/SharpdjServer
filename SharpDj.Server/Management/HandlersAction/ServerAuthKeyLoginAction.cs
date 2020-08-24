using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using Network.Interfaces;
using Network.Packets;
using SCPackets.Packets.AuthKeyLogin;
using SharpDj.Domain.Entity;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

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
                var loggedInUser = ConnectionExtension.GetClient(conn);
                if (loggedInUser != null)
                {
                    ext.SendPacket(new AuthKeyLoginResponse(AuthKeyLoginResult.AlreadyLogged, request));
                    return;
                }

                var userEntity = _context.Users
                    .Include(x => x.UserAuth).
                    FirstOrDefault(x => x.UserAuth.AuthenticationKey.Equals(request.AuthenticationKey));

                loggedInUser = ClientSingleton.Instance.Users
                    .GetList()
                    .FirstOrDefault(x => x.User.Id == userEntity?.Id);

                if (!Validate(ext, userEntity, request))
                    return;

                if (loggedInUser != null)
                {
                    ClientSingleton.Instance.Users
                        .GetList()
                        .FirstOrDefault(x => x.User.Id == userEntity.Id)
                        .Connections.Add(conn);
                }
                else
                {
                    ClientSingleton.Instance.Users.Add(new ServerUserModel(userEntity, conn));
                }

                var response = new AuthKeyLoginResponse(AuthKeyLoginResult.Success, request);
                response.Data.FillData(userEntity, _context);

                ext.SendPacket(response);
                Log.Information("Success login by authKey");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new AuthKeyLoginResponse(AuthKeyLoginResult.Error, request));
            }
        }

        private bool Validate(ConnectionExtension ext, User userEntity, AuthKeyLoginRequest request)
        {
            var validation = new DictionaryConditionsValidation<AuthKeyLoginResult>();
            var expiration = userEntity?.UserAuth.AuthenticationExpiration;

            validation.Conditions.Add(AuthKeyLoginResult.Error, userEntity == null);
            // validation.Conditions.Add(LoginResult.AlreadyLogged, active != null);
            validation.Conditions.Add(AuthKeyLoginResult.Expired, expiration < DateTime.Now);

            var validate = validation.AnyError();
            if (validate != null)
            {
                Log.Information("Validation has failed. {@LoginResult}", (AuthKeyLoginResult)validate);
                ext.SendPacket(new AuthKeyLoginResponse((AuthKeyLoginResult)validate, request));
                return false;
            }

            return true;
        }
    }
}