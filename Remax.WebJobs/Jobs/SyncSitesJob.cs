using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Remax.WebJobs.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Remax.WebJobs.Jobs
{
    public class SyncSitesJob
    {
        private readonly ILogger _logger;
        private readonly IFtpManager _ftpManager;
        private readonly INotificationManager _notificationManager;
        private readonly Microsoft.Extensions.Hosting.IHostEnvironment _hostingEnvironment;
        private readonly string _webConfigsPath;
        private readonly string _basePath;
        private readonly bool _replaceWebConfigs;
        private readonly int _webDeployTimeout;
        private readonly IPowerShellScriptRunner _powerShellScriptRunner;
        private readonly Dictionary<string, SiteSetting> _sites;
        private readonly CheckRunner _checkRunner;

        public SyncSitesJob(ILogger<SyncSitesJob> logger, 
            IFtpManager ftpManager,
            IOptions<Dictionary<string, SiteSetting>> options, 
            IPowerShellScriptRunner powerShellScriptRunner,
            Microsoft.Extensions.Hosting.IHostEnvironment hostingEnvironment, 
            INotificationManager notificationManager, 
            IConfiguration configuration, 
            CheckRunner checkRunner)
        {
            _logger = logger;
            _ftpManager = ftpManager;
            _powerShellScriptRunner = powerShellScriptRunner;
            _hostingEnvironment = hostingEnvironment;
            _notificationManager = notificationManager;
            _checkRunner = checkRunner;
            _webConfigsPath = configuration["WebConfigs"];
            _basePath = configuration["BasePath"];
            _replaceWebConfigs = bool.Parse(configuration["ReplaceWebConfigs"]);
            _webDeployTimeout = configuration.GetValue<int>("WebDeployTimeout");
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
            if (_checkRunner.IsRunning) return;

            try
            {
                _checkRunner.IsRunning = true;
                _logger.LogInformation("Begin Syncing ftp sites");
                var modulePath = $@"{_hostingEnvironment.ContentRootPath}\scripts\PSWebDeploy\PSWebDeploy.psm1";
                var detailSuccessUpdated = new List<SiteUpdatedDetail>();
                var detailFailed = new List<SiteUpdatedDetail>();
                foreach (var setting in _sites.Where(x => x.Value.Enable))
                {
                    try
                    {
                        _logger.LogInformation($"Begin Syncing {setting.Key}");
                        _powerShellScriptRunner.ExecuteScript("remaxgetsite.ps1",
                            new Dictionary<string, object>
                            {
                                {"appServiceName", setting.Key},
                                {"username", $"${setting.Key}"},
                                {"password", setting.Value.Password},
                                {"modulepath", modulePath},
                                {"wwwroot", _basePath},
                                {"root", setting.Value.RootFolder[0]},
                                {"timeout", _webDeployTimeout}
                            });
                        
                        if(!_powerShellScriptRunner.Success)  throw new Exception("Error running sync site script");

                        if (_replaceWebConfigs)
                        {
                            _logger.LogInformation($"Replacing web.config for site {setting.Key}");
                            File.Copy(
                                $@"{_basePath}\{_webConfigsPath}\{setting.Key}.config",
                                $@"{_basePath}\{setting.Key}\Web.config",
                                true);
                        }

                        _logger.LogInformation($"End Syncing {setting.Key}");
                        detailSuccessUpdated.Add(new SiteUpdatedDetail
                        {
                            AppService = setting.Key,
                            Url = setting.Value.Url
                        });
                    }
                    catch (Exception exc)
                    {
                        detailFailed.Add(new SiteUpdatedDetail {AppService = setting.Key});
                        _logger.LogError(exc, $"Error updating site {setting.Key}");
                    }
                }

                _logger.LogInformation("Finish syncing ftp sites");
                _notificationManager.SitesUpdated(detailSuccessUpdated.ToArray(), detailFailed.ToArray());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error syncing sites");
            }
            finally
            {
                _checkRunner.IsRunning = false;
            }
        }

    }
}
