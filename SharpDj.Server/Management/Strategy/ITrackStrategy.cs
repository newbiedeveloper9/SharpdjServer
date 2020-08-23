using System.Collections.Generic;
using SCPackets.Domain;
using SCPackets.Models;

namespace SharpDj.Server.Management.Strategy
{
    public interface ITrackStrategy
    {
        void NextTrack(List<Track> tracks);
    }
}