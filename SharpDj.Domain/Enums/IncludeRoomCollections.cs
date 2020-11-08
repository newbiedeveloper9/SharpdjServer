using System;

namespace SharpDj.Domain.Enums
{
    [Flags]
    public enum IncludeRoomCollections
    {
        Creator = 1,
        Posts = 2,
        Config = 4
    }
}