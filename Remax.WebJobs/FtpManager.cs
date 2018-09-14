using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class FtpManager : IFtpManager
    {

        private readonly IFtpClient _client;
        private readonly ILogger _logger;

        public FtpManager(IFtpClient ftpClient, ILogger<FtpManager> logger)
        {
            _client = ftpClient;
            _logger = logger;
        }

        public async Task SyncDirectory(SiteSetting site, string siteKey)
        {
            _client.Host = site.FtpServer;
            _client.Credentials = new NetworkCredential(site.UserName, site.Password);
            try
            {
                await _client.ConnectAsync();
                await _client.DownloadFilesAsync(site.DestinationFolder, new[] { site.RootFolder }, errorHandling: FtpError.DeleteProcessed | FtpError.Throw);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, $"Error processing site {siteKey}");
            }
            finally
            {
                if(_client.IsConnected)
                    await _client.DisconnectAsync();
            }
        }

    }
}
