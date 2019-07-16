using Network;
using SCPackets.LoginPacket;
using SCPackets.LoginPacket.Container;
using Server.Management.Singleton;
using Server.Models;
using Server.Security;
using System;
using System.Data.Entity;
using System.Linq;

namespace Server.Management.HandlersAction
{
    public class ServerLoginAction
    {
        private readonly ServerContext _context;

        public ServerLoginAction(ServerContext context)
        {
            _context = context;
        }

        public void Request(LoginRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                //Check if is logged in (as connection)
                var isActive = ConnectionExtension.GetClient(conn);
                if (isActive != null)
                {
                    ext.SendPacket(new LoginResponse(Result.AlreadyLoggedError, req));
                    return;
                }

                //Check if login/email exists in DB
                User user = _context.Users.Include(x => x.UserAuth).
                    FirstOrDefault(x => (x.UserAuth.Login.Equals(req.Login)) || (x.Email.Equals(req.Login)));
                if (user == null)
                {
                    ext.SendPacket(new LoginResponse(Result.Error, req));
                    return;
                }

                
                isActive = ClientSingleton.Instance.Users.GetList().FirstOrDefault(x => x.User.Id == user.Id);
                if (isActive != null)
                {
                    ext.SendPacket(new LoginResponse(Result.AlreadyLogged, req));
                    return;
                }

                string hashedPass = Scrypt.Hash(req.Password, user.UserAuth.Salt);
                if (user.UserAuth.Hash.Equals(hashedPass))
                {
                    var response = new LoginResponse(Result.Success, req);
                    response.User = user.ToUserClient();

                    //Pull all rooms
                    foreach (var roomModel in RoomSingleton.Instance.RoomInstances.GetList())
                        response.RoomOutsideModelList.Add(roomModel.ToRoomOutsideModel());

                    //Pull his rooms
                    var userRooms = _context.Rooms.Include(x=>x.RoomConfig)
                        .Where(x => x.Author.Id.Equals(user.Id));
                    foreach (var room in userRooms)
                        response.UserRoomList.Add(room.ToRoomModel());

                    ext.SendPacket(response);
                    ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));
                    Console.WriteLine("Login: {0}", user);
                }
                else
                {
                    ext.SendPacket(new LoginResponse(Result.CredentialsError, req));
                }
            }
            catch (Exception e)
            {
                ext.SendPacket(new LoginResponse(Result.Error, req));
                Console.WriteLine(e.Message);
            }
        }
    }
}
