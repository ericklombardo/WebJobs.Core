using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
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
            services.AddLogging(configure => configure.AddConsole());
            services.AddLogging(configure => configure.AddSerilog());
            services.AddSingleton<IConfiguration>(provider => configuration);
            services.AddTransient<SyncSitesWebJob>();
            services.Configure<Dictionary<string, SiteSetting>>(configuration.GetSection("Sites"));
            
            Provider = services.BuildServiceProvider();
        }
    }
}
