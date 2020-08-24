using Microsoft.EntityFrameworkCore;
using Network;
using System;
using System.Linq;
using System.Threading.Tasks;
using SCPackets.Packets.Disconnect;
using SharpDj.Domain.Entity;
using SharpDj.Infrastructure;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerDisconnectAction : ActionAbstract<DisconnectRequest>
    {
        private readonly ServerContext _context;

        public ServerDisconnectAction(ServerContext context)
        {
            _context = context;
        }
        public async Task Action(DisconnectRequest request, Connection connection, bool forced = false)
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
                        {
                            ClearAuthKey(userContext?.UserAuth, _context);
                        }

                        Log.Information("{@User} has disconnected",
                            user.User.Username);

                        ext.SendPacket(new DisconnectResponse(DisconnectResult.Success, request));
                        return;
                    }
                }

                var response = ClientSingleton.Instance.Users.Remove(user)
                    ? new DisconnectResponse(DisconnectResult.Success, request)
                    : new DisconnectResponse(DisconnectResult.Error, request);

                if (response.Result == DisconnectResult.Success && !forced)
                {
                    ClearAuthKey(userContext?.UserAuth, _context);
                }

                ext.SendPacket(response);
                Log.Information("Status: {@LoginResult}", response.Result);
            }
            catch (Exception e)
            {
                ext.SendPacket(new DisconnectResponse(DisconnectResult.Error, request));
                Log.Error(e.StackTrace);
            }
        }

        public override async Task Action(DisconnectRequest request, Connection connection)
        {
            await Action(request, connection, false);
        }

        public void ClearAuthKey(UserAuth auth, ServerContext context)
        {
            auth.AuthenticationKey = string.Empty;
            auth.AuthenticationExpiration = DateTime.Now.AddYears(-20);
        }
    }
}
