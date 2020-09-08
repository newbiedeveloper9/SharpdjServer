using System;
using System.Threading.Tasks;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Services
{
    public interface IUserAuthorizationService
    {
        Task<UserDTO> Login(string login, string password);
        Task<UserDTO> Login(string authKey);
        Task<UserDTO> Register(string login, string email, string password);
        Task<bool> ResetPassword(string newPassword, string oldPassword, string login);
        Task Logout();
    }
}