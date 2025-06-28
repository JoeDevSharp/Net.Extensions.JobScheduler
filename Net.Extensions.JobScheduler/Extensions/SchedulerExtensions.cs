using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;
using Net.Extensions.JobScheduler.Stores;
using Microsoft.Extensions.Logging;

namespace Net.Extensions.JobScheduler.Extensions
{
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Registra los servicios necesarios para usar el JobScheduler.
        /// </summary>
        public static IServiceCollection AddJobScheduler(this IServiceCollection services)
        {
            services.AddSingleton<IJobStore, InMemoryJobStore>();
            services.AddSingleton<IJobScheduler, JobScheduler>();
            services.AddSingleton<JobRunner>();
            return services;
        }

        /// <summary>
        /// Inicia el scheduler como parte del ciclo de vida del host.
        /// </summary>
        public static async Task<IHost> UseJobSchedulerAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            var scheduler = host.Services.GetRequiredService<IJobScheduler>();
            var logger = host.Services.GetService<ILoggerFactory>()?.CreateLogger("JobScheduler");

            logger?.LogInformation("Iniciando el JobScheduler...");
            await scheduler.StartAsync(cancellationToken);
            return host;
        }
    }
}
