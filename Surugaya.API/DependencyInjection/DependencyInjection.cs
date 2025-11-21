using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Options;
using Supabase;
using Surugaya.API.Configuration;
using Surugaya.API.Settings;

namespace Surugaya.API.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;

public static class DependencyInjection
{
    /// <summary>
    /// 新增 supabase 服務
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddSupabase(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        services.AddSingleton<Client>(_ =>
        {
            var supabaseSettings = new SupabaseSettings();
            configuration.GetSection("Supabase").Bind(supabaseSettings);

            if (string.IsNullOrEmpty(supabaseSettings.Url))
                throw new InvalidOperationException("Supabase URL 未設定");

            if (string.IsNullOrEmpty(supabaseSettings.AnonKey))
                throw new InvalidOperationException("Supabase AnonKey 未設定");

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false 
            };
            
            var apiKey = supabaseSettings.GetApiKey();
            var client = new Client(supabaseSettings.Url, apiKey, options);
            client.InitializeAsync().Wait();
            return client;
        });
        
        return services;
    }
    
    
    public static IServiceCollection AddHangFireServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 從 Supabase 取得連線字串
        var supabaseConnectionString = configuration.GetConnectionString("Supabase");
        
        services.AddHangfire((sp, config) =>
        {
            var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>();
            
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => 
                    options.UseNpgsqlConnection(supabaseConnectionString));
        });

        services.AddHangfireServer(config =>
        {
            var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>();
            config.ServerName = $"{Environment.MachineName}:{hangfireSettings!.ServerName}";
            config.WorkerCount = hangfireSettings.WorkerCount;
            config.Queues = hangfireSettings.Queues;
        });

        return services;
    }
}