using FluentFTP;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class FtpManager : IFtpManager
    {

        private readonly ILogger _logger;
        private readonly IFtpClient _client;
        private List<FtpListItem> _ftpListItems = new List<FtpListItem>();

        public FtpManager(ILogger<FtpManager> logger, IFtpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task SyncDirectory(SiteSetting site, string siteKey)
        {
            _logger.LogInformation($"Starting sync {siteKey}");
            try
            {
                _client.Host = site.FtpServer;
                _client.Credentials = new System.Net.NetworkCredential(site.UserName, site.Password);
                _client.SocketPollInterval = 1000;
                _client.ConnectTimeout = 2000;
                _client.ReadTimeout = 2000;
                _client.DataConnectionConnectTimeout = 2000;
                _client.DataConnectionReadTimeout = 2000;
                _client.DataConnectionType = FtpDataConnectionType.PASV;
                await _client.ConnectAsync();
                await DownloadFolderRecursive(site.RootFolder, site.DestinationFolder);
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

        protected async Task DownloadFolderRecursive(string source, string destination)
        {
            var entries = await _client.GetListingAsync(source);
            foreach (var item in entries.Where(x => x.Type == FtpFileSystemObjectType.File))
            {
                try
                {
                    var result = await _client.DownloadFileAsync($"{destination}\\{item.Name}", item.FullName);
                    if (!result)
                        throw new FtpException($"Error downloading file {item.FullName}");
                }
                catch (FtpException exc)
                {
                    if (exc.InnerException is FtpCommandException ftpCommandException)
                    {
                        _logger.LogError(ftpCommandException, $"Error downloading file {item.FullName}. Response type {ftpCommandException.ResponseType}");
                    }
                    else
                    {
                        _logger.LogError($"Error downloading file {item.FullName}", exc);
                    }
                }
            }

            foreach (var item in entries.Where(x => x.Type == FtpFileSystemObjectType.Directory))
            {
                var newDestination = $@"{destination}\{item.Name}";
                if (!Directory.Exists(newDestination))
                {
                    Directory.CreateDirectory(newDestination);
                }
                await DownloadFolderRecursive($"{source}/{item.Name}", newDestination);
            }
        }

    }
}
