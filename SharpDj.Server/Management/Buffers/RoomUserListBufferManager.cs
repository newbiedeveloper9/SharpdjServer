using System.Collections.Generic;
using System.Linq;
using Network;
using SCPackets.Buffers;
using Serilog;
using SharpDj.Server.Management.Singleton;

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
                    .GetList()
                    .FirstOrDefault(x => x.Id == roomId);

                if (roomInstance == null)
                    Log.Fatal("This shouldn't ever happen");


                buffer.Connections = new List<Connection>(roomInstance.ActionHelper.GetConnections);

                var packet = buffer.RequestPacket;
                buffer.CanSend = (packet.InsertUsers.Count > 0 ||
                                  packet.RemoveUsers.Count > 0 ||
                                  packet.UpdateUsers.Count > 0);
            };

            Buffers.Add(buffer);
        }
    }
}
