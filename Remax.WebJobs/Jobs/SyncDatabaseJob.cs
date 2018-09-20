using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Remax.WebJobs.Jobs
{
    public class SyncDatabaseJob
    {

        private readonly IPowerShellScriptRunner _powerShellScriptRunner;

        public SyncDatabaseJob(IPowerShellScriptRunner powerShellScriptRunner)
        {
            _powerShellScriptRunner = powerShellScriptRunner;
        }

        public async Task SyncDatabase(/*[QueueTrigger("%SyncDatabaseQueue%")]*/ string json)
        {

        }

        public void SyncDatabase([TimerTrigger("* 0 7 * * 1-5", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            _powerShellScriptRunner.Execute("exportdb.ps1", null);
        }
    }
}
