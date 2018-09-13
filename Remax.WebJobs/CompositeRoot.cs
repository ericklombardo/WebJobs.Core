using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace Remax.WebJobs
{
    public sealed class CompositeRoot
    {

        private static IServiceProvider Instance;
        private static readonly object padlock = new object();

        public static IServiceProvider ServiceProvider
        {
            get
            {
                lock (padlock)
                {
                    return Instance ?? (Instance = new CompositeRoot().Provider);
                }
            }
        }

        public  IServiceProvider Provider { get; }

        private CompositeRoot()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .Build();

            services.AddSingleton<IConfiguration>(provider => configuration);
            services.AddTransient<CopySiteWebJob>();
            services.AddLogging(configure => configure.AddSerilog());
            Provider = services.BuildServiceProvider();
        }
    }
}
