using System;
using SCPackets;
using SharpDj.Common;
using SharpDj.Server.Models;

namespace SharpDj.Server.Singleton
{
    public sealed class RoomSingleton
    {
        private static readonly Lazy<RoomSingleton> lazy =
            new Lazy<RoomSingleton>(() => new RoomSingleton());

        public static RoomSingleton Instance => lazy.Value;

        private RoomSingleton()
        {
            RoomInstances = new ListWrapper<RoomEntityInstance>();
        }

        public ListWrapper<RoomEntityInstance> RoomInstances { get; set; }
    }
}
