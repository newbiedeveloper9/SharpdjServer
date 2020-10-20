using System;
using System.Threading.Tasks;
using Autofac;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using Log = Serilog.Log;

namespace SharpDj.Server.Application
{
    public class MainApp
    {
        private IContainer _container;

        public async Task Run()
        {
            Log.Information("Starting server...");
            try
            {
                _container = BuildContainer();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while building a {@Container}", "CONTAINER");
                return;
            }

            await using var scope = _container.BeginLifetimeScope();

            var server = scope.Resolve<ServerApp>();
            server.Start();
        }

        private IContainer BuildContainer()
        {
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ServerConfig.LoadConfig())
                .SingleInstance()
                .As<IServerConfig>();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,>));
            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,,>));

            builder.RegisterAssemblyTypes()
                .AsClosedTypesOf(typeof(ActionAbstract<>));

            builder.RegisterType<ServerContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<Setup>()
                .AsSelf()
                .AutoActivate();

            builder.RegisterType<ServerApp>()
                .AsSelf();

            return builder.Build();
        }
    }
}
