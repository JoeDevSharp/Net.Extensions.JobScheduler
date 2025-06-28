using System;
using Net.Extensions.JobScheduler.Builders;
using Net.Extensions.JobScheduler.Policies;

namespace Net.Extensions.JobScheduler.Builders
{
    public static class JobBuilderExtensions
    {
        /// <summary>
        /// Define un intervalo recurrente en minutos.
        /// </summary>
        public static JobBuilder EveryMinutes(this JobBuilder builder, int minutes)
        {
            return builder.RecurringEvery(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Define un intervalo recurrente en segundos.
        /// </summary>
        public static JobBuilder EverySeconds(this JobBuilder builder, int seconds)
        {
            return builder.RecurringEvery(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Añade una política de reintento simple.
        /// </summary>
        public static JobBuilder WithSimpleRetry(this JobBuilder builder, int maxAttempts = 3, TimeSpan? delay = null)
        {
            return builder.WithPolicy(new RetryPolicy(maxAttempts, delay ?? TimeSpan.FromSeconds(5)));
        }

        /// <summary>
        /// Añade una política combinada de timeout y reintento.
        /// </summary>
        public static JobBuilder WithRetryAndTimeout(this JobBuilder builder, int maxAttempts, TimeSpan delay, TimeSpan timeout)
        {
            return builder.WithPolicy(new RetryPolicy(maxAttempts, delay).Wrap(new TimeoutPolicy(timeout)));
        }
    }
}
