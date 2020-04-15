using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.LoginPacket;
using SCPackets.LoginPacket.Container;
using SCPackets.Models;
using SharpDj.Server.Entity;
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
                    Log.Information($"Given connection is already logged in to account {connectionIsActive.User.Username}");
                    ext.SendPacket(new LoginResponse(Result.AlreadyLoggedError, req));
                    return;
                }

                //Check if login/email exists in DB
                User user = _context.Users
                    .Include(x => x.UserAuth)
                    .FirstOrDefault(x => (x.UserAuth.Login.Equals(req.Login)) || (x.Email.Equals(req.Login)));
                if (user == null)
                {
                    Log.Information("User with given credentials doesn't exists");
                    ext.SendPacket(new LoginResponse(Result.Error, req));
                    return;
                }

                var userIsActive = ClientSingleton.Instance.Users
                    .GetList()
                    .FirstOrDefault(x => x.User.Id == user.Id);
                // if (userIsActive != null)
                // {
                //     Logger.Info("User is already active");
                //     ext.SendPacket(new LoginResponse(Result.AlreadyLogged, req));
                //     return;
                // }

                string hashedPass = Scrypt.Hash(req.Password, user.UserAuth.Salt);
                if (user.UserAuth.Hash.Equals(hashedPass))
                {
                    if (userIsActive != null)
                        ClientSingleton.Instance.Users
                            .GetList()
                            .FirstOrDefault(x => x.User.Id == user.Id)
                            .Connections.Add(conn);
                    else
                        ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));

                    var response = new LoginResponse(Result.Success, req);
                    response.Data.FillData(user, _context);

                    if (req.RememberMe)
                    {
                        var authKey = Scrypt.GenerateSalt();
                        user.UserAuth.AuthenticationKey = authKey;
                        user.UserAuth.AuthenticationExpiration = DateTime.Now.AddDays(30);
                        await _context.SaveChangesAsync().ConfigureAwait(false);

                        response.AuthenticationKey = authKey;
                    }

                    ext.SendPacket(response);
                    Log.Information("Success login: {@User}", user.ToString());
                }
                else
                {
                    ext.SendPacket(new LoginResponse(Result.CredentialsError, req));
                    Log.Information("An error has occurred");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new LoginResponse(Result.Error, req));
            }
        }
    }

    public static class LoginHelper
    {
        public static void FillData(this LoginDataModel data, User user, ServerContext _context)
        {
            data.User = user.ToUserClient();

            //Pull all rooms
            foreach (var roomModel in RoomSingleton.Instance.RoomInstances.GetList())
                data.RoomOutsideModelList.Add(roomModel.ToRoomOutsideModel());

            //Pull his rooms
            var userRooms = _context.Rooms.Include(x => x.RoomConfig)
                .Where(x => x.Author.Id.Equals(user.Id));

            foreach (var room in userRooms)
                data.UserRoomList.Add(room.ToRoomModel());
        }
    }
}
