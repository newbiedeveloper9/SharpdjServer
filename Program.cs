using System;
using Server.Management;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ServerConfig.LoadConfig();
            var server = new Management.Server(config);
            server.Start();
            Console.WriteLine("Server started");
            Console.ReadLine();
        }
    }
}