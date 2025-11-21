using System.Net.Http;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;

namespace Surugaya.API.Services;

/// <summary>
/// 健康檢查工作服務
/// </summary>
public class HealthCheckJobService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckJobService> _logger;
    private const string HealthCheckUrl = "https://surugaya.onrender.com/health";

    public HealthCheckJobService(
        IHttpClientFactory httpClientFactory,
        ILogger<HealthCheckJobService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// 執行健康檢查
    /// </summary>
    [JobDisplayName("健康檢查")]
    public async Task ExecuteHealthCheckAsync(PerformContext? context)
    {
        try
        {
            context?.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 開始執行健康檢查: {HealthCheckUrl}");
            _logger.LogInformation("開始執行健康檢查: {Url}", HealthCheckUrl);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            context?.WriteLine("發送 HTTP GET 請求...");
            var response = await httpClient.GetAsync(HealthCheckUrl);

            if (response.IsSuccessStatusCode)
            {
                context?.WriteLine($"✓ 健康檢查成功 - 狀態碼: {response.StatusCode}", ConsoleTextColor.Green);
                _logger.LogInformation("健康檢查成功 - 狀態碼: {StatusCode}", response.StatusCode);
            }
            else
            {
                context?.WriteLine($"✗ 健康檢查失敗 - 狀態碼: {response.StatusCode}", ConsoleTextColor.Yellow);
                _logger.LogWarning("健康檢查失敗 - 狀態碼: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            context?.WriteLine($"✗ 健康檢查發生錯誤: {ex.Message}", ConsoleTextColor.Red);
            _logger.LogError(ex, "健康檢查發生錯誤: {Message}", ex.Message);
        }
    }
}
