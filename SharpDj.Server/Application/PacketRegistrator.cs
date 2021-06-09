using System.Collections.Generic;
using Autofac;
using Network;
using SCPackets.Packets.Login;
using SCPackets.Packets.Register;
using SharpDj.Common.Handlers.Base;
using SharpDj.Server.Application.Commands.Handlers;

namespace SharpDj.Server.Application
{
    public class PacketRegistrator : IPacketRegistrator
    {
        private readonly IEnumerable<IPacketRegister> _actionRegisterList;

        public PacketRegistrator(IComponentContext componentContext)
        {
            _actionRegisterList = new List<IPacketRegister>()
            {
                componentContext.Resolve<RequestHandler<LoginRequest>>(),
                componentContext.Resolve<RequestHandler<RegisterRequest>>(),
            };
        }

        public void ConnectionPacketsRegister(Connection connection)
        {
            foreach (var action in _actionRegisterList)
            {
                action.RegisterPacket(connection);
            }
        }
    }
}
