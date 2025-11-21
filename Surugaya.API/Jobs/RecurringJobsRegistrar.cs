using Hangfire;
using Microsoft.Extensions.Options;
using Surugaya.API.Configuration;
using Surugaya.API.Services;
using Surugaya.API.Settings;

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

        // 註冊健康檢查任務
        recurringJobManager.AddOrUpdate<HealthCheckJobService>(
            recurringJobId: "健康檢查",
            methodCall: service => service.ExecuteHealthCheckAsync(null),
            cronExpression: "*/10 * * * *",
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });

        // 註冊駿河屋爬蟲任務
        var scraperSettings = app.ApplicationServices
            .GetRequiredService<IOptions<SurugayaScraperSettings>>().Value;

        if (scraperSettings.Enabled)
        {
            recurringJobManager.AddOrUpdate<SurugayaScraperJob>(
                recurringJobId: "爬蟲任務",
                methodCall: service => service.ExecuteAsync(null),
                cronExpression: scraperSettings.CronExpression,
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local
                });

            Console.WriteLine($"✓ 駿河屋爬蟲任務已註冊，執行排程: {scraperSettings.CronExpression}");
        }
        else
        {
            Console.WriteLine("⚠ 駿河屋爬蟲任務已停用");
        }
    }
}
