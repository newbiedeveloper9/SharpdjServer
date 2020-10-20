using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SharpDj.Server.Application;
using SharpDj.Server.Management;

namespace SharpDj.Server
{
    internal class Program
    {
        private static async Task Main()
        {
            AppDomain.MonitoringIsEnabled = true;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            SetupLogging();
            await new MainApp().Run().ConfigureAwait(false);
            Listening();

            Log.Information("The process scope has ended.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
        }

        private static void SetupLogging()
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(@"logs\.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Logger = log;
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