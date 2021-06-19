using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using SharpDj.Domain.SeedWork;
using SharpDj.Infrastructure.EntityConfiguration;

namespace SharpDj.Infrastructure
{
    public class ServerContext : DbContext, IUnitOfWork
    {
        public ServerContext(DbContextOptions<ServerContext> options) 
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);
        }

        public DbSet<UserAuthEntity> UserAuths { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoomConfigEntity> RoomConfigs { get; set; }
        public DbSet<RoomChatMessageEntity> RoomChatPosts { get; set; }
        public DbSet<UserAuditEntity> Connections { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
