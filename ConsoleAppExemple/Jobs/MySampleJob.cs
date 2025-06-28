using Microsoft.Extensions.Logging;
using Net.Extensions.JobScheduler;
using Net.Extensions.JobScheduler.Abstractions;

namespace ConsoleAppExemple.Jobs
{
    public class MySampleJob : IJob
    {
        private readonly ILogger<MySampleJob> _logger;

        public MySampleJob(ILogger<MySampleJob> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Ejecutando MySampleJob con ID: {JobId} en {Time}", context.JobId, DateTimeOffset.Now);
            // Aquí la lógica real de tu job
            return Task.CompletedTask;
        }
    }
}
