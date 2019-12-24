using System;
using NLog;
using Server.Management;

namespace Server
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                Logger.Trace("Starting server...");
                var config = ServerConfig.LoadConfig();
                var server = new Management.Server(config);
                server.Start();
                Logger.Trace("Server is running!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                Console.ReadLine();
            }
        }
    }
}