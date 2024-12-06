using EasyPOS.Application.Common.BackgroundJobs;
using EasyPOS.Infrastructure.Persistence.Outbox;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPOS.Infrastructure.BackgroundJobs;

internal static class HangfireServiceExtension
{
    public static IServiceCollection AddHangfireService(this IServiceCollection services, string connectionString)
    {
        // Configure SQL Server storage for Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        // Add the Hangfire server
        services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            options.ServerTimeout = TimeSpan.FromMinutes(5);
        });

        // Jobs
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesDapperJob>();


        return services;
    }
}
