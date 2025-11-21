using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Options;
using Supabase;
using Surugaya.API.Configuration;
using Surugaya.API.Settings;

namespace Surugaya.API.DependencyInjection;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Npgsql;

/// <summary>
/// 依賴注入擴展方法
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 標記 Hangfire 是否已成功初始化
    /// </summary>
    public static bool IsHangfireEnabled { get; private set; } = false;
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
    
    /// <summary>
    /// 新增 Hangfire 服務（包含連線測試）
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddHangFireServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        try
        {
            // 從 Supabase 取得連線字串
            var supabaseConnectionString = configuration.GetConnectionString("Supabase");
            
            if (string.IsNullOrEmpty(supabaseConnectionString))
            {
                Console.WriteLine("⚠ 警告: Supabase 連線字串未設定，跳過 Hangfire 服務初始化");
                IsHangfireEnabled = false;
                return services;
            }

            // 測試資料庫連線
            if (!TestDatabaseConnection(supabaseConnectionString))
            {
                Console.WriteLine("⚠ 警告: 無法連線到資料庫，跳過 Hangfire 服務初始化");
                IsHangfireEnabled = false;
                return services;
            }

            services.AddHangfire((sp, config) =>
            {
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

            Console.WriteLine("✓ Hangfire 服務已成功初始化");
            IsHangfireEnabled = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ 警告: Hangfire 服務初始化失敗，將跳過啟用: {ex.Message}");
            IsHangfireEnabled = false;
        }

        return services;
    }

    /// <summary>
    /// 測試資料庫連線
    /// </summary>
    /// <param name="connectionString">連線字串</param>
    /// <returns>連線是否成功</returns>
    private static bool TestDatabaseConnection(string connectionString)
    {
        try
        {
            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            connection.Open();
            
            // 執行簡單查詢測試連線
            using var command = new Npgsql.NpgsqlCommand("SELECT 1", connection);
            command.ExecuteScalar();
            
            Console.WriteLine("✓ 資料庫連線測試成功");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 資料庫連線測試失敗: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全地配置 Hangfire Dashboard（僅在 Hangfire 服務已啟用時）
    /// </summary>
    /// <param name="app">應用程式建構器</param>
    /// <param name="pathMatch">Dashboard 路徑</param>
    /// <param name="options">Dashboard 選項</param>
    /// <returns>應用程式建構器</returns>
    public static IApplicationBuilder UseHangfireDashboardSafely(
        this IApplicationBuilder app, 
        string pathMatch = "/hangfire",
        DashboardOptions? options = null)
    {
        if (IsHangfireEnabled)
        {
            options ??= new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>(),
                IgnoreAntiforgeryToken = true
            };

            app.UseHangfireDashboard(pathMatch, options);
            Console.WriteLine($"✓ Hangfire Dashboard 已啟用，路徑: {pathMatch}");
        }
        else
        {
            Console.WriteLine("⚠ 跳過 Hangfire Dashboard 配置（服務未啟用）");
        }

        return app;
    }
}