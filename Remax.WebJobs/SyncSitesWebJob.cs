using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class SyncSitesWebJob
    {
        private readonly ILogger<SyncSitesWebJob> _logger;
        private readonly IFtpManager _ftpManager;
        private readonly Dictionary<string, SiteSetting> _sites;

        public SyncSitesWebJob(ILogger<SyncSitesWebJob> logger, 
            IFtpManager ftpManager,
            IOptions<Dictionary<string, SiteSetting>> options)
        {
            _logger = logger;
            _ftpManager = ftpManager;
            _sites = options.Value;
        }

        public Task TestJob([TimerTrigger("%ScheduleExpression%", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            // Act on the DI-ed class:
            Console.WriteLine($"{DateTime.Now} - prueba");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sync one or more sites
        /// </summary>
        /// <param name="json">Json array string with the name of the sites to sync</param>
        /// <returns></returns>
        public void SyncSites([QueueTrigger("%QueueName%")] string json)
        {
            var jObject = JObject.Parse(json);
            var sites = jObject["sites"].ToObject<string[]>();
        }

    }
}
