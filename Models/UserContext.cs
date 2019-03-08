using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base("Data Source=(LocalDB)\\MSSQLLocalDB;" +
                                     $"AttachDbFilename={System.AppDomain.CurrentDomain.BaseDirectory}\\UserDB.mdf;" +
                                     "Integrated Security=True")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<UserContext>());
        }

        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
