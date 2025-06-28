using Net.Extensions.JobScheduler.Abstractions;
using System;

namespace Net.Extensions.JobScheduler
{
    /// <summary>
    /// Describe un job registrado en el scheduler con su configuración y metadata.
    /// </summary>
    public class JobDescriptor
    {
        /// <summary>
        /// Identificador único del job.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Tipo concreto que implementa IJob.
        /// </summary>
        public Type JobType { get; }

        /// <summary>
        /// Política aplicada para la ejecución (reintentos, timeout, etc).
        /// </summary>
        public IJobPolicy Policy { get; }

        /// <summary>
        /// Indica si el job es recurrente o solo se ejecuta una vez.
        /// </summary>
        public bool IsRecurring { get; }

        /// <summary>
        /// Intervalo para ejecución recurrente (si aplica).
        /// </summary>
        public TimeSpan? RecurrenceInterval { get; }

        /// <summary>
        /// Metadatos arbitrarios asociados al job.
        /// </summary>
        public IDictionary<string, object>? Metadata { get; }

        /// <summary>
        /// Constructor para jobs recurrentes.
        /// </summary>
        public JobDescriptor(string id, Type jobType, IJobPolicy policy, TimeSpan recurrenceInterval, IDictionary<string, object>? metadata = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            IsRecurring = true;
            RecurrenceInterval = recurrenceInterval;
            Metadata = metadata;
        }

        /// <summary>
        /// Constructor para jobs one-time.
        /// </summary>
        public JobDescriptor(string id, Type jobType, IJobPolicy policy, IDictionary<string, object>? metadata = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            IsRecurring = false;
            RecurrenceInterval = null;
            Metadata = metadata;
        }

        /// <summary>
        /// Calcula el tiempo a esperar para la próxima ejecución.
        /// </summary>
        /// <returns>Tiempo a delay antes de ejecutar el job.</returns>
        public TimeSpan GetNextDelay()
        {
            // Para jobs one-time, ejecuta inmediatamente (no delay)
            if (!IsRecurring)
                return TimeSpan.Zero;

            return RecurrenceInterval ?? TimeSpan.Zero;
        }
    }
}
