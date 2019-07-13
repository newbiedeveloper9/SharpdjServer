using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SCPackets.Buffers;
using Server.Management.Buffers;

namespace Server.Management.Singleton
{
    public sealed class BufferSingleton
    {
        private static readonly Lazy<BufferSingleton> lazy =
            new Lazy<BufferSingleton>(() => new BufferSingleton());

        public static BufferSingleton Instance => lazy.Value;

        private BufferSingleton()
        {
            RoomUserListBufferManager = new RoomUserListBufferManager();
        }

       public RoomUserListBufferManager RoomUserListBufferManager { get; set; }
    }
}
