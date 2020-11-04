using System.Collections.Generic;
using SCPackets.Models;
using SharpDj.Common.DTO;

namespace SharpDj.Server.Management.Strategy
{
    public interface ITrackStrategy
    {
        void NextTrack(List<TrackDTO> tracks);
    }
}