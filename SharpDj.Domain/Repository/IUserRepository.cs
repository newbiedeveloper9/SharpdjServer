using System.Threading.Tasks;
using SharpDj.Domain.Entity;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Domain.Repository
{
    public interface IUserRepository : IRepository
    {
        Task<bool> GivenLoginOrEmailExistsAsync(string login, string email);
        Task<UserEntity> GetUserByLoginOrEmailAsync(string login, string email);
        void AddUser(UserEntity user);
    }
}