using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharpDj.Server.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace SharpDj.Server
{
    internal class Program
    {
        private static async Task Main()
        {
            //Log all unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            //Create startup with initial IoC
            var startup = CreateStartup();

            //Configure services with .NET build-in IoC
            var serviceCollection = new ServiceCollection();
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
            startup.ConfigureServices(serviceCollection);

            //Populate autofac DI
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            //Create proper IoC
            var container = containerBuilder.BuildContainer();
            var serviceProvider = new AutofacServiceProvider(container);
            serviceProvider.ConfigureAwait(false);

            //Recreate startup
            startup = serviceProvider.GetRequiredService<Startup>();

            //Run
            await startup.Start(serviceProvider)
                .ConfigureAwait(false);
            Listening();

            Log.Information("Closing server...");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
        }

        private static Startup CreateStartup()
        {
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.BuildInitialContainer();

            var serviceProvider = new AutofacServiceProvider(container);
            var startup = serviceProvider.GetService<Startup>();
            serviceProvider.Dispose();
            return startup;
        }

        private static void Listening()
        {
            var closeCommands = new[] { "exit", "quit", "q", "close", "stop" };
            while (true)
            {
                var currentCommand = Console.ReadLine();
                if (closeCommands.Any(x => x.Equals(currentCommand)))
                {
                    break;
                }
            }
        }
    }
}