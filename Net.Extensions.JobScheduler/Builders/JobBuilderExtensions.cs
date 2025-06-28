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
        /// Define un intervalo recurrente en horas.
        /// </summary>
        public static JobBuilder EveryHours(this JobBuilder builder, int hours)
        {
            return builder.RecurringEvery(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// Define un intervalo recurrente en días.
        /// </summary>
        public static JobBuilder EveryDays(this JobBuilder builder, int days)
        {
            return builder.RecurringEvery(TimeSpan.FromDays(days));
        }

        /// <summary>
        /// Define un intervalo recurrente en semanas.
        /// </summary>
        public static JobBuilder EveryWeeks(this JobBuilder builder, int weeks)
        {
            return builder.RecurringEvery(TimeSpan.FromDays(7 * weeks));
        }

        /// <summary>
        /// Define un intervalo recurrente en meses (aproximado a 30 días por mes).
        /// </summary>
        public static JobBuilder EveryMonths(this JobBuilder builder, int months)
        {
            return builder.RecurringEvery(TimeSpan.FromDays(30 * months));
        }

        /// <summary>
        /// Define un intervalo recurrente en años (aproximado a 365 días por año).
        /// </summary>
        public static JobBuilder EveryYears(this JobBuilder builder, int years)
        {
            return builder.RecurringEvery(TimeSpan.FromDays(365 * years));
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
