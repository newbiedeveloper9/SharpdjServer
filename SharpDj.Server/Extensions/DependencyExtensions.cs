using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application;
using SharpDj.Server.Extensions;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;

namespace SharpDj.Server
{
    public static class DependencyExtensions
    {
        private static ContainerBuilder RegisterServerConfig(this ContainerBuilder builder)
        {
            builder.RegisterInstance(ServerConfig.LoadConfig())
                .SingleInstance()
                .As<IServerConfig>();

            return builder;
        }

        private static ContainerBuilder RegisterServer(this ContainerBuilder builder)
        {
            builder.RegisterType<Setup>()
                .AsSelf();

            builder.RegisterType<ServerApp>()
                .AsSelf();

            return builder;
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,>));
            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,,>));

            return builder;
        }

        private static ContainerBuilder RegisterAppSettings(this ContainerBuilder builder)
        {
            builder.RegisterInstance(new ConfigurationBuilder().AddAppsettingsConfiguration())
                .SingleInstance()
                .AsImplementedInterfaces();

            return builder;
        }

        public static IContainer BuildContainer(this IContainer container)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes()
                .AsClosedTypesOf(typeof(ActionAbstract<>));

            builder.RegisterType<ServerContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAppSettings()
                .RegisterServerConfig()
                .RegisterMappers()
                .RegisterServer();
                

            container = builder.Build();
            return container;
        }
    }
}
