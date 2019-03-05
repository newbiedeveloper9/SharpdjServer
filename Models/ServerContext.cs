using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class ServerContext : DbContext
    {
        public ServerContext() : base("Data Source=(LocalDB)\\MSSQLLocalDB;" +
                                     $"AttachDbFilename={System.AppDomain.CurrentDomain.BaseDirectory}\\ServerDB.mdf;" +
                                     "Integrated Security=True")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ServerContext>());
        }

        public DbSet<RoomConfig> RoomConfigs { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
