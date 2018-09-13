using Serilog;
using Microsoft.Azure.WebJobs;
using System;

namespace Remax.WebJobs
{
    class Program
    {


        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("webjobs.log")
                .CreateLogger();

            var serviceProvider = CompositeRoot.ServiceProvider;


            var configuration = new JobHostConfiguration();
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.VisibilityTimeout = TimeSpan.FromMinutes(1);
            configuration.Queues.BatchSize = 1;
            configuration.JobActivator = new CustomJobActivator(serviceProvider);
            configuration.UseTimers();

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }
    }
}
