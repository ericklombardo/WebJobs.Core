using System.Collections;

namespace Remax.WebJobs
{
    public interface IPowerShellScriptRunner
    {
        void Execute(string scriptFileName, IDictionary scriptParams);
    }
}