namespace Net.Extensions.JobScheduler.Abstractions
{
    /// <summary>
    /// Define un contrato para el almacenamiento de trabajos programados.
    /// </summary>
    public interface IJobStore
    {
        /// <summary>
        /// Guarda un descriptor de trabajo en el almacenamiento.
        /// </summary>
        /// <param name="descriptor">El descriptor del trabajo.</param>
        void Save(JobDescriptor descriptor);

        /// <summary>
        /// Recupera todos los trabajos almacenados.
        /// </summary>
        /// <returns>Enumeración de descriptores de trabajo.</returns>
        IEnumerable<JobDescriptor> GetAll();
    }
}