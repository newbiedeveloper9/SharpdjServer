using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;
using SharpDj.Domain.Interfaces;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Application.Handlers;
using SharpDj.Server.Application.Handlers.CoR;
using SharpDj.Server.Application.Management.Config;
using SharpDj.Server.Extensions;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;

namespace SharpDj.Server
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

        public static IContainer BuildContainer(this IContainer container)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ServerContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAppSettings()
                .RegisterServerConfig()
                .RegisterAssemblies()
                .RegisterServer();


            container = builder.Build();
            return container;
        }
    }
}
