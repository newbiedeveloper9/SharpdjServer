using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Network;
using Network.Interfaces;
using Network.Packets;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Commands.Bags;

namespace SharpDj.Server.Application.Commands.Handlers
{
    public class RequestHandler<TReq> : IAction
        where TReq : RequestPacket
    {
        private readonly IDictionaryConverter<IActionBag> _bagConverter;
        private readonly IServiceProvider _serviceProvider;
        private readonly PacketReceivedHandler<TReq> _packetHandler;

        public RequestHandler(IDictionaryConverter<IActionBag> bagConverter, IServiceProvider serviceProvider)
        {
            _bagConverter = bagConverter;
            _serviceProvider = serviceProvider;

            _packetHandler = async (packet, connection) =>
                await Handle(packet, new List<IActionBag> { new ConnectionBag(connection) }).ConfigureAwait(false);
        }

        private async Task Handle(object request, List<IActionBag> actionBags)
        {
            var connection = _bagConverter.Get<ConnectionBag>(actionBags).Connection;
            using var scope = _serviceProvider.CreateScope();

            var handler = _serviceProvider.GetRequiredService<IAction<TReq>>();
            
            handler.Pipeline.SetNext()
        }

        public void RegisterPacket(Connection conn)
        {
            conn.RegisterStaticPacketHandler(_packetHandler);
        }
    }
}