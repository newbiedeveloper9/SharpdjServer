using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using Network.Packets;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Application.Handlers.CoR;

namespace SharpDj.Server.Application.Handlers
{
    public abstract class RequestHandler<TReq> : AbstractHandler, IAction
        where TReq : RequestPacket
    {
        private readonly PacketReceivedHandler<TReq> _packetHandler;

        protected RequestHandler(IDictionaryConverter<IActionBag> bagConverter) 
            : base(bagConverter)
        {
            _packetHandler = async (packet, connection) =>
                await Handle(packet, new List<IActionBag> { new ConnectionBag(connection) }).ConfigureAwait(false);
        }

        public override async Task<object> Handle(object request, List<IActionBag> actionBags)
        {
            var connection = BagConverter.Get<ConnectionBag>(actionBags).Connection;

            await Action(request as TReq, connection, actionBags)
                .ConfigureAwait(false);

            return base.Handle(request, actionBags);
        }

        protected abstract Task Action(TReq request, Connection conn, List<IActionBag> actionBags);

        public void RegisterPacket(Connection conn)
        {
            conn.RegisterStaticPacketHandler(_packetHandler);
        }
    }
}