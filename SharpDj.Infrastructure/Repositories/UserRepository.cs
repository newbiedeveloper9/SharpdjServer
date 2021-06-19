using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ServerContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public UserRepository(ServerContext context)
        {
            _context = context;
        }

        public async Task<bool> GivenLoginOrEmailExistsAsync(string login, string email)
        {
            /*            await semaphoreSlim.WaitAsync();
                        try
                        {*/
            return await _context.Users
                   .AnyAsync(x => x.Email == email)
                   .ConfigureAwait(false) ||
               await _context.Users
                   .Include(x => x.UserAuthEntity)
                   .AnyAsync(x => x.UserAuthEntity.Login == login)
                   .ConfigureAwait(false);
            /*            }
                        finally
                        {
                            semaphoreSlim.Release();
                        }*/
        }

        public async Task<UserEntity> GetUserByLoginOrEmailAsync(string login, string email)
        {
            return await _context.Users
                    .Include(x => x.UserAuthEntity)
                    .FirstOrDefaultAsync(x => x.UserAuthEntity.Login == login || x.Email == email)
                    .ConfigureAwait(false);
        }

        public void AddUser(UserEntity user)
        {
            _context.Users.Add(user);
        }
    }
}
