using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using SCPackets.Buffers;

namespace Server.Management.Buffers
{
    public abstract class BufferManager<TReq> where TReq : RequestPacket
    {
        protected readonly List<ActionBuffer<TReq>> Buffers;
        public int Timer;

        protected BufferManager(int timer = 10000)
        {
            Timer = timer;
            Buffers = new List<ActionBuffer<TReq>>();

            Task.Factory.StartNew(BufferLoop);
        }

        private void BufferLoop()
        {
            while (true)
            {
                Thread.Sleep(Timer);

                foreach (var actionBuffer in Buffers)
                {
                    actionBuffer.SendRequestToAll();

                    ClearBuffer(actionBuffer);
                }
            }
        }

        protected abstract void ClearBuffer(ActionBuffer<TReq> actionBuffer);
        public abstract void CreateBuffer(dynamic obj);
    }
}
