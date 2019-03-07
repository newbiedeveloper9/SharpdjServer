using Network;
using Network.Interfaces;
using SCPackets.LoginPacket;
using System;
using SCPackets;

namespace Server.Management
{
    public class ServerPacketsToHandleList
    {
        public HandlerModel<LoginRequest> Login { get; set; }

        public ServerPacketsToHandleList()
        {
            Login = new HandlerModel<LoginRequest> { Action = new HandlersAction.LoginAction().Action };
        }

        public void Register(Connection conn)
        {
            Login.RegisterPacket(conn);
        }
    }
}
