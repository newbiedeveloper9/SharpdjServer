using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Network;
using Network.Interfaces;
using Network.Packets;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Commands.Bags;

namespace SharpDj.Server.Application.Handlers
{
    public class RequestHandler<TReq> : IPacketRegister
        where TReq : RequestPacket
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PacketReceivedHandler<TReq> _packetHandler;

        public RequestHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _packetHandler = async (packet, connection) =>
                await Handle(packet, new List<IActionBag> { new ConnectionBag(connection) }).ConfigureAwait(false);
        }

        private async Task Handle(object request, List<IActionBag> actionBags)
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = _serviceProvider.GetRequiredService<IAction<TReq>>();

            var pipeline =  handler.BuildPipeline();
            await pipeline.Handle(request, actionBags)
                .ConfigureAwait(false);
        }

        public void RegisterPacket(Connection conn)
        {
            conn.RegisterStaticPacketHandler(_packetHandler);
        }
    }
}