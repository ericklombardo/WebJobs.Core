using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class JobHostedService : IHostedService, IDisposable
    {

        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly JobHost _jobHost;

        public JobHostedService(ILogger<JobHostedService> logger, IServiceProvider serviceProvider, IApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;

            _jobHost = new JobHost(CreateJobConfiguration(serviceProvider));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            await _jobHost.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _jobHost.StopAsync();
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");            
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
        }

        private JobHostConfiguration CreateJobConfiguration(IServiceProvider serviceProvider)
        {
            var configuration = new JobHostConfiguration();
            configuration.UseDevelopmentSettings();
            configuration.UseCore();
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.VisibilityTimeout = TimeSpan.FromMinutes(1);
            configuration.Queues.BatchSize = 1;
            configuration.UseTimers();
            configuration.JobActivator = new CustomJobActivator(serviceProvider);

            return configuration;
        }

        public void Dispose()
        {
            _jobHost?.Dispose();
        }
    }
}
