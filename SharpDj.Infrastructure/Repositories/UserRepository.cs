using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ServerContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public UserRepository(ServerContext context)
        {
            _context = context;
        }

        public bool GivenLoginOrEmailExists(string login, string email)
        {
            return _context.Users.Include(x => x.UserAuthEntity).Any(x => x.UserAuthEntity.Login == login) ||
                   _context.Users.Any(x => x.Email == email);
        }

        public void AddUser(UserEntity user)
        {
            _context.Users.Add(user);
        }
    }
}
