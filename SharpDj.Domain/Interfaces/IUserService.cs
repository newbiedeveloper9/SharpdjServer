using System.Threading.Tasks;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Services
{
    public interface IUserService
    {
        Task<UserDTO> Update(UserDTO user);

        Task<UserDTO> Create(UserDTO user);
        Task<bool> Delete(UserDTO user); 
    }
}