using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Net.Extensions.JobScheduler.Abstractions;

namespace Net.Extensions.JobScheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ConcurrentDictionary<string, JobDescriptor> _jobs = new();
        private readonly IJobStore _jobStore;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobScheduler> _logger;
        private CancellationTokenSource? _cts;

        public JobScheduler(
            IJobStore jobStore,
            IServiceScopeFactory scopeFactory,
            ILogger<JobScheduler> logger)
        {
            _jobStore = jobStore;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Register(JobDescriptor descriptor)
        {
            if (_jobs.TryAdd(descriptor.Id, descriptor))
            {
                _jobStore.Save(descriptor);
                _logger.LogInformation("Job registrado: {JobId}", descriptor.Id);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _logger.LogInformation("JobScheduler iniciado.");

            foreach (var descriptor in _jobStore.GetAll())
            {
                _ = RunJobLoopAsync(descriptor, _cts.Token); // Fire & forget
            }

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("JobScheduler detenido.");
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async Task RunJobLoopAsync(JobDescriptor descriptor, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var delay = descriptor.GetNextDelay();
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken);

                    using var scope = _scopeFactory.CreateScope();
                    var job = (IJob)scope.ServiceProvider.GetRequiredService(descriptor.JobType);
                    var context = new JobContext(descriptor.Id, descriptor.Metadata);
                    var result = await descriptor.Policy.ExecuteAsync(job, context, cancellationToken);

                    _logger.LogInformation("Job {JobId} finalizado con estado: {Status}", descriptor.Id, result.Status);
                }
                catch (TaskCanceledException)
                {
                    break; // Terminación esperada
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando job {JobId}", descriptor.Id);
                }

                if (!descriptor.IsRecurring)
                    break;
            }
        }
    }
}
