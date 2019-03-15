using System.Collections.Generic;
using SCPackets.Models;

namespace Server.Management.Strategy
{
    public interface ITrackStrategy
    {
        void NextTrack(List<TrackModel> tracks);
    }
}