using System.Collections;

namespace Remax.WebJobs
{
    public interface IPowerShellScriptRunner
    {
        void ExecuteScript(string scriptFileName, IDictionary scriptParams);
    }
}