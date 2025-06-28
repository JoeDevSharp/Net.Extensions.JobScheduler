using Net.Extensions.JobScheduler.Abstractions;
using Net.Extensions.JobScheduler;

namespace Net.Extensions.JobScheduler.Builders
{
    public class JobBuilder
    {
        private string? _id;
        private Type? _jobType;
        private IJobPolicy? _policy;
        private TimeSpan? _recurrenceInterval;
        private IDictionary<string, object>? _metadata;

        private JobBuilder() { }

        public static JobBuilder Create() => new JobBuilder();

        public JobBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public JobBuilder WithJobType<TJob>() where TJob : IJob
        {
            _jobType = typeof(TJob);
            return this;
        }

        public JobBuilder WithPolicy(IJobPolicy policy)
        {
            _policy = policy;
            return this;
        }

        public JobBuilder RecurringEvery(TimeSpan interval)
        {
            _recurrenceInterval = interval;
            return this;
        }

        public JobBuilder WithMetadata(IDictionary<string, object> metadata)
        {
            _metadata = metadata;
            return this;
        }

        public JobDescriptor Build()
        {
            if (string.IsNullOrWhiteSpace(_id))
                throw new InvalidOperationException("Job id is required.");

            if (_jobType == null)
                throw new InvalidOperationException("Job type is required.");

            if (_policy == null)
                throw new InvalidOperationException("Job policy is required.");

            if (_recurrenceInterval.HasValue)
                return new JobDescriptor(_id, _jobType, _policy, _recurrenceInterval.Value, _metadata!);

            return new JobDescriptor(_id, _jobType, _policy, _metadata);
        }
    }
}
