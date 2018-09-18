using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentFTP;

namespace Remax.WebJobs
{
    public static class FtpClientExtensions
    {
        public static async Task DownloadFolderRecursive(this IFtpClient client, string source, string destination)
        {
            var entries = await client.GetListingAsync(source);
            foreach (var item in entries.Where(x => x.Type == FtpFileSystemObjectType.File))
            {
                try
                {
                    var result = await client.DownloadFileAsync($"{destination}\\{item.Name}", item.FullName);
                    if (!result)
                        throw new FtpException($"Error downloading file {item.FullName}");
                }
                catch (FtpException exc)
                {
                    if (exc.InnerException is FtpCommandException ftpCommandException)
                    {
                        throw new FtpException($"Error downloading file {item.FullName}. Response type {ftpCommandException.ResponseType}", ftpCommandException);
                    }

                    throw new FtpException($"Error downloading file {item.FullName}", exc);
                }
            }

            foreach (var item in entries.Where(x => x.Type == FtpFileSystemObjectType.Directory))
            {
                var newDestination = $@"{destination}\{item.Name}";
                if (!Directory.Exists(newDestination))
                {
                    Directory.CreateDirectory(newDestination);
                }
                await client.DownloadFolderRecursive($"{source}/{item.Name}", newDestination);
            }
        }
    }
}