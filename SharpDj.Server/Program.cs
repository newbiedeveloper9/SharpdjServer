using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using SharpDj.Server.Application;
using SharpDj.Server.Management;

namespace SharpDj.Server
{
    internal class Program
    {
        private static IContainer _container;

        private static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            _container = _container.BuildContainer();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            await SetupLogging().ConfigureAwait(false);
            await SetupServer().ConfigureAwait(false);

            Listening(cancellationToken);

            Log.Information("The process scope has ended.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
        }

        private static async Task SetupLogging()
        {
            await using var scope = _container.BeginLifetimeScope();

            var configuration = scope.Resolve<IConfiguration>();
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true),
                    @"logs\\log..txt",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static async Task SetupServer()
        {
            await using var scope = _container.BeginLifetimeScope();

            var setup = scope.Resolve<Setup>();
            var server = scope.Resolve<App>();
            server.Start();
        }


        private static void Listening(CancellationToken cancellationToken)
        {
            var closeCommands = new[] { "exit", "quit", "q", "close", "stop" };
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var currentCommand = Console.ReadLine();
                if (closeCommands.Any(x => x.Equals(currentCommand)))
                {
                    break;
                }
            }
        }
    }
}