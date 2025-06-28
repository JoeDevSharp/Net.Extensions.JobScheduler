# Net.Extensions.JobScheduler

## Overview

`Net.Extensions.JobScheduler` is a lightweight yet powerful framework for scheduling and executing background jobs in .NET applications. It was designed to be simple, extensible, and fully compatible with modern .NET idioms like Dependency Injection (`DI`) and the Generic Host (`IHost`). The framework supports both recurring and one-time jobs, with flexible execution policies such as retry and timeout, and a modular architecture that allows custom extensions for job storage, policies, and execution logic.

This README provides a comprehensive guide to understanding, configuring, and extending the framework for various use cases — from small console apps to large industrial or legacy system modernization projects.

---

## Why a Job Scheduler Framework?

Many applications require background tasks — for example, sending emails, cleaning up data, synchronizing with external systems, or running periodic reports. While .NET offers several options (Quartz.NET, Hangfire, Windows Services), these can sometimes be overly complex, heavyweight, or not fully customizable for specific business needs.

`Net.Extensions.JobScheduler` targets scenarios where you need:

- **Fine-grained control:** Define how and when jobs run, with custom retry or timeout policies.
- **Minimal dependencies:** Avoid large frameworks and keep the dependency graph clean.
- **Integration:** Seamlessly work with the Microsoft DI and Hosting ecosystem.
- **Extensibility:** Easily create your own job policies, stores, or job types.
- **Modernization:** Facilitate incremental migration of legacy batch jobs into modern async .NET code.

---

## Project Architecture

The project is organized into several core folders representing clear responsibilities:

### 1. Abstractions

Defines the main interfaces and contracts that the rest of the system implements:

- **IJob:** Represents an executable unit of work. Any class implementing this can be scheduled and run by the scheduler.
- **IJobScheduler:** Orchestrates job registration, scheduling, and lifecycle management (start/stop).
- **IJobPolicy:** Encapsulates behavior that wraps job execution, such as retries or timeouts.
- **IJobStore:** Abstracts storage and retrieval of job metadata (`JobDescriptor`). Allows for pluggable persistence (in-memory, database, etc.).

This separation ensures your scheduler remains decoupled and testable.

### 2. Core

Contains the main engine classes:

- **JobScheduler:** Central class managing job lifecycles and execution loops. Uses DI to resolve job instances scoped per execution.
- **JobRunner:** Handles execution of jobs applying configured policies.
- **JobContext:** Carries execution metadata and state for each job run.
- **JobDescriptor:** Holds metadata such as job ID, type, schedule, policy, and custom metadata.
- **JobResult:** Represents the outcome of a job execution, including success or failure details.

### 3. Jobs

Predefined job types like:

- **RecurringJob:** Jobs scheduled to run repeatedly on a defined interval.
- **OneTimeJob:** Jobs scheduled to run once.

You can extend and create your own job types based on `IJob`.

---

## ⏰ Scheduling Intervals and Calendar Considerations

This framework provides convenient extension methods to specify recurring job intervals:

- `EverySeconds(int seconds)`
- `EveryMinutes(int minutes)`
- `EveryHours(int hours)`
- `EveryDays(int days)`
- `EveryWeeks(int weeks)`
- `EveryMonths(int months)` (approximate)
- `EveryYears(int years)` (approximate)

**Important:**

For `EveryMonths` and `EveryYears`, the scheduling uses a fixed `TimeSpan` approximation:

- Months are treated as **30 days** each.
- Years are treated as **365 days** each.

This means the actual calendar variation (months of 28-31 days, leap years) is **not** accounted for.

`TimeSpan` represents a fixed duration and does not natively support calendar-aware intervals. Therefore, using these methods schedules jobs at approximately these intervals, which is usually acceptable for many scenarios but **not suitable** when precise calendar dates/times are critical (e.g., running on the 1st day of each month).

If your use case requires precise calendar scheduling — accounting for variable month lengths or leap years — a different scheduling logic must be implemented, such as calculating the next run date explicitly using calendar-aware APIs (e.g., `DateTime.AddMonths()`, `DateTime.AddYears()`).

---

### 4. Builders

Fluent API classes and extension methods to build and configure jobs cleanly, for example:

- `.EveryMinutes(int)` to define recurring schedules.
- `.WithSimpleRetry()` to add a retry policy with default settings.

### 5. Policies

Execution policies control how jobs are run:

- **RetryPolicy:** Retries job execution upon failure, configurable by attempts and delay.
- **TimeoutPolicy:** Cancels jobs running longer than a specified time.
- **CompositePolicy:** Enables composing multiple policies for complex behaviors (e.g., retry with timeout).

Policies implement `IJobPolicy` and wrap job executions, enforcing constraints or behaviors.

### 6. Stores

Implementations of `IJobStore`. The default:

