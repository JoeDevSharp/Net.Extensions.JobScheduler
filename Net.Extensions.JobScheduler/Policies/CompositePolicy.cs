using Net.Extensions.JobScheduler.Abstractions;

namespace Net.Extensions.JobScheduler.Policies
{
    /// <summary>
    /// Política que compone dos políticas: la exterior ejecuta a la interior.
    /// </summary>
    public class CompositePolicy : IJobPolicy
    {
        private readonly IJobPolicy _outer;
        private readonly IJobPolicy _inner;

        public CompositePolicy(IJobPolicy outer, IJobPolicy inner)
        {
            _outer = outer;
            _inner = inner;
        }

        public Task<JobResult> ExecuteAsync(IJob job, JobContext context, CancellationToken cancellationToken = default)
        {
            // La política exterior ejecuta la política interior con el job real
            return _outer.ExecuteAsync(new WrappedJob(job, _inner), context, cancellationToken);
        }

        /// <summary>
        /// Job wrapper que ejecuta el job real usando la política interior.
        /// </summary>
        private class WrappedJob : IJob
        {
            private readonly IJob _innerJob;
            private readonly IJobPolicy _innerPolicy;

            public WrappedJob(IJob innerJob, IJobPolicy innerPolicy)
            {
                _innerJob = innerJob;
                _innerPolicy = innerPolicy;
            }

            public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
            {
                return _innerPolicy.ExecuteAsync(_innerJob, context, cancellationToken);
            }
        }
    }
}
