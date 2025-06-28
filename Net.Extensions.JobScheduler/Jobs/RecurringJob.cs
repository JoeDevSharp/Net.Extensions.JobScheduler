using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Extensions.JobScheduler.Jobs
{
    /// <summary>
    /// Job recurrente que ejecuta una tarea específica.
    /// </summary>
    public class RecurringJob : IJob
    {
        private readonly Func<CancellationToken, Task> _action;

        public RecurringJob(Func<CancellationToken, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
        {
            return _action(cancellationToken);
        }
    }
}
