using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Remax.WebJobs
{
    public class CopySiteWebJob
    {
        private readonly ILogger<CopySiteWebJob> _logger;
        private readonly string abc;

        public CopySiteWebJob(ILogger<CopySiteWebJob> logger)
        {
            _logger = logger;
        }

        public Task DoSomethingUseful([TimerTrigger("%ScheduleExpression%", RunOnStartup = true)] TimerInfo timerInfo, TextWriter log)
        {
            // Act on the DI-ed class:
            Console.WriteLine($"{DateTime.Now} - prueba");
            return Task.CompletedTask;
        }

        public Task DoSomethingOnAQueue([QueueTrigger("copy-site")] int id)
        {
            return Task.CompletedTask;
        }
    }
}
