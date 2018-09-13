using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.Extensions.DependencyInjection;


namespace Remax.WebJobs
{
    public class CustomJobActivator : IJobActivator
    {

        private readonly IServiceProvider _service;
        public CustomJobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            return _service.GetService<T>();
        }
    }
}
