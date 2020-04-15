using System;
using SharpDj.Server.Management.Buffers;

namespace SharpDj.Server.Singleton
{
    public sealed class BufferSingleton
    {
        private static readonly Lazy<BufferSingleton> lazy =
            new Lazy<BufferSingleton>(() => new BufferSingleton());

        public static BufferSingleton Instance => lazy.Value;

        private BufferSingleton()
        {
            RoomUserListBufferManager = new RoomUserListBufferManager();
            SquareRoomBufferManager = new SquareRoomBufferManager();
        }

       public RoomUserListBufferManager RoomUserListBufferManager { get; set; }
       public SquareRoomBufferManager SquareRoomBufferManager { get; set; }
    }
}
