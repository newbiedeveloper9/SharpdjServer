using System.Threading.Tasks;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDetailsDTO> Update();
        Task<UserDTO> Create(UserDTO user);
        Task<bool> Delete(UserDTO user);
    }
}