using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// Uncomment to run as a Windows Service
// using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace PlexWebHook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("hosting.json", true)
                    .Build();

            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }


        /* To run as a Windows Service, replace the Main method with the one below, and add the missing import.
        public static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Directory.SetCurrentDirectory(path);

            var config = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("hosting.json", true)
                    .Build();

            var builder = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>();
 
            var host = builder.Build();

            host.RunAsService();
        }
         */
    }
}
