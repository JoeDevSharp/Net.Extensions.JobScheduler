namespace Net.Extensions.JobScheduler.Abstractions
{
    /// <summary>
    /// Representa una política de ejecución para un trabajo, como reintentos, timeouts, etc.
    /// </summary>
    public interface IJobPolicy
    {
        /// <summary>
        /// Ejecuta la lógica del trabajo aplicando la política definida.
        /// </summary>
        /// <param name="job">Instancia del trabajo a ejecutar.</param>
        /// <param name="context">Contexto del trabajo.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Resultado de la ejecución del trabajo.</returns>
        Task<JobResult> ExecuteAsync(IJob job, JobContext context, CancellationToken cancellationToken = default);
    }
}
