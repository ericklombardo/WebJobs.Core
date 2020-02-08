using System;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("webjobs.log")
                .CreateLogger();

            try
            {
                Log.Information("Starting up the service");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Fatal error starting the service");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                        .AddAzureStorage()
                        .AddTimers()
                        .AddExecutionContextBinding();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<IPowerShellScriptRunner, PowerShellScriptRunner>();
                    services.AddSingleton<INotificationManager, EmailNotificationManager>();
                    services.AddSingleton<IJobActivator, CustomJobActivator>();
                    services.AddSingleton<CheckRunner>();
                    services.AddScoped<IFtpClient, FtpClient>();
                    services.AddScoped<IFtpManager, FtpManager>();
                    services.AddScoped<SyncSitesJob>();
                    services.AddScoped<SyncDatabaseJob>();
                    services.AddScoped<CloudQueueClientProvider>();
                    services.Configure<Dictionary<string, SiteSetting>>(hostContext.Configuration.GetSection("Sites"));
                    services.Configure<FtpSetting>(hostContext.Configuration.GetSection("FtpSetting"));
                    services.Configure<EmailSetting>(hostContext.Configuration.GetSection("EmailSetting"));
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    configLogging.AddSerilog();
                })
                .UseWindowsService();
        }
    }
}
