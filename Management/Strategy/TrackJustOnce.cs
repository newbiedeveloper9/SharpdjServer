using System.Collections.Generic;
using SCPackets.Models;

namespace Server.Management.Strategy
{
    public class TrackJustOnce : ITrackStrategy
    {
        public void NextTrack(List<TrackModel> tracks)
        {
            if (tracks.Count > 0)
                tracks?.RemoveAt(0);
        }
    }
}