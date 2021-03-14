using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharpDj.Infrastructure;

namespace SharpDj.Server.DesignTime
{
    class ServerContextFactory : IDesignTimeDbContextFactory<ServerContext>
    {
        public ServerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<ServerContext>();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Main"));

            return new ServerContext(optionsBuilder.Options);
        }
    }
}