- **InMemoryJobStore:** Stores job descriptors in-memory for simplicity. Can be replaced with database-backed stores.

### 7. Extensions

Helpers for integrating the scheduler with the .NET DI and Hosting ecosystem, including extension methods to register scheduler services and start the scheduler when the host runs.

---

## Core Concepts and Interfaces in Detail

### IJob

The fundamental unit of work.

```csharp
public interface IJob
{
    Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default);
}
```

- `ExecuteAsync` contains the logic your job performs.
- The `JobContext` provides metadata like job ID and user-defined info.
- Cancellation support is built-in via `CancellationToken`.

### IJobScheduler

Manages jobs and their execution lifecycle:

```csharp
public interface IJobScheduler
{
    void Register(JobDescriptor descriptor);
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
```

- `Register` adds jobs to the scheduler.
- `StartAsync` launches all scheduled jobs asynchronously.
- `StopAsync` requests cancellation and graceful shutdown.

### IJobPolicy

Defines how jobs are executed beyond just calling `ExecuteAsync`:

```csharp
public interface IJobPolicy
{
    Task<JobResult> ExecuteAsync(IJob job, JobContext context, CancellationToken cancellationToken = default);
}
```

Allows behaviors like retrying, timing out, logging, or other cross-cutting concerns without modifying job code.

### IJobStore

Storage abstraction for job metadata:

```csharp
public interface IJobStore
{
    void Save(JobDescriptor descriptor);
    IEnumerable<JobDescriptor> GetAll();
}
```

Allows persisting job schedules and metadata, decoupling scheduling logic from persistence.

---

## Defining and Registering Jobs

Jobs are defined as classes implementing `IJob`. For example:

```csharp
public class MyJob : IJob
{
    private readonly ILogger<MyJob> _logger;
    public MyJob(ILogger<MyJob> logger) => _logger = logger;

    public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running job {JobId} at {Time}", context.JobId, DateTimeOffset.Now);
        // Job logic here
        return Task.CompletedTask;
    }
}
```

Register the job with the scheduler using the fluent builder API:

```csharp
scheduler.Register(JobBuilder.Create()
    .WithId("myjob")
    .WithJobType<MyJob>()
    .EveryMinutes(10)
    .WithSimpleRetry()
    .Build());
```

---

## Scheduling Models

- **RecurringJob:** Runs on a fixed interval (e.g., every 5 minutes).
- **OneTimeJob:** Runs only once at the next scheduling opportunity.

Both job types can be extended or customized.

---

## Execution Policies

### RetryPolicy

Retries job execution on failure.

```csharp
var retryPolicy = new RetryPolicy(maxAttempts: 3, delayBetweenAttempts: TimeSpan.FromSeconds(5));
```

### TimeoutPolicy

Cancels job execution after a timeout period.

```csharp
var timeoutPolicy = new TimeoutPolicy(TimeSpan.FromSeconds(30));
```

### CompositePolicy

Combine multiple policies for advanced behavior.

```csharp
var combined = retryPolicy.Wrap(timeoutPolicy);
```

---

## Dependency Injection & Scoped Execution

The scheduler resolves job instances from the DI container, creating a new scope for each execution. This allows using scoped services inside jobs (e.g., DbContext). Internally, this is done via `IServiceScopeFactory.CreateScope()` to ensure clean scope lifetimes.

---

## Integration with Microsoft.Extensions.Hosting

Designed to integrate naturally with `IHost`:

- Register scheduler and jobs in `ConfigureServices`.
- Start scheduler in `StartAsync` or via extension method.
- Use host’s lifetime and cancellation handling for graceful shutdown.

Example:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddJobScheduler();
        services.AddTransient<MyJob>();
    })
    .Build();

var scheduler = host.Services.GetRequiredService<IJobScheduler>();
scheduler.Register(...);
await scheduler.StartAsync();
await host.RunAsync();
```

---

## Extensibility

- Add custom job policies by implementing `IJobPolicy`.
- Create new job stores by implementing `IJobStore` to persist to database or distributed caches.
- Define custom job types for special scheduling or execution needs.
- Extend builders with your own fluent API methods.

---

## Typical Use Cases

- Batch processing pipelines
- Scheduled data cleanup or archival
- Email or notification dispatchers
- Legacy system background jobs modernization
- Microservice background tasks with retry and timeout control

---

## Troubleshooting & Tips

- **Job never runs?** Ensure you call `StartAsync()` after registering jobs.
- **Scoped services not resolving?** Make sure you use DI scopes properly; the scheduler creates a scope per job execution.
- **Job stuck or long-running?** Use `TimeoutPolicy` to limit execution time.
- **Multiple policies?** Use `CompositePolicy` to combine retry and timeout logically.

---

## Contributing

Contributions are welcome! Please follow the usual GitHub flow:

1. Fork the repo
2. Create a descriptive branch
3. Write tests and update docs
4. Submit a PR for review
