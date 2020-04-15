using System;
using System.Linq;
using System.Reflection;
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
        private static void Main()
        {
            try
            {
                var log = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(@"logs\.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                Log.Logger = log;

                new ConsoleApp().Run().ConfigureAwait(false);
                Listening();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }

            Log.Information("The process scope has ended.");
        }

        private static void Listening()
        {
            var tmp = new[] { "exit", "quit", "q", "close", "stop" };
            while (true)
            {
                var line = Console.ReadLine();
                if (tmp.Any(x => x.Equals(line))) break;
            }
        }
    }
}