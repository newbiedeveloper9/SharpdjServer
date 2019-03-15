using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Management
{
    public sealed class RoomSingleton
    {
        private static readonly Lazy<RoomSingleton> lazy =
            new Lazy<RoomSingleton>(() => new RoomSingleton());

        public static RoomSingleton Instance => lazy.Value;

        private RoomSingleton()
        {
            Rooms = new SpecialRoomList<RoomInstance>();
        }

        public SpecialRoomList<RoomInstance> Rooms { get; set; }

    }
}
