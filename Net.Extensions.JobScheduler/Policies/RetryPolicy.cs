using Net.Extensions.JobScheduler.Abstractions;

namespace Net.Extensions.JobScheduler.Policies
{
    public class RetryPolicy : IJobPolicy
    {
        private readonly int _maxAttempts;
        private readonly TimeSpan _delay;

        public RetryPolicy(int maxAttempts = 3, TimeSpan? delay = null)
        {
            if (maxAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Debe ser al menos 1.");

            _maxAttempts = maxAttempts;
            _delay = delay ?? TimeSpan.FromSeconds(5);
        }

        public async Task<JobResult> ExecuteAsync(IJob job, JobContext context, CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            Exception? lastException = null;

            while (attempt < _maxAttempts && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    attempt++;
                    await job.ExecuteAsync(context, cancellationToken);
                    return JobResult.Success();
                }
                catch (Exception ex) when (attempt < _maxAttempts)
                {
                    lastException = ex;
                    await Task.Delay(_delay, cancellationToken);
                }
                catch (Exception ex)
                {
                    return JobResult.Failed(ex);
                }
            }

            return JobResult.Failed(lastException);
        }

        /// <summary>
        /// Composición funcional de políticas (ej: retry + timeout).
        /// </summary>
        public IJobPolicy Wrap(IJobPolicy inner)
        {
            return new CompositePolicy(this, inner);
        }
    }
}
