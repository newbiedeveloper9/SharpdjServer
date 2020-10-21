using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace SharpDj.Server.Management
{
    public class ServerConfig : IServerConfig
    {
        [JsonRequired] public short Port { get; set; } = 5666;
        [JsonRequired] public string Ip { get; set; } = "127.0.0.1";
        public short? RSAKeySize { get; set; } = null;
        public bool Logging { get; set; } = false;

        public static ServerConfig LoadConfig(string configFile = "config.json")
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\{configFile}";
            if (CreateConfigIfNotExists(path))
            {
                Environment.Exit(0);
            }

            while (true)
            {
                try
                {
                    return JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Config error...");
                    Console.ReadLine();
                }
            }
        }

        private static bool CreateConfigIfNotExists(string path)
        {
            if (File.Exists(path))
            {
                return false;
            }

            var json = JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented);
            File.WriteAllText(path, json);

            Log.Information("New config file has been created.");
            Console.ReadLine();

            return true;
        }
    }

    public interface IServerConfig
    {
        short Port { get; set; }
        string Ip { get; set; }
        short? RSAKeySize { get; set; }
        bool Logging { get; set; }
    }
}