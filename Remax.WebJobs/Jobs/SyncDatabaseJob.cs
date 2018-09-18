using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Remax.WebJobs.Jobs
{
    public class SyncDatabaseJob
    {
        public async Task SyncDatabase([QueueTrigger("%SyncDatabaseQueue%")] string json)
        {

        }
    }
}
