namespace Net.Extensions.JobScheduler
{
    /// <summary>
    /// Proporciona contexto de ejecución para un trabajo.
    /// </summary>
    public class JobContext
    {
        /// <summary>
        /// Identificador único del job.
        /// </summary>
        public string JobId { get; }

        /// <summary>
        /// Diccionario de metadatos asociados al job.
        /// </summary>
        public IDictionary<string, object>? Metadata { get; }

        /// <summary>
        /// Marca de tiempo de inicio.
        /// </summary>
        public DateTimeOffset StartedAt { get; } = DateTimeOffset.UtcNow;

        public JobContext(string jobId, IDictionary<string, object>? metadata = null)
        {
            JobId = jobId;
            Metadata = metadata;
        }
    }
}
