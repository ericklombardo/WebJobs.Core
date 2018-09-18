using Remax.WebJobs.Settings;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public interface IFtpManager
    {
        Task SyncDirectory(SiteSetting site, string siteKey);
    }
}