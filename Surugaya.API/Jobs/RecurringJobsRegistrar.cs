using Hangfire;
using Surugaya.API.Services;

namespace Surugaya.API.Jobs;

/// <summary>
/// 註冊 Recurring Jobs 擴充方法
/// </summary>
public static class RecurringJobsRegistrar
{
    /// <summary>
    /// 註冊 Recurring Jobs
    /// </summary>
    public static void RegisterRecurringJobs(this IApplicationBuilder app)
    {
        var recurringJobManager = app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<HealthCheckJobService>(
            recurringJobId: "health-check-job",
            methodCall: service => service.ExecuteHealthCheckAsync(null),
            cronExpression: "*/12 * * * *",
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });
    }
}
