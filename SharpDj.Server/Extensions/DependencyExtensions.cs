using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using SharpDj.Common.Handlers;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Domain.Interfaces;
using SharpDj.Server.Application;
using System;
using System.Collections.Generic;
using System.IO;
using Network.Packets;
using SCPackets.Packets.Login;
using SCPackets.Packets.Register;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Handlers;

namespace SharpDj.Server.Extensions
{
    public static class DependencyExtensions
    {
        private static ContainerBuilder RegisterServer(this ContainerBuilder builder, bool initial)
        {
            builder.RegisterType<Startup>()
                .InstancePerDependency()
                .AsSelf();

            if (initial == false)
            {
                builder.RegisterType<Setup>()
                    .InstancePerDependency()
                    .AsSelf();

                builder.RegisterType<ServerMain>()
                    .InstancePerDependency()
                    .AsSelf();
            }

            return builder;
        }

        private static ContainerBuilder RegisterAssemblies(this ContainerBuilder builder)
        {
            var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,>))
                .SingleInstance();
            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDualMapper<,,>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IDictionaryConverter<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(appAssemblies)
                .InNamespaceOf<AbstractHandler>()
                .PublicOnly()
                .Where(x => x.Name.EndsWith("Handler"))
                .InstancePerLifetimeScope()
                .AsSelf();

            builder.RegisterAssemblyTypes(appAssemblies)
                .AsClosedTypesOf(typeof(IAction<>))
                .InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterHandler<T>(this ContainerBuilder builder)
            where T : RequestPacket
        {
            builder.RegisterType<RequestHandler<T>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterHandlers(this ContainerBuilder builder)
        {
            builder.RegisterHandler<LoginRequest>()
                .RegisterHandler<RegisterRequest>();

            return builder;
        }

        private static ContainerBuilder RegisterConfiguration(this ContainerBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(configurationBuilder)
                .SingleInstance()
                .AsImplementedInterfaces();

            return builder;
        }

        private static ContainerBuilder RegisterContext<TContext>(this ContainerBuilder builder, string connectionString)
            where TContext : DbContext
        {
            builder.Register(componentContext =>
                {
                    var serviceProvider = componentContext.Resolve<IServiceProvider>();
                    var configuration = componentContext.Resolve<IConfiguration>();
                    var dbContextOptions = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
                    var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                        .UseApplicationServiceProvider(serviceProvider)
                        .UseSqlServer(configuration.GetConnectionString(connectionString),
                            serverOptions => serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null));

                    return optionsBuilder.Options;
                }).As<DbContextOptions<TContext>>()
                .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<DbContextOptions<TContext>>())
                .As<DbContextOptions>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            return builder;
        }

        public static IContainer BuildContainer(this ContainerBuilder builder)
        {
            builder.RegisterConfiguration()
                .RegisterContext<ServerContext>("Main")
                .RegisterAssemblies()
                .RegisterHandlers()
                .RegisterServer(initial:false);

            return builder.Build();
        }

        public static IContainer BuildInitialContainer(this ContainerBuilder builder)
        {
            builder.RegisterConfiguration()
                .RegisterServer(initial: true);

            return builder.Build();
        }
    }
}
