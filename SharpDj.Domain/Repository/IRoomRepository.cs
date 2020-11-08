using System.Threading.Tasks;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Repository
{
    public interface IRoomRepository : IRepository
    {
        Task<RoomEntity> GetRoomByIdAsync(int id);
        Task UpdateRoom(RoomEntity room);
        bool AnyRoomContainsName(string name);
    }
}