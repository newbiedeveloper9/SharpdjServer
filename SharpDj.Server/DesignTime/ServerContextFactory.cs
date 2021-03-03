using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharpDj.Infrastructure;

namespace SharpDj.Server.DesignTime
{
    class ServerContextFactory : IDesignTimeDbContextFactory<ServerContext>
    {
        public ServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServerContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=SdjServerDB; Trusted_Connection=true;Max Pool Size=200");

            return new ServerContext(optionsBuilder.Options);
        }
    }
}
