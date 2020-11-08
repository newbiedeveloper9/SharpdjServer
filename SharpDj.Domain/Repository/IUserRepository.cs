using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Repository
{
    public interface IUserRepository : IRepository
    {
        bool GivenLoginOrEmailExists(string login, string email);
        void AddUser(UserEntity user);
    }
}