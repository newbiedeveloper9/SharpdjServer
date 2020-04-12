using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using SharpDj.Server.Management;

namespace SharpDj.Server
{
    public class ConsoleApp
    {
        readonly ServerConfig _config;

        public ConsoleApp(ServerConfig config)
        {
            this._config = config;

            BuildDi(new ContainerBuilder());
        }

        public async Task Run()
        {
            Log.Information("Starting server...");
            var server = new Management.Server(_config);
            server.Start();
        }

        public IContainer BuildDi(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(x => x.Name.EndsWith("Action"))
                .AsImplementedInterfaces();

            return builder.Build();
        }
    }
}
