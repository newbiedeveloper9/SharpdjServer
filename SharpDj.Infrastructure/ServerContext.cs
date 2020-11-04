using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using Claim = System.Security.Claims.Claim;

namespace SharpDj.Infrastructure
{
    public class ServerContext : DbContext, IUnitOfWork
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB; Database=SdjServerDB; Trusted_Connection=true;",
                    builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<UserAuthEntity> UserAuths { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoomConfigEntity> RoomConfigs { get; set; }
        public DbSet<RoomChatMessageEntity> RoomChatPosts { get; set; }
        public DbSet<UserConnectionEntity> Connections { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
