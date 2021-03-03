﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;
using SharpDj.Domain.Builders;
using SharpDj.Domain.Factory;
using SharpDj.Domain.Options;
using SharpDj.Domain.Repository;
using SharpDj.Infrastructure.Repositories;
using SharpDj.Server.Application;
using System;

namespace SharpDj.Server
{
    public class Startup
    {
        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            SetupLogging();
        }

        private void SetupLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true),
                    @"logs\\log..txt",// log.20210214.txt
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void Start(IServiceProvider serviceProvider)
        {
            var setup = serviceProvider.GetService<Setup>();
            var server = serviceProvider.GetService<ServerMain>();

            setup.Start();
            server.Start();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SettingsOptions>(Configuration.GetSection(
                SettingsOptions.Settings));

            services
                .AddSingleton<IServerBuilder, ServerBuilder>()
                .AddSingleton<IPacketRegistrator, PacketRegistrator>()
                .AddSingleton<IChatMessageFactory, ChatMessageFactory>()
                .AddSingleton<IUserFactory, UserFactory>()
                .AddScoped<IRoomRepository, RoomRepository>()
                .AddScoped<IUserRepository, UserRepository>();
        }
    }
}