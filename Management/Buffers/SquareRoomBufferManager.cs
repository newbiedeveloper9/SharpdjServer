using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SCPackets.Buffers;
using Server.Management.Singleton;

namespace Server.Management.Buffers
{
    public class SquareRoomBufferManager : BufferManager<SquareRoomBufferRequest>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SquareRoomBufferManager() : base(Logger, 8000)
        {
            CreateBuffer();
        }

        protected override void ClearBuffer(ActionBuffer<SquareRoomBufferRequest> actionBuffer)
        {
            actionBuffer.RequestPacket = new SquareRoomBufferRequest();
        }

        public SquareRoomBufferRequest GetRequest() => Buffers[0].RequestPacket;

        public sealed override void CreateBuffer(dynamic obj = null)
        {
            var request = new SquareRoomBufferRequest();
            var actionBuffer = new ActionBuffer<SquareRoomBufferRequest>(request);

            actionBuffer.BeforeSendBuffer +=
                (sender, args) =>
                {
                    actionBuffer.Connections =
                        ClientSingleton.Instance.Users
                            .GetList()
                            .GetAllConnections();

                    var packet = actionBuffer.RequestPacket;
                    actionBuffer.CanSend = (packet.InsertRooms.Count > 0 ||
                                            packet.RemoveRooms.Count > 0 ||
                                            packet.UpdatedRooms.Count > 0);
                };

            Buffers.Add(actionBuffer);
        }
    }
}
