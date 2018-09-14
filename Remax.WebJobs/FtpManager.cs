using FluentFTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class FtpManager : IFtpManager
    {

        private readonly IFtpClient _client;

        public FtpManager(IFtpClient ftpClient)
        {
            _client = ftpClient;
        }

        public async Task SyncDirectory(SiteSetting site)
        {
            _client.Host = site.FtpServer;
            _client.Credentials = new NetworkCredential(site.UserName, site.Password);

            await _client.ConnectAsync();
        }

    }
}
