using System;
using SCPackets;
using SharpDj.Server.Models;

namespace SharpDj.Server.Management.Singleton
{
    public sealed class RoomSingleton
    {
        private static readonly Lazy<RoomSingleton> lazy =
            new Lazy<RoomSingleton>(() => new RoomSingleton());

        public static RoomSingleton Instance => lazy.Value;

        private RoomSingleton()
        {
            RoomInstances = new ListWrapper<RoomInstance>();
        }

        public ListWrapper<RoomInstance> RoomInstances { get; set; }

    }
}
