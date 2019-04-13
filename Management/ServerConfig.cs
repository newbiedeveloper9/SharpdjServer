using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Server.Management
{
    public class ServerConfig
    {
        [JsonRequired] public int Port { get; set; } = 5666;

        [JsonRequired] public string Ip { get; set; } = "127.0.0.1";
        [JsonRequired] public int RSAKeySize { get; set; } = 2048;

        private ServerConfig()
        {
        }

        public static ServerConfig LoadConfig(string configFile = "config.json")
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\{configFile}";

            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented);
                File.WriteAllText(path, json);
                Console.WriteLine("New config file has been created. Press enter to restart.");
                Console.ReadLine();
                Process.Start("Server.exe");
                Environment.Exit(0);
            }

            while (true)
                try
                {
                    return JsonConvert.DeserializeObject<ServerConfig>(
                        File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in config.json:");
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
        }
    }
}