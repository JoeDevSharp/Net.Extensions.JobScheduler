using System;

namespace Net.Extensions.JobScheduler
{
    /// <summary>
    /// Resultado de la ejecución de un job.
    /// </summary>
    public class JobResult
    {
        public bool IsSuccess { get; }
        public Exception? Exception { get; }
        public string Status => IsSuccess ? "Success" : "Failed";

        private JobResult(bool isSuccess, Exception? exception = null)
        {
            IsSuccess = isSuccess;
            Exception = exception;
        }

        public static JobResult Success() => new JobResult(true);

        public static JobResult Failed(Exception? exception = null) => new JobResult(false, exception);
    }
}
