using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using SCPackets.Buffers;
using Server.Management.Singleton;

namespace Server.Management.Buffers
{
    public class RoomUserListBufferManager
    {
        private readonly List<ActionBuffer<RoomUserListBufferRequest>> _roomUserListBuffer;

        public RoomUserListBufferManager()
        {
            _roomUserListBuffer = new List<ActionBuffer<RoomUserListBufferRequest>>();

            Task.Factory.StartNew(BufferLoop);
        }

        private void BufferLoop()
        {
            while (true)
            {
                Thread.Sleep(10000);

                foreach (var actionBuffer in _roomUserListBuffer)
                {
                    actionBuffer.SendRequestToAll();

                    var roomId = actionBuffer.RequestPacket.RoomId;
                    actionBuffer.RequestPacket = new RoomUserListBufferRequest(roomId);
                }
            }
        }

        public ActionBuffer<RoomUserListBufferRequest> GetByRoomId(int roomId)
        {
            return _roomUserListBuffer.FirstOrDefault(x => x.RequestPacket.RoomId == roomId);
        }

        public void CreateBuffer(int roomId)
        {
            var request = new RoomUserListBufferRequest(roomId);
            var buffer = new ActionBuffer<RoomUserListBufferRequest>(15000, request);
            buffer.BeforeSendBuffer += (sender, args) =>
            {
                var roomInstance = RoomSingleton.Instance.RoomInstances.GetList().FirstOrDefault(x => x.Id == roomId);
                if (roomInstance == null) throw new Exception("CreateBufer Manager for RoomUserList");

                buffer.Connections = new List<Connection>(roomInstance.ActionHelper.GetConnections);
            };

            _roomUserListBuffer.Add(buffer);

        }
    }
}
