using System.Data.Entity;

namespace Server.Models
{
    public class ServerContext : DbContext
    {
        public ServerContext() : base("Data Source=(LocalDB)\\MSSQLLocalDB;" +
                                     $"AttachDbFilename={System.AppDomain.CurrentDomain.BaseDirectory}\\ServerDB.mdf;" +
                                     "Integrated Security=True")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ServerContext>());
        }

        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RoomConfig> RoomConfigs { get; set; }
        public DbSet<RoomChatPost> RoomChatPosts { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<MediaHistory> MediaHistories { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConnection> Connections { get; set; }
    }
}
