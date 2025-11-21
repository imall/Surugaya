using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surugaya.API.Configuration;
using Surugaya.API.Settings;
using Surugaya.Service;

namespace Surugaya.API.Services;

/// <summary>
/// 駿河屋爬蟲 Hangfire Job 服務
/// 使用 Hangfire 執行定期爬蟲任務
/// </summary>
public class SurugayaScraperJob
{
    private readonly SurugayaScraperService _scraperService;
    private readonly ILogger<SurugayaScraperJob> _logger;
    private readonly SurugayaScraperSettings _settings;

    /// <summary>
    /// 初始化駿河屋爬蟲 Job 服務
    /// </summary>
    /// <param name="scraperService">爬蟲服務</param>
    /// <param name="logger">日誌記錄器</param>
    /// <param name="options">爬蟲設定選項</param>
    public SurugayaScraperJob(
        SurugayaScraperService scraperService,
        ILogger<SurugayaScraperJob> logger,
        IOptions<SurugayaScraperSettings> options)
    {
        _scraperService = scraperService;
        _logger = logger;
        _settings = options.Value;
    }

    /// <summary>
    /// 執行爬蟲任務
    /// </summary>
    /// <param name="context">Hangfire 執行上下文</param>
    /// <returns>非同步任務</returns>
    public async Task ExecuteAsync(PerformContext? context)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(_settings.TimeoutMinutes));
        
        try
        {
            var startTime = DateTime.Now;
            _logger.LogInformation("開始執行駿河屋爬蟲任務 - {DateTime}", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
            context?.WriteLine($"開始執行駿河屋爬蟲任務 - {startTime:yyyy-MM-dd HH:mm:ss}");

            // 建立 writeLog 委派函數,將 log 輸出到 Hangfire Console
            Action<string>? writeLog = context != null 
                ? context.WriteLine 
                : null;

            var results = await _scraperService.ScrapeAllProductInfo(writeLog);
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            var resultCount = results?.Count() ?? 0;
            
            _logger.LogInformation("駿河屋爬蟲任務完成 - 處理了 {Count} 個商品，耗時: {Duration}", 
                resultCount, duration.ToString(@"hh\:mm\:ss"));
            context?.WriteLine($"駿河屋爬蟲任務完成 - 處理了 {resultCount} 個商品，耗時: {duration:hh\\:mm\\:ss}");
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            var errorMessage = $"駿河屋爬蟲任務執行超時 ({_settings.TimeoutMinutes} 分鐘)";
            _logger.LogWarning(errorMessage);
            context?.WriteLine(errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行駿河屋爬蟲任務時發生錯誤");
            context?.WriteLine($"執行駿河屋爬蟲任務時發生錯誤: {ex.Message}");
            throw;
        }
    }
}
