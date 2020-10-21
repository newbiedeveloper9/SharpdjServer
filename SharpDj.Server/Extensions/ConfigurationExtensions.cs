using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SharpDj.Server.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration AddAppsettingsConfiguration(this ConfigurationBuilder configBuilder)
        {
            var configuration = configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration;
        }
    }
}
