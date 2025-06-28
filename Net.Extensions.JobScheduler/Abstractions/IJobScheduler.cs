namespace Net.Extensions.JobScheduler.Abstractions
{
    /// <summary>
    /// Expone las operaciones principales del planificador de trabajos.
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// Registra un trabajo en el planificador.
        /// </summary>
        /// <param name="descriptor">Descriptor del trabajo.</param>
        void Register(JobDescriptor descriptor);

        /// <summary>
        /// Inicia el planificador y ejecuta los trabajos programados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Una tarea que representa la ejecución del planificador.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Detiene el planificador y todos los trabajos activos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Una tarea que representa la operación de detención.</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
