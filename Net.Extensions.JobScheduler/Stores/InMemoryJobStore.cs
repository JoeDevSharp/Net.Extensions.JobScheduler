using System.Collections.Concurrent;
using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;

namespace Net.Extensions.JobScheduler.Stores
{
    /// <summary>
    /// Almacenamiento en memoria para descriptores de jobs.
    /// No persistente: se pierde al reiniciar la aplicación.
    /// </summary>
    public class InMemoryJobStore : IJobStore
    {
        private readonly ConcurrentDictionary<string, JobDescriptor> _store = new();

        public void Save(JobDescriptor descriptor)
        {
            _store[descriptor.Id] = descriptor;
        }

        public IEnumerable<JobDescriptor> GetAll()
        {
            return _store.Values;
        }
    }
}
