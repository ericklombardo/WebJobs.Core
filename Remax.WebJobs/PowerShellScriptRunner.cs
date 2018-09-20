using System.Collections;
using System.IO;
using System.Management.Automation;
using Microsoft.Extensions.Logging;

namespace Remax.WebJobs
{
    public class PowerShellScriptRunner : IPowerShellScriptRunner
    {

        private readonly ILogger _logger;

        public PowerShellScriptRunner(ILogger<PowerShellScriptRunner> logger)
        {
            _logger = logger;
        }

        public void Execute(string scriptFileName, IDictionary scriptParams)
        {
            using (var ps = PowerShell.Create())
            {
                _logger.LogInformation($"Executing scripts {scriptFileName}");
                var results = ps.AddScript(File.ReadAllText($"scripts\\{scriptFileName}"))
                    .AddParameters(scriptParams)
                    .Invoke();

                foreach (var item in results)
                {
                    if(item == null) continue;
                    
                    _logger.LogInformation($"{item}");
                }
            }
        }
    }
}