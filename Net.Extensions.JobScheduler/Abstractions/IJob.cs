namespace Net.Extensions.JobScheduler.Abstractions
{
    /// <summary>
    /// Representa una unidad de trabajo ejecutable por el JobScheduler.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Método de ejecución del job.
        /// </summary>
        /// <param name="context">Contexto de ejecución del job.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default);
    }
}
