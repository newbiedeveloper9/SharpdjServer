using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Disconnect;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models.EF;
using System;
using System.Linq;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerDisconnectAction
    {
        private readonly ServerContext _context;

        public ServerDisconnectAction(ServerContext context)
        {
            _context = context;
        }
        public void Action(DisconnectRequest request, Connection connection, bool forced = false)
        {
            var ext = new ConnectionExtension(connection, this);

            try
            {
                var user = ConnectionExtension.GetClient(connection);
                var userContext = _context.Users.Include(x => x.UserAuth).FirstOrDefault(x => x.Id == user.User.Id);
                var isActive = ClientSingleton.Instance.Users.GetList().FirstOrDefault(x => x.User.Id == user.User.Id);
                if (isActive != null)
                {
                    var removed = ClientSingleton.Instance.Users.Remove(isActive);
                    if (removed)
                    {
                        if (!forced)
                            userContext?.UserAuth.ClearAuthKey(_context);
                        Log.Information("{@User} has disconnected",
                            user.User.Username);

                        ext.SendPacket(new DisconnectResponse(Result.Success, request));
                        return;
                    }
                }

                var response = ClientSingleton.Instance.Users.Remove(user)
                    ? new DisconnectResponse(Result.Success, request)
                    : new DisconnectResponse(Result.Error, request);

                if (response.Result == Result.Success && !forced)
                    userContext?.UserAuth.ClearAuthKey(_context);

                ext.SendPacket(response);
                Log.Information("Status: {@Result}", response.Result);
            }
            catch (Exception e)
            {
                ext.SendPacket(new DisconnectResponse(Result.Error, request));
                Log.Error(e.StackTrace);
            }
        }

        public void Action(DisconnectRequest request, Connection connection)
        {
            Action(request, connection, false);
        }
    }
}
