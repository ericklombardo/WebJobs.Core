using System.Collections;
using System.IO;
using System.Management.Automation;
using Microsoft.Extensions.Logging;

namespace Remax.WebJobs
{
    public class PowerShellScriptRunner : IPowerShellScriptRunner
    {

        private readonly ILogger _logger;
        public bool Success { get; private set; }

        public PowerShellScriptRunner(ILogger<PowerShellScriptRunner> logger)
        {
            _logger = logger;
        }

        public void ExecuteScript(string scriptFileName, IDictionary scriptParams)
        {
            Success = true;
            using var psInstance = PowerShell.Create();
            _logger.LogInformation($"Executing scripts {scriptFileName}");
            psInstance.Streams.Error.DataAdded += (sender, args) =>
            {
                foreach (var errorRecord in (PSDataCollection<ErrorRecord>) sender)
                {
                    Success = false;
                    _logger.LogError(errorRecord.Exception, $"Error executing {scriptFileName}");
                }
            };
            var result = new PSDataCollection<PSObject>();
            result.DataAdded += (sender, args) =>
            {
                var output = ((PSDataCollection<PSObject>)sender)[args.Index];
                _logger.LogInformation($"OUTPUT: {output}");
            };
            psInstance
                .AddScript(File.ReadAllText($"scripts\\{scriptFileName}"))
                .AddParameters(scriptParams)
                .Invoke(null, result);
        }

    }
}