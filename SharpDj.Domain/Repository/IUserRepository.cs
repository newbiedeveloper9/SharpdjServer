using SharpDj.Domain.Entity;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Domain.Repository
{
    public interface IUserRepository : IRepository
    {
        bool GivenLoginOrEmailExists(string login, string email);
        void AddUser(UserEntity user);
    }
}