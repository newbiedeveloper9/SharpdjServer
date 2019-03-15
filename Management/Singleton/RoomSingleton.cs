using System;
using System.Collections.Generic;
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
            Rooms = new List<RoomInstance>();
        }

        public List<RoomInstance> Rooms { get; set; }
    }
}
