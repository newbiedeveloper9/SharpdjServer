using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        public Task<UserDTO> Login(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> Login(string authKey)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> Register(string login, string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAccount(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPassword(string newPassword, string oldPassword, string login)
        {
            throw new NotImplementedException();
        }
    }
}
