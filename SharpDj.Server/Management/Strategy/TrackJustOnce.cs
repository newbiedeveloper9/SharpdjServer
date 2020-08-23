using System.Collections.Generic;
using SCPackets.Domain;
using SCPackets.Models;

namespace SharpDj.Server.Management.Strategy
{
    public class TrackJustOnce : ITrackStrategy
    {
        public void NextTrack(List<Track> tracks)
        {
            if (tracks.Count > 0)
                tracks?.RemoveAt(0);
        }
    }
}