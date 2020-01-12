using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using NLog;
using NLog.Fluent;
using SCPackets.Buffers;

namespace Server.Management.Buffers
{
    public abstract class BufferManager<TReq> where TReq : RequestPacket
    {
        private readonly Logger Logger;
        protected readonly List<ActionBuffer<TReq>> Buffers;
        public int Timer { get; set; }

        protected BufferManager(Logger logger, int timer = 10000)
        {
            Logger = logger;
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
                    var success = actionBuffer.SendRequestToEveryone();
                    if (!success)
                    {
                        //Logger.Debug("Buffer is probably EMPTY. Sending data has *been skipped");
                        continue;
                    };

                    ClearBuffer(actionBuffer);
                    Logger.Info("Buffer is cleared");
                }
            }
        }

        protected abstract void ClearBuffer(ActionBuffer<TReq> actionBuffer);
        public abstract void CreateBuffer(dynamic obj);
    }
}
