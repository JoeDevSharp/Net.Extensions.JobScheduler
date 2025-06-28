using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.Extensions.JobScheduler.Abstractions;

namespace Net.Extensions.JobScheduler
{
    public class JobRunner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobRunner> _logger;

        public JobRunner(IServiceProvider serviceProvider, ILogger<JobRunner> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<JobResult> RunAsync(JobDescriptor descriptor, CancellationToken cancellationToken = default)                                   
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var job = (IJob)scope.ServiceProvider.GetRequiredService(descriptor.JobType);
                var policy = descriptor.Policy;

                var context = new JobContext(descriptor.Id, descriptor.Metadata);
                _logger.LogDebug("Ejecutando job {JobId}", descriptor.Id);

                return await policy.ExecuteAsync(job, context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al ejecutar el job {JobId}", descriptor.Id);
                return JobResult.Failed(ex);
            }
        }
    }
}
