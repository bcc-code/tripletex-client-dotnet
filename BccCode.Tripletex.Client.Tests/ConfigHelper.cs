using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BccCode.Tripletex.Client.Tests
{
    public class ConfigHelper
    {
        public static IConfigurationRoot GetConfigurationRoot(string? outputPath = null)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("419a9a75-4bd4-4ca5-a810-888b084782f3")
                .Build();
        }

        public static TripletexClientOptions GetOptions(string? outputPath = null)
        {
            outputPath = outputPath ?? Directory.GetCurrentDirectory();
            return GetConfigurationRoot(outputPath).GetRequiredSection("Tripletex").Get<TripletexClientOptions>();
        }
    }
}
