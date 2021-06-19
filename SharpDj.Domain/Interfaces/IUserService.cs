using System.Threading.Tasks;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> Update(UserDTO user);

        Task<UserDTO> Create(UserDTO user);
        Task<bool> Delete(UserDTO user); 
    }
}