﻿using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentFTP;
using Remax.WebJobs.Jobs;
using Remax.WebJobs.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Remax.WebJobs
{
    internal class Program
    {

        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("webjobs.log")
                .CreateLogger();

            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                 {
                     configHost.SetBasePath(Directory.GetCurrentDirectory());
                     configHost.AddJsonFile("hostsettings.json", true);
                     configHost.AddEnvironmentVariables("PREFIX_");
                     configHost.AddCommandLine(args);
                 })
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                        .AddAzureStorage()
                        .AddTimers()
                        .AddExecutionContextBinding();
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        true);
                    configApp.AddEnvironmentVariables("PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<IPowerShellScriptRunner, PowerShellScriptRunner>();
                    services.AddSingleton<INotificationManager, EmailNotificationManager>();
                    services.AddSingleton<IJobActivator, CustomJobActivator>();
                    services.AddScoped<IFtpClient, FtpClient>();
                    services.AddScoped<IFtpManager, FtpManager>();
                    services.AddScoped<SyncSitesJob>();
                    services.AddScoped<SyncDatabaseJob>();
                    services.Configure<Dictionary<string, SiteSetting>>(hostContext.Configuration.GetSection("Sites"));
                    services.Configure<FtpSetting>(hostContext.Configuration.GetSection("FtpSetting"));
                    services.Configure<EmailSetting>(hostContext.Configuration.GetSection("EmailSetting"));
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    configLogging.AddSerilog();
                })
                .UseConsoleLifetime();


            if (isService)
            {
                await hostBuilder.RunAsServiceAsync();
            }
            else
            {
                await hostBuilder.RunConsoleAsync();
            }
        }
    }
}
