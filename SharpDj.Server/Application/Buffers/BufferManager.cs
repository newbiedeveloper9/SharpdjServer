using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Network.Packets;
using Serilog;

namespace SharpDj.Server.Application.Management.Buffers
{
    public abstract class BufferManager<TReq> where TReq : RequestPacket
    {
        protected readonly List<ActionBuffer<TReq>> Buffers;
        public int Timer { get; set; }

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
                    var success = actionBuffer.SendRequestToEveryone();
                    if (!success)
                    {
                        //Logger.Debug("Buffer is probably EMPTY. Sending data has *been skipped");
                        continue;
                    };

                    ClearBuffer(actionBuffer);
                    Log.Information("Buffer is cleared");
                }
            }
        }

        protected abstract void ClearBuffer(ActionBuffer<TReq> actionBuffer);
        public abstract void CreateBuffer(dynamic obj);
    }
}
