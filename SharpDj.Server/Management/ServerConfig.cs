using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace SharpDj.Server.Management
{
    public class ServerConfig : IServerConfig
    {
        [JsonRequired] public int Port { get; set; } = 5666;

        [JsonRequired] public string Ip { get; set; } = "127.0.0.1";
        [JsonRequired] public int RSAKeySize { get; set; } = 2048;

        public ServerConfig()
        {

        }

        public static ServerConfig LoadConfig(string configFile = "config.json")
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\{configFile}";

            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented);
                File.WriteAllText(path, json);

                Log.Information("New config file has been created.");
                Console.ReadLine();

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
                    Log.Error(ex, "Config error.");
                    Console.ReadLine();
                }
        }
    }

    public interface IServerConfig
    {
        int Port { get; set; }
        string Ip { get; set; }
        int RSAKeySize { get; set; }
    }
}