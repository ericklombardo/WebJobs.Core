using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Remax.WebJobs.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Remax.WebJobs.Jobs
{
    public class SyncSitesJob
    {
        private readonly ILogger _logger;
        private readonly IFtpManager _ftpManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPowerShellScriptRunner _powerShellScriptRunner;
        private readonly Dictionary<string, SiteSetting> _sites;

        public SyncSitesJob(ILogger<SyncSitesJob> logger, 
            IFtpManager ftpManager,
            IOptions<Dictionary<string, SiteSetting>> options, IPowerShellScriptRunner powerShellScriptRunner, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _ftpManager = ftpManager;
            _powerShellScriptRunner = powerShellScriptRunner;
            _hostingEnvironment = hostingEnvironment;
            _sites = options.Value;
        }

        /// <summary>
        /// Sync one or more sites
        /// </summary>
        /// <param name="json">Json object with the string array with the name of the sites to sync</param>
        /// <returns></returns>
        public async Task SyncSites(/*[QueueTrigger("%SyncSitesQueue%")]*/ string json)
        {
            var jObject = JObject.Parse(json);
            var siteKeys = jObject["sites"].ToObject<string[]>();

            var tasks = _sites.Where(x => siteKeys.Contains(x.Key))
                              .Select(site => _ftpManager.SyncDirectory(site.Value, site.Key))
                              .ToArray();

            _logger.LogInformation("Syncing ftp sites");
            int total = 0, pageSize = 5;
            while (total < tasks.Length)
            {
                await Task.WhenAll(tasks.Skip(total).Take(pageSize));
                total += pageSize;
            }
            _logger.LogInformation("Finish syncing ftp sites");
        }

        public void SyncSitesPs([QueueTrigger("%SyncSitesQueue%")] string json)
        {
            _logger.LogInformation("Begin Syncing ftp sites");

            var settings = _sites["ballon-regionalsys"];
            _powerShellScriptRunner.ExecuteScript("remaxgetsite.ps1", 
                new Dictionary<string, string>
                {
                    {"appServiceName", "ballon-regionalsys" },
                    {"username", "$ballon-regionalsys" },
                    {"password", settings.Password },
                    {"modulepath", $@"{_hostingEnvironment.ContentRootPath}scripts\PSWebDeploy\PSWebDeploy.psm1" }
                });

            _logger.LogInformation("Finish syncing ftp sites");
        }

    }
}
