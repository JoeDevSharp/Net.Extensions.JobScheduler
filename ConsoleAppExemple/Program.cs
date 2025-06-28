using ConsoleAppExemple.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler.Builders;
using Net.Extensions.JobScheduler.Extensions;

namespace ConsoleAppExemple
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    // Registra tu JobScheduler y servicios relacionados
                    services.AddJobScheduler();

                    // Registrar jobs concretos (puede ser un job definido aquí o externo)
                    services.AddTransient<MySampleJob>();
                })
                .Build();

            // Registrar un job recurrente con builder y política de retry
            var scheduler = host.Services.GetRequiredService<IJobScheduler>();

            scheduler.Register(JobBuilder.Create()
                .WithId("job1")
                .WithJobType<MySampleJob>()
                .EverySeconds(1)
                .WithSimpleRetry()
                .Build());

            // Inicia el scheduler
            await scheduler.StartAsync();

            Console.WriteLine("JobScheduler iniciado. Presiona Ctrl+C para salir.");

            // Mantener la app viva hasta que se cancele
            await host.WaitForShutdownAsync();
        }
    }
}
