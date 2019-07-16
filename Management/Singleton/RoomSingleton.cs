using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCPackets;

namespace Server.Management
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
