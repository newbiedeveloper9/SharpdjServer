using System.Collections.Generic;
using SCPackets.Models;

namespace SharpDj.Server.Management.Strategy
{
    public interface ITrackStrategy
    {
        void NextTrack(List<TrackModel> tracks);
    }
}