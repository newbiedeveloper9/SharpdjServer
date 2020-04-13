using System;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using SharpDj.Server.Management;

namespace SharpDj.Server
{
    public class ConsoleApp
    {
        private IContainer container;

        public async Task Run()
        {
            Log.Information("Starting server...");
            try
            {
                container = BuildDi();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while building Dependency Injection");
            }
        }

        public IContainer BuildDi()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ServerConfig.LoadConfig())
                .SingleInstance()
                .As<IServerConfig>();

            builder.RegisterType<ServerApp>()
                .AsSelf()
                .AutoActivate()
                .OnActivated(x=>x.Instance.Start());

            return builder.Build();
        }
    }
}
