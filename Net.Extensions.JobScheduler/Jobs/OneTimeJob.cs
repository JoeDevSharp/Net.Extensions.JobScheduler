using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;

namespace Net.Extensions.JobScheduler.Jobs
{
    /// <summary>
    /// Job único que ejecuta una acción una sola vez.
    /// </summary>
    public class OneTimeJob : IJob
    {
        private readonly Func<CancellationToken, Task> _action;

        public OneTimeJob(Func<CancellationToken, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
        {
            return _action(cancellationToken);
        }
    }
}
