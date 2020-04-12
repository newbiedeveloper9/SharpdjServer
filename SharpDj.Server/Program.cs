using System;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                Log.Logger = log;

                var config = ServerConfig.LoadConfig();
                _ = new ConsoleApp(config).Run();

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}