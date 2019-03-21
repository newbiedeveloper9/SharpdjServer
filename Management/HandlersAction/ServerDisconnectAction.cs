using Network;
using SCPackets.Disconnect;
using Server.Management.Singleton;
using Server.Models;
using System;
using System.Linq;

namespace Server.Management.HandlersAction
{
    public class ServerDisconnectAction
    {
        private readonly ServerContext _context;

        public ServerDisconnectAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(DisconnectRequest request, Connection connection)
        {
            var ext = new ConnectionExtension(connection, this);

            try
            {
                var user = ConnectionExtension.GetClient(connection);
                var isActive = ClientSingleton.Instance.Users.FirstOrDefault(x => x.User.Id == user.User.Id);
                if (isActive != null)
                {
                    var removed = ClientSingleton.Instance.Users.Remove(isActive);
                    if (removed)
                    {
                        ext.SendPacket(new DisconnectResponse(Result.Success, request));
                        return;
                    }
                }

                var response = ClientSingleton.Instance.Users.Remove(user)
                    ? new DisconnectResponse(Result.Success, request)
                    : new DisconnectResponse(Result.Error, request);

                ext.SendPacket(response);
            }
            catch (Exception e)
            {
                ext.SendPacket(new DisconnectResponse(Result.Error, request));
                Console.WriteLine(e);
            }
        }
    }
}
