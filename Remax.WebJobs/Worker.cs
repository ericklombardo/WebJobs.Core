using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Remax.WebJobs
{
    public class Worker : IHostedService
    {

        private readonly ILogger<Worker> _logger;
        private readonly IJobHost _jobHost;
        private readonly CloudQueueClientProvider _cloudQueueClientProvider;
        private readonly IConfiguration _configuration;

        public Worker(
            ILogger<Worker> logger, 
            IJobHost jobHost, 
            CloudQueueClientProvider cloudQueueClientProvider, 
            IConfiguration configuration)
        {
            _logger = logger;
            _jobHost = jobHost;
            _cloudQueueClientProvider = cloudQueueClientProvider;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting up the service");
            await ClearQueues();
            _logger.LogInformation("The service has started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping up the service");
            await ClearQueues();
            await _jobHost.StopAsync();
            _logger.LogInformation("The service has been stopped");
        }

        public async Task ClearQueues()
        {
            var queueClient = _cloudQueueClientProvider.Create();
            var queue = queueClient.GetQueueReference(_configuration["SyncSitesQueue"]);
            _logger.LogInformation("Clear queue");
            await queue.ClearAsync();
        }

    }
}