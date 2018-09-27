using FluentFTP;
using Microsoft.Extensions.Logging;
using Remax.WebJobs.Settings;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Remax.WebJobs
{
    public class FtpManager : IFtpManager
    {

        private readonly ILogger _logger;
        private readonly IFtpClient _client;


        public FtpManager(ILogger<FtpManager> logger, IFtpClient client, IOptions<FtpSetting> ftpSetting)
        {
            _logger = logger;
            _client = client;

            var settings = ftpSetting.Value;
            _client.SocketPollInterval = settings.SocketPollInterval;
            _client.ConnectTimeout = settings.ConnectTimeout;
            _client.ReadTimeout = settings.ReadTimeout;
            _client.DataConnectionConnectTimeout = settings.DataConnectionConnectTimeout;
            _client.DataConnectionReadTimeout = settings.DataConnectionReadTimeout;
            _client.DataConnectionType = settings.DataConnectionType;
        }

        public async Task SyncDirectory(SiteSetting site, string siteKey)
        {
            _logger.LogInformation($"Starting sync {siteKey}");
            try
            {
                _client.Host = site.FtpServer;
                _client.Credentials = new System.Net.NetworkCredential(site.UserName, site.Password);
                await _client.ConnectAsync();
                await _client.DownloadFolderRecursive(site.RootFolder[0], site.DestinationFolder);
                _logger.LogInformation($"Finish sync {siteKey}");
            }
            catch (FtpException exc)
            {
                _logger.LogError(exc, $"Error processing site {siteKey}");
            }
            finally
            {
                if (_client.IsConnected)
                {
                    await _client.DisconnectAsync();
                }
            }
        }

    }
}
