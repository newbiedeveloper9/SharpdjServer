using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SCPackets.NotLoggedIn;
using SCPackets.SendRoomChatMessage;
using Server.Models;

namespace Server.Management.HandlersAction
{
    public class ServerSendRoomChatMessageAction
    {
        private ServerContext _context;

        public ServerSendRoomChatMessageAction(ServerContext context)
        {
            _context = context;
        }

        public void Action(SendRoomChatMessageRequest request, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (active == null)
                {
                    conn.Send(new NotLoggedInRequest());
                    return;
                }

                //TODO: Sending message to user active room chat
                throw new Exception("Todo sending message to user active room chat");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ext.SendPacket(new SendRoomChatMessageResponse(Result.Error, request));
            }
        }
    }
}
