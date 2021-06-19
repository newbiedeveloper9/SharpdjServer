using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Domain.Entity;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Domain.Repository
{
    public interface IRoomRepository : IRepository
    {
        Task<RoomEntity> GetRoomByIdAsync(int id);
        IEnumerable<RoomEntity> GetRoomByCreatorId(long userId);
        Task UpdateRoom(RoomEntity room);
        bool AnyRoomContainsName(string name);
    }
}