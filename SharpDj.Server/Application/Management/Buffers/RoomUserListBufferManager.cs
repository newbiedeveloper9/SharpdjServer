using System.Collections.Generic;
using System.Linq;
using Network;
using SCPackets.Packets.Buffers;
using Serilog;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Management.Buffers
{
    public class RoomUserListBufferManager : BufferManager<RoomUserListBufferRequest>
    {
        public RoomUserListBufferManager() : base(5000)
        {

        }

        public ActionBuffer<RoomUserListBufferRequest> GetByRoomId(int roomId) =>
             Buffers.FirstOrDefault(x => x.RequestPacket.RoomId == roomId);

        protected override void ClearBuffer(ActionBuffer<RoomUserListBufferRequest> actionBuffer)
        {
            var roomId = actionBuffer.RequestPacket.RoomId;
            actionBuffer.RequestPacket = new RoomUserListBufferRequest(roomId);
        }

        public override void CreateBuffer(dynamic roomId)
        {
            var request = new RoomUserListBufferRequest(roomId);
            var buffer = new ActionBuffer<RoomUserListBufferRequest>(request);
            buffer.BeforeSendBuffer += (sender, args) =>
            {
                var roomInstance = RoomSingleton.Instance.RoomInstances
                    .FirstOrDefault(x => x.Id == roomId);

                buffer.Connections = new List<Connection>(roomInstance.ActionHelper.GetConnections);

                var packet = buffer.RequestPacket;
                buffer.CanSend = (packet.InsertUsers.Any() ||
                                  packet.RemoveUsers.Any() ||
                                  packet.UpdateUsers.Any());
            };

            Buffers.Add(buffer);
        }
    }
}
