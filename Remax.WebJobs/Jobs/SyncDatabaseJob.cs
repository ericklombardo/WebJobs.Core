using Microsoft.Azure.WebJobs;

namespace Remax.WebJobs.Jobs
{
    public class SyncDatabaseJob
    {

        private readonly IPowerShellScriptRunner _powerShellScriptRunner;

        public SyncDatabaseJob(IPowerShellScriptRunner powerShellScriptRunner)
        {
            _powerShellScriptRunner = powerShellScriptRunner;
        }

        public  void SyncDatabase([QueueTrigger("%SyncDatabaseQueue%")] string json)
        {
            _powerShellScriptRunner.ExecuteScript("exportdb.ps1", null);
        }

    }
}
