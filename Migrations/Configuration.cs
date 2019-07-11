using Server.Models.EF;

namespace Server.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Server.Models.ServerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Server.Models.ServerContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.ServerRoles.AddOrUpdate(new ServerRole() { Name = "Admin" });
            context.ServerRoles.AddOrUpdate(new ServerRole() { Name = "Moderator" });
            context.ServerRoles.AddOrUpdate(new ServerRole() { Name = "User" });
        }
    }
}
