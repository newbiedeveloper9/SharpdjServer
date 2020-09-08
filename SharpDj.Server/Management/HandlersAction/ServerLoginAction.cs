using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Models;
using SCPackets.Packets.Login;
using SharpDj.Domain.Entity;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using SharpDj.Server.Security;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerLoginAction : ActionAbstract<LoginRequest>
    {
        private readonly ServerContext _context;

        public ServerLoginAction(ServerContext context)
        {
            _context = context;
        }
        public override async Task Action(LoginRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                //Check if is logged in (as connection)
                var connectionIsActive = ConnectionExtension.GetClient(conn);
                if (connectionIsActive != null)
                {
                    Log.Information($"Given connection is already logged in to account {connectionIsActive.UserEntity.Username}");
                    ext.SendPacket(new LoginResponse(LoginResult.AlreadyLoggedError, req));
                    return;
                }

                //Check if login/email exists in DB
                UserEntity userEntity = _context.Users
                    .Include(x => x.UserAuthEntity)
                    .FirstOrDefault(x => (x.UserAuthEntity.Login.Equals(req.Login)) || (x.Email.Equals(req.Login)));
                if (userEntity == null)
                {
                    Log.Information("UserEntity with given credentials doesn't exists");
                    ext.SendPacket(new LoginResponse(LoginResult.Error, req));
                    return;
                }

                var userIsActive = ClientSingleton.Instance.Users
                    .GetList()
                    .FirstOrDefault(x => x.UserEntity.Id == userEntity.Id);
                // if (userIsActive != null)
                // {
                //     Logger.Info("UserEntity is already active");
                //     ext.SendPacket(new LoginResponse(LoginResult.AlreadyLogged, req));
                //     return;
                // }

                string hashedPass = Scrypt.Hash(req.Password, userEntity.UserAuthEntity.Salt);
                if (userEntity.UserAuthEntity.Hash.Equals(hashedPass))
                {
                    if (userIsActive != null)
                    {
                        ClientSingleton.Instance.Users
                            .GetList()
                            .FirstOrDefault(x => x.UserEntity.Id == userEntity.Id)
                            .Connections.Add(conn);
                    }
                    else
                    {
                        ClientSingleton.Instance.Users.Add(new ServerUserModel(userEntity, conn));
                    }

                    var response = new LoginResponse(LoginResult.Success, req);
                    response.Data.FillData(userEntity, _context);

                    if (req.RememberMe)
                    {
                        var authKey = Scrypt.GenerateSalt();
                        userEntity.UserAuthEntity.AuthenticationKey = authKey;
                        userEntity.UserAuthEntity.AuthenticationExpiration = DateTime.Now.AddDays(30);
                        await _context.SaveChangesAsync().ConfigureAwait(false);

                        response.AuthenticationKey = authKey;
                    }

                    ext.SendPacket(response);
                    Log.Information("Success login: {@UserEntity}", userEntity.ToString());
                }
                else
                {
                    ext.SendPacket(new LoginResponse(LoginResult.CredentialsError, req));
                    Log.Information("An error has occurred");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new LoginResponse(LoginResult.Error, req));
            }
        }
    }

    public static class LoginHelper
    {
        public static void FillData(this PreviewLogin data, UserEntity userEntity, ServerContext _context)
        {
            data.User = userEntity.ToUserClient();

            //Pull all rooms
            foreach (var roomModel in RoomSingleton.Instance.RoomInstances.GetList())
            {
                data.RoomOutsideModelList.Add(roomModel.ToRoomOutsideModel());
            }

            //Pull his rooms
            var userRooms = _context.Rooms
                .Include(x => x.ConfigEntity)
                .Where(x => x.Author.Id.Equals(userEntity.Id));

            foreach (var room in userRooms)
            {
                //TODO data.UserRoomList.Add(room.ToRoomModel());
            }
        }
    }
}
