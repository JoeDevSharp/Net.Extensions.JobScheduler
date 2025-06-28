using System;
using System.Threading;
using System.Threading.Tasks;
using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;

namespace Net.Extensions.JobScheduler.Policies
{
    public class TimeoutPolicy : IJobPolicy
    {
        private readonly TimeSpan _timeout;

        public TimeoutPolicy(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "El timeout debe ser mayor que cero.");

            _timeout = timeout;
        }

        public async Task<JobResult> ExecuteAsync(IJob job, JobContext context, CancellationToken cancellationToken = default)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(_timeout);

            try
            {
                await job.ExecuteAsync(context, timeoutCts.Token);
                return JobResult.Success();
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                return JobResult.Failed(new TimeoutException($"El job '{context.JobId}' excedió el tiempo límite de {_timeout.TotalSeconds} segundos."));
            }
            catch (Exception ex)
            {
                return JobResult.Failed(ex);
            }
        }
    }
}
