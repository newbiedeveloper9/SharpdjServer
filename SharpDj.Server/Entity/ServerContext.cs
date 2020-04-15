using Microsoft.EntityFrameworkCore;

namespace SharpDj.Server.Entity
{
    public class ServerContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "server=(localdb)\\mssqllocaldb; Database=SdjServerDB; MultipleActiveResultSets=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RoomConfig> RoomConfigs { get; set; }
        public DbSet<RoomChatPost> RoomChatPosts { get; set; }
        public DbSet<MediaHistory> MediaHistories { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConnection> Connections { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ServerRole> ServerRoles { get; set; }
        public DbSet<Room> Rooms { get; set; }


    }
}
