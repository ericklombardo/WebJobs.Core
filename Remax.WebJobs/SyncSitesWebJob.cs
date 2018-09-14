using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class SyncSitesWebJob
    {
        private readonly ILogger _logger;
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
            Console.WriteLine($"{DateTime.Now} - prueba");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sync one or more sites
        /// </summary>
        /// <param name="json">Json object with the string array with the name of the sites to sync</param>
        /// <returns></returns>
        public async Task SyncSites([QueueTrigger("%QueueName%")] string json)
        {
            var jObject = JObject.Parse(json);
            var siteKeys = jObject["sites"].ToObject<string[]>();

            var tasks = _sites.Where(x => siteKeys.Contains(x.Key))
                              .Select(site => _ftpManager.SyncDirectory(site.Value, site.Key));

            await Task.WhenAll(tasks);
        }

    }
}
