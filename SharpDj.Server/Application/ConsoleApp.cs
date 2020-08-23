using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using SharpDj.Server.Entity;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using Log = Serilog.Log;

namespace SharpDj.Server.Application
{
    public class ConsoleApp
    {
        private IContainer container;

        public async Task Run()
        {
            Log.Information("Starting server...");
            try
            {
                container = BuildContainer();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while building a {@Container}", "CONTAINER");
            }
        }

        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.AddAutoMapper();

            builder.RegisterInstance(ServerConfig.LoadConfig())
                .SingleInstance()
                .As<IServerConfig>();

            builder.RegisterType<ServerContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(x => x.Name.EndsWith("Action"))
                .As<IAction>()
                .SingleInstance();

            builder.RegisterType<ServerApp>()
                .AsSelf()
                .AutoActivate()
                .OnActivated(x=>x.Instance.Start());

            builder.RegisterType<Setup>()
                .AsSelf()
                .AutoActivate();

            return builder.Build();
        }
    }
}
