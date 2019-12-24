using Network;
using SCPackets.Disconnect;
using Server.Management.Singleton;
using Server.Models;
using System;
using System.Data.Entity;
using System.Linq;
using NLog;

namespace Server.Management.HandlersAction
{
    public class ServerDisconnectAction
    {
        private readonly ServerContext _context;

        public ServerDisconnectAction(ServerContext context)
        {
            _context = context;
        }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
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
                        Logger.Info("User disconnected");

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
                Logger.Info($"Status: {response.Result}");
            }
            catch (Exception e)
            {
                ext.SendPacket(new DisconnectResponse(Result.Error, request));
                Console.WriteLine(e);
            }
        }

        public void Action(DisconnectRequest request, Connection connection)
        {
            Action(request, connection, false);
        }
    }
}
