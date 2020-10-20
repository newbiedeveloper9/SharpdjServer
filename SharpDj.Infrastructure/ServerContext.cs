using System;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using Claim = System.Security.Claims.Claim;

namespace SharpDj.Infrastructure
{
    public class ServerContext : DbContext
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
        public DbSet<RoomChatPostEntity> RoomChatPosts { get; set; }
        public DbSet<RecordEntity> MediaHistories { get; set; }
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<ConversationMessageEntity> ConversationMessages { get; set; }
        public DbSet<ConversationEntity> Conversations { get; set; }
        public DbSet<UserConnectionEntity> Connections { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }


    }
}
