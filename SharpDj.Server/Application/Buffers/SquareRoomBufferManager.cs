using System.Linq;
using SCPackets.Packets.Buffers;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Extensions;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Management.Buffers
{
    public class SquareRoomBufferManager : BufferManager<SquareRoomBufferRequest>
    {
        public SquareRoomBufferManager() : base(10_000)
        {
            CreateBuffer();
        }

        protected override void ClearBuffer(ActionBuffer<SquareRoomBufferRequest> actionBuffer)
        {
            actionBuffer.RequestPacket = new SquareRoomBufferRequest();
        }

        public SquareRoomBufferRequest GetRequest() => Buffers.FirstOrDefault()?.RequestPacket;

        public sealed override void CreateBuffer(dynamic obj = null)
        {
            var request = new SquareRoomBufferRequest();
            var actionBuffer = new ActionBuffer<SquareRoomBufferRequest>(request);

            actionBuffer.BeforeSendBuffer +=
                (sender, args) =>
                {
                    actionBuffer.Connections = ClientSingleton.Instance.Users.GetList().GetAllConnections();

                    var packet = actionBuffer.RequestPacket;
                    actionBuffer.CanSend = packet.InsertRooms.Any() ||
                                           packet.RemoveRooms.Any() ||
                                           packet.UpdatedRooms.Any();
                };

            Buffers.Add(actionBuffer);
        }
    }
}
