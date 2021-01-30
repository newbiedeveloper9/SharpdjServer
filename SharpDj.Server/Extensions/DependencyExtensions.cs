using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using SharpDj.Domain.Factory;
using SharpDj.Domain.Interfaces;
using SharpDj.Domain.Repository;
using SharpDj.Infrastructure;
using SharpDj.Infrastructure.Repositories;
using SharpDj.Server.Application;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Handlers;
using SharpDj.Server.Application.Handlers.Base;
using SharpDj.Server.Application.Handlers.CoR;
using SharpDj.Server.Application.Management;
using SharpDj.Server.Application.Management.Config;

namespace SharpDj.Server.Extensions
{
    public static class DependencyExtensions
    {
        private static ContainerBuilder RegisterServerConfig(this ContainerBuilder builder)
        {
            builder.RegisterInstance(Config.LoadConfig())
                .SingleInstance()
                .As<IConfig>();

            return builder;
        }

        private static ContainerBuilder RegisterServer(this ContainerBuilder builder)
        {
            builder.RegisterType<Setup>()
                .AsSelf();

            builder.RegisterType<App>()
                .AsSelf();

            return builder;
        }

        private static ContainerBuilder RegisterAssemblies(this ContainerBuilder builder)
        {
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,>));
            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,,>));

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDictionaryConverter<>));

            builder.RegisterAssemblyTypes(appAssemblies)
                .InNamespaceOf<AbstractHandler>()
                .PublicOnly()
                .Where(x => x.Name.EndsWith("Handler"))
                .SingleInstance()
                .AsSelf();

            builder.RegisterAssemblyTypes(appAssemblies)
                .InNamespace("SharpDj.Server.Application.Handlers")
                .PublicOnly()
                .Where(x => x.Name.StartsWith("Server") && x.Name.EndsWith("Action"))
                .SingleInstance()
                .As<IAction>();

            return builder;
        }

        private static ContainerBuilder RegisterAppSettings(this ContainerBuilder builder)
        {
            builder.RegisterInstance(new ConfigurationBuilder().AddAppsettingsConfiguration())
                .SingleInstance()
                .AsImplementedInterfaces();

            return builder;
        }

        private static ContainerBuilder RegisterRepositories(this ContainerBuilder builder)
        {
            builder.RegisterType<RoomRepository>()
                .As<IRoomRepository>()
                .InstancePerDependency();

            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerDependency();

            return builder;
        }

        private static ContainerBuilder RegisterFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<ChatMessageFactory>()
                .As<IChatMessageFactory>()
                .SingleInstance();

            builder.RegisterType<UserFactory>()
                .As<IUserFactory>()
                .SingleInstance();

            return builder;
        }

        public static IContainer BuildContainer(this IContainer container)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ServerContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAppSettings()
                .RegisterServerConfig()
                .RegisterRepositories()
                .RegisterFactories()
                .RegisterAssemblies()
                .RegisterServer();


            container = builder.Build();
            return container;
        }
    }
}
