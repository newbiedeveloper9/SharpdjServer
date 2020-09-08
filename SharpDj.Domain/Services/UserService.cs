using SharpDj.Common.DTO;
using System.Threading.Tasks;

namespace SharpDj.Domain.Services
{
    public class UserService : IUserService
    {
        public Task<UserDTO> Create(UserDTO user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(UserDTO user)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserDTO> Update(UserDTO user)
        {
            throw new System.NotImplementedException();
        }
    }
}